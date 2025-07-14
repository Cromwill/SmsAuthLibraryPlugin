using TMPro;
using System;
using UnityEngine;
using System.Text;
using UnityEngine.UI;
using System.Collections;
using SmsAuthAPI.Program;
using Com.Yandex.Varioqub;
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
        [SerializeField, Min(0)] private int _defaultTimerGiftMinutes = 10;
        [Header("Reward button")]
        [SerializeField] private Button _rewardDemoTimeButton;
        [SerializeField] private TMP_Text _rewardButtonLabel;
        [SerializeField] private TMP_Text _rewardButtonDiscription;
        [SerializeField] private RewardSettings _rewardSettings;

        private Dictionary<int, char> _minutWordEndings = new Dictionary<int, char>
        { { 1, 'у' }, { 2, 'ы' }, { 3, 'ы' }, { 4, 'ы' }, { 21, 'у' }, { 22, 'ы' }, { 23, 'ы' }, { 24, 'ы' }, { 31, 'у' }, { 32, 'ы' }, { 33, 'ы' }, { 34, 'ы' } };

        private DemoTimer _demoTimer;
        private Color _defaultTextColor;
        private Color _blinkTextColor;
        private Coroutine _reloadAd;
        private AppMetricaInfo _appMetricaInfo;
        private int _fetchCount = 3;

        public bool Initialized { get; private set; } = false;

        public event Action RewardSuccessed;

        public IEnumerator Construct(DemoTimer demoTimer, string storeName, AppMetricaInfo appMetricaInfo)
        {
            _demoTimer = demoTimer ?? throw new ArgumentNullException(nameof(demoTimer));
            _appMetricaInfo = appMetricaInfo ?? throw new ArgumentNullException(nameof(appMetricaInfo));

            _defaultTextColor = _blinkTextColor = _rewardButtonLabel.color;
            _blinkTextColor.a = 0.5f;
            DeactivateRewardButton();

            if (string.IsNullOrEmpty(storeName))
                Debug.LogError("Incorrect store name received.");

            yield return new WaitUntil(() => SmsAuthApi.Initialized);

            Task<string> trialTask = RemoteConfig.StringRemoteConfig(_trialPeriodDaysKey, string.Empty);
            yield return new WaitUntil(() => trialTask.IsCompleted);

            Task<string> priceTask = RemoteConfig.StringRemoteConfig(_winkPriceKey, string.Empty);
            yield return new WaitUntil(() => priceTask.IsCompleted);

            _winkSubsDescription.text = string.Format(_winkSubsDescriptionPattern, string.IsNullOrEmpty(trialTask.Result) ? _defaultTrialPeriodDays : trialTask.Result, string.IsNullOrEmpty(priceTask.Result) ? _defaultWinkPrice : priceTask.Result);


            string id = $"appmetrica.{_appMetricaInfo.VarioqubId}";
            var settings = new VarioqubSettings(id);
            settings.Logs = true;
            settings.ThrottleInterval = 60;

            Varioqub.InitVarioqubWithAppMetricaAdapter(settings);
            Varioqub.ActivateConfig();

            yield return RepeatFetch();

            _minutWordEndings.TryGetValue(_rewardSettings.demo_overtime_minutes, out char ending);

            _rewardButtonLabel.text = _rewardSettings.ads_show_text;
            _rewardButtonDiscription.text = _rewardSettings.over_time_text.Replace($"{{{"n"}}}", _rewardSettings.demo_overtime_minutes.ToString()) + ending;
            _rewardButtonDiscription.gameObject.SetActive(_rewardSettings.over_time_bool);

            Debug.Log($"Advertisement Plugin: get reward remote, trialResult = {trialTask.Result}, priceResult = {priceTask.Result}, reward minutes = {_rewardSettings.demo_overtime_minutes}");

            Initialized = true;
        }

        private IEnumerator RepeatFetch()
        {
            bool success = false;
            int fetchCount = 0;
            yield return new WaitForSeconds(.5f);

            while (success == false)
            {
                Varioqub.Fetch(
                    onSuccessDelegate: () =>
                    {
                        success = true;
                        Debug.Log($"VARIOQUB: Fetch successed");

                        string demo_overtime_minutes = Varioqub.GetString("demo_overtime_minutes", $"{_rewardSettings.demo_overtime_minutes}");
                        string over_time_bool = Varioqub.GetString("over_time_bool", $"{_rewardSettings.over_time_bool}");
                        string over_time_text = Varioqub.GetString("over_time_text", _rewardSettings.over_time_text);
                        string ads_show_text = Varioqub.GetString("ads_show_text", _rewardSettings.ads_show_text);

                        _rewardSettings.SetSettings(demo_overtime_minutes, over_time_bool, over_time_text, ads_show_text);
                    },
                    onErrorDelegate: error =>
                    {
                        Debug.Log($"VARIOQUB: Fetch Error = {error}!");
                    }
                );

                if (success == false)
                {
                    fetchCount++;

                    if (fetchCount > _fetchCount)
                    {
                        Debug.Log($"VARIOCUB: Fetch breaked!");
                        success = true;
                    }
                    else
                    {
                        Debug.Log($"VARIOCUB: Fetch restarted!");
                        yield return new WaitForSeconds(2f);
                    }
                }
            }
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
        private const int DefaultRewardMinutes = 10;
        private const bool DefaultOvertimeText = true;
        private const string DefaultOverTimeText = "и играть ещё {n} минут";
        private const string DefaultAdsShowText = "Посмотреть рекламу";

        [field: SerializeField] public int demo_overtime_minutes { get; private set; } = DefaultRewardMinutes;
        [field: SerializeField] public bool over_time_bool { get; private set; } = DefaultOvertimeText;

        public string over_time_text { get; private set; } = DefaultOverTimeText;
        public string ads_show_text { get; private set; } = DefaultAdsShowText;

        internal void SetSettings(string demo_overtime_minutes, string over_time_bool, string over_time_text, string ads_show_text)
        {
            if (int.TryParse(demo_overtime_minutes, out int rewardMitutes))
                this.demo_overtime_minutes = rewardMitutes;

            if (bool.TryParse(over_time_bool, out bool overtimeText))
                this.over_time_bool = overtimeText;

            if (IsValidString(over_time_text))
            {
                this.over_time_text = over_time_text;
                Debug.Log($"Advertisement Plugin: VARIOQUB string ({over_time_text}) correct.");
            }
            else
            {
                this.over_time_text = DefaultOverTimeText;
                Debug.Log($"Advertisement Plugin: VARIOQUB the string ({over_time_text}) contains invalid characters.");
            }

            if (IsValidString(ads_show_text))
            {
                this.ads_show_text = ads_show_text;
                Debug.Log($"Advertisement Plugin: VARIOQUB string ({ads_show_text}) correct.");
            }
            else
            {
                this.ads_show_text = DefaultAdsShowText;
                Debug.Log($"Advertisement Plugin: VARIOQUB the string ({ads_show_text}) contains invalid characters.");
            }

            Debug.Log($"Advertisement Plugin: get varioqub settings, demo_overtime_minutes = {this.demo_overtime_minutes}, over_time_bool = {this.over_time_bool}, over_time_text = {this.over_time_text}, ads_show_text = {this.ads_show_text}");
        }

        internal bool IsValidString(string input)
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                string decodedString = Encoding.UTF8.GetString(bytes);
                return input == decodedString;
            }
            catch (Exception ex)
            {
                Debug.Log($"Advertisement Plugin: encoding exception = {ex}");
                return false;
            }
        }
    }
}
