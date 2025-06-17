using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SmsAuthAPI.Program;
using UnityEngine.Scripting;
using System.Threading.Tasks;
using KinDzaDzaGames.AdvertisementPlugin;
using System.Collections.Generic;

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
        [SerializeField] private string _defaultWinkPrice = "Далее 199 р/месяц";
        //[SerializeField] private string _defaultTimerGiftMinutesKey = "demo-overtime-minutes_";
        [SerializeField, Min(0)] private int _defaultTimerGiftMinutes = 10;
        [Header("Reward button")]
        [SerializeField] private Button _rewardDemoTimeButton;
        [SerializeField] private TMP_Text _rewardButtonLabel;
        [SerializeField] private TMP_Text _rewardButtonDiscription;
        [SerializeField] private RewardSettings _rewardSettings;

        private Dictionary<int, char> _minutWordEndings = new Dictionary<int, char>
        { { 1, 'а' }, { 2, 'ы' }, { 3, 'ы' }, { 4, 'ы' }, { 21, 'а' }, { 22, 'ы' }, { 23, 'ы' }, { 24, 'ы' }, { 31, 'а' }, { 32, 'ы' }, { 33, 'ы' }, { 34, 'ы' } };

        private DemoTimer _demoTimer;
        private Color _defaultTextColor;
        private Color _blinkTextColor;
        private Coroutine _reloadAd;
        //private int _rewardMinutes = 10;

        public bool Initialized { get; private set; } = false;

        public event Action RewardSuccessed;

        public IEnumerator Construct(DemoTimer demoTimer, string storeName)
        {
            _demoTimer = demoTimer ?? throw new ArgumentNullException(nameof(demoTimer));

            _defaultTextColor = _blinkTextColor = _rewardButtonLabel.color;
            _blinkTextColor.a = 0.5f;
            DeactivateRewardButton();

            if (string.IsNullOrEmpty(storeName))
                Debug.LogError("Incorrect store name received.");

            yield return new WaitUntil(() => SmsAuthApi.Initialized);

            //string remoteRewardTimeKey = _defaultTimerGiftMinutesKey + Application.identifier + $"_{storeName}";

            Task<string> trialTask = RemoteConfig.StringRemoteConfig(_trialPeriodDaysKey, string.Empty);
            yield return new WaitUntil(() => trialTask.IsCompleted);

            Task<string> priceTask = RemoteConfig.StringRemoteConfig(_winkPriceKey, string.Empty);
            yield return new WaitUntil(() => priceTask.IsCompleted);

            /*Task<int> minutesTask = RemoteConfig.IntRemoteConfig(remoteRewardTimeKey, _defaultTimerGiftMinutes);
            yield return new WaitUntil(() => minutesTask.IsCompleted);*/

            _winkSubsDescription.text = string.Format(_winkSubsDescriptionPattern, string.IsNullOrEmpty(trialTask.Result) ? _defaultTrialPeriodDays : trialTask.Result, string.IsNullOrEmpty(priceTask.Result) ? _defaultWinkPrice : priceTask.Result);

            //_rewardMinutes = minutesTask.Result;
            _minutWordEndings.TryGetValue(_rewardSettings.demo_overtime_minutes, out char ending);

            //_rewardButtonDiscription.text = string.Format(_rewardButtonDiscriptionPattern + ending, _rewardSettings.demo_overtime_minutes);
            _rewardButtonLabel.text = _rewardSettings.ads_show_text;
            _rewardButtonDiscription.text = _rewardSettings.over_time_text.Replace($"{{{"n"}}}", _rewardSettings.demo_overtime_minutes.ToString()) + ending;
            _rewardButtonDiscription.gameObject.SetActive(_rewardSettings.over_time_bool);

            //Debug.Log($"Advertisement Plugin: remote reward time key = {remoteRewardTimeKey}");
            Debug.Log($"Advertisement Plugin: get reward remote, trialResult = {trialTask.Result}, priceResult = {priceTask.Result}, reward minutes = {_rewardSettings.demo_overtime_minutes}");

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
                _rewardButtonDiscription.text = string.Format(text, _rewardSettings.demo_overtime_minutes);
        }

        private void ShowReward()
        {
            AdvertisementController.Instance.ShowReward(AddDemoTime, ReloadReward);
            DeactivateRewardButton();
        }

        private void AddDemoTime()
        {
            _demoTimer.AddDemoTime(_rewardSettings.demo_overtime_minutes * OneMinute);
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
            _reloadAd ??= StartCoroutine(ReloadAD());

            IEnumerator ReloadAD()
            {
                yield return new WaitForSeconds(_reloadAdDelay);

                AdvertisementController.Instance.TryPreloadRewardAD(ActivateRewardButton);
                _reloadAd = null;
            }
        }
    }

    [Preserve, Serializable]
    internal class RewardSettings
    {
        [field: SerializeField] public bool over_time_bool { get; private set; } = true;
        [field: SerializeField] public string over_time_text { get; private set; } = "и играть ещё {n} минут";
        [field: SerializeField] public string ads_show_text { get; private set; } = "Посмотреть рекламу";
        [field: SerializeField] public int demo_overtime_minutes { get; private set; } = 10;
    }
}
