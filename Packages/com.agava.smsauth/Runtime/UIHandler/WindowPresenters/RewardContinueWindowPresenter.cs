using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Scripting;
using KinDzaDzaGames.AdvertisementPlugin;
using SmsAuthAPI.Program;
using System.Threading.Tasks;

namespace Agava.Wink
{
    [Preserve]
    internal class RewardContinueWindowPresenter : WindowPresenter
    {
        private const int OneMinute = 60;

        [SerializeField, Min(0)] private float _reloadAdDelay = 2;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private ImagesCarousel _imagesCarousel;
        [Header("Remote rewards data")]
        [SerializeField] private TMP_Text _winkSubsDescription;
        [SerializeField] private string _winkSubsDescriptionPattern = "{0}. {1}";
        [SerializeField] private string _trialPeriodDaysKey = "trial-period-days-text";
        [SerializeField] private string _defaultTrialPeriodDays = "30 дней за 0 руб";
        [SerializeField] private string _winkPriceKey = "wink-price-text";
        [SerializeField] private string _defaultWinkPrice = "ƒалее 199 р/мес€ц";
        [SerializeField] private string _defaultTimerGiftMinutesKey = "demo-overtime-minutes_";
        [SerializeField, Min(0)] private int _defaultTimerGiftMinutes = 10;
        [Header("Reward button")]
        [SerializeField] private Button _rewardDemoTimeButton;
        [SerializeField] private TMP_Text _rewardButtonLabel;
        [SerializeField] private TMP_Text _rewardButtonDiscription;
        [SerializeField] private string _rewardButtonDiscriptionPattern = "и играть ещЄ {0} минут";

        private DemoTimer _demoTimer;
        private Color _defaultTextColor;
        private Color _blinkTextColor;
        private Coroutine _reloadAd;
        private int _rewardMinutes = 10;

        public bool Initialized { get; private set; } = false;

        public event Action RewardSuccessed;

        public IEnumerator Construct(DemoTimer demoTimer)
        {
            _demoTimer = demoTimer ?? throw new ArgumentNullException(nameof(demoTimer));

            _defaultTextColor = _blinkTextColor = _rewardButtonLabel.color;
            _blinkTextColor.a = 0.5f;
            DeactivateRewardButton();

            yield return new WaitUntil(() => SmsAuthApi.Initialized);

            Task<string> trialTask = RemoteConfig.StringRemoteConfig(_trialPeriodDaysKey, string.Empty);
            yield return new WaitUntil(() => trialTask.IsCompleted);

            Task<string> priceTask = RemoteConfig.StringRemoteConfig(_winkPriceKey, string.Empty);
            yield return new WaitUntil(() => priceTask.IsCompleted);

            string trialResult = trialTask.Result;
            string priceResult = priceTask.Result;

            _winkSubsDescription.text = string.Format(_winkSubsDescriptionPattern, string.IsNullOrEmpty(trialResult) ? _defaultTrialPeriodDays : trialResult, string.IsNullOrEmpty(priceResult) ? _defaultWinkPrice : priceResult);

            Task<int> minutesTask = RemoteConfig.IntRemoteConfig(_defaultTimerGiftMinutesKey + Application.identifier, _defaultTimerGiftMinutes);
            yield return new WaitUntil(() => minutesTask.IsCompleted);

            _rewardMinutes = minutesTask.Result;
            _rewardButtonDiscription.text = string.Format(_rewardButtonDiscriptionPattern, _rewardMinutes);

            Debug.Log($"ADS PLUGIN: get reward remote, trialResult = {trialResult}, priceResult = {priceResult}, reward minutes = {_rewardMinutes}");

            Initialized = true;
        }

        public override void Enable()
        {
            _imagesCarousel.Enable();
            _rewardDemoTimeButton.onClick.AddListener(ShowReward);
            EnableCanvasGroup(_canvasGroup);

            if (AdvertisementController.Instance == null)
            {
                Debug.LogError("AdvertisementController not constructed!");
                return;
            }

            AdvertisementController.Instance.TryPreloadRewardAD(ActivateRewardButton);
        }

        public override void Disable()
        {
            DisableCanvasGroup(_canvasGroup);
            _rewardDemoTimeButton.onClick.RemoveListener(ShowReward);
            _imagesCarousel.Disable();
            DeactivateRewardButton();
        }

        public void SetRewardText(string text)
        {
            if (string.IsNullOrEmpty(text))
                _rewardButtonDiscription.gameObject.SetActive(false);
            else
                _rewardButtonDiscription.text = string.Format(text, _rewardMinutes);
        }

        private void ShowReward()
        {
            AdvertisementController.Instance.ShowReward(AddDemoTime, ReloadReward);
            DeactivateRewardButton();
        }

        private void AddDemoTime()
        {
            _demoTimer.AddDemoTime(_rewardMinutes * OneMinute);
            RewardSuccessed?.Invoke();
        }

        private void ActivateRewardButton()
        {
            _rewardDemoTimeButton.interactable = true;
            _rewardButtonLabel.color = _defaultTextColor;
            _rewardButtonDiscription.color = _defaultTextColor;
        }

        private void DeactivateRewardButton()
        {
            _rewardDemoTimeButton.interactable = false;
            _rewardButtonLabel.color = _blinkTextColor;
            _rewardButtonDiscription.color = _blinkTextColor;
        }

        private void ReloadReward()
        {
            Debug.Log("RELOAD");
            _reloadAd ??= StartCoroutine(ReloadAD());

            IEnumerator ReloadAD()
            {
                yield return new WaitForSeconds(_reloadAdDelay);

                AdvertisementController.Instance.TryPreloadRewardAD(ActivateRewardButton);
                _reloadAd = null;
            }
        }
    }
}
