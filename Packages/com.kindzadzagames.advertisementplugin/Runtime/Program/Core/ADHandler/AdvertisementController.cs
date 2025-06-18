using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Scripting;
using System.Collections.Generic;
using KinDzaDzaGames.AdvertisementPlugin.DTO;

#if YABBI_AD
using YabbiSDK.Api;
using SspnetSDK.Unfiled;
using YabbiSDK.ConsentManagerSDK.Api;
using SspnetSDK.ConsentManagerSDK.Unfiled;
#endif

namespace KinDzaDzaGames.AdvertisementPlugin
{
    [Preserve]
    public class AdvertisementController : MonoBehaviour, ICoroutine
#if YABBI_AD
        , IInitializationListener
#endif
    {
        [SerializeField] private InterstitialPlayer _interstitialPlayer;
        [SerializeField] private UserConsentScreen _userConsentScreen;
        [SerializeField] private PlaceOnScreen _standartPlace = PlaceOnScreen.BottomCenter;
        [SerializeField] private List<BannerPlace> _bannerPlaces;

        private bool _vip;
        private RewardHandler _rewardHandler;
        private InterstitialHandler _interstitialHandler;
        private BannerHandler _bannerHandler;
        private RewardSettings _rewardSettings;
        private AdsSdkSettingsData _settings;
#if YABBI_AD
        ConsentManager _consentManager = new ConsentManager();
#endif

        public static AdvertisementController Instance { get; private set; }

        public bool Initialized { get; private set; } = false;

        public event Action InitializationFailed;
        public event Action BannerDisplayed;
        public event Action BannerHided;

        public void Construct(bool vip, RewardSettings rewardSettings, AdsSdkSettingsData settings)
        {
            _vip = vip;
            _rewardSettings = rewardSettings ?? throw new ArgumentNullException(nameof(rewardSettings));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));

            if (Instance == null)
                Instance = this;

            DontDestroyOnLoad(this);

#if UNITY_EDITOR && YABBI_AD == false
            OnInitializeSuccess();
#elif YABBI_AD
            StartCoroutine(StartYabbiService());
#elif YANDEX_AD
            Debug.Log("Advertisement Plugin: start YANDEX service.");
            MobileAds.SetAgeRestrictedUser(true);
            OnInitializeSuccess();
#endif
        }

        private void OnDestroy()
        {
            if(Initialized)
            {
                _interstitialHandler.Dispose();
                _rewardHandler.Dispose();
                _bannerHandler.Dispose();
                _interstitialPlayer.Dispose();

                _bannerHandler.BannerDisplayed -= OnBannerDisplayed;
                _bannerHandler.BannerHided -= OnBannerHided;
            }
        }

        private void OnApplicationFocus(bool focus)
        {
            if (Initialized)
            {
                _interstitialHandler.ChangeFocusState(focus);
                _rewardHandler.ChangeFocusState(focus);
            }
        }

        public void OnInitializeSuccess()
        {
            Debug.Log("Advertisement Plugin: initialize success!");
            InitADListeners();
            _interstitialPlayer.Construct(_interstitialHandler, _settings, _vip);
            Initialized = true;

            _bannerHandler.BannerDisplayed += OnBannerDisplayed;
            _bannerHandler.BannerHided += OnBannerHided;
        }

        public void ChangeSubscribeStatus(bool vip)
        {
            _vip = vip;
            _interstitialPlayer.ChangeSubscribeStatus(vip);
            _bannerHandler.ChangeSubscribeStatus(vip);

            if(_vip)
                _interstitialHandler.DropAd();
        }

        public void StartInterstitialTimer() => _interstitialPlayer.StartTimer();
        public void AddInterstitialBlocker(IAdBlocker adBlocker) => _interstitialHandler.AddBlocker(adBlocker);

        public bool CanShowReward() => _rewardHandler.CanShow();
        public void TryPreloadRewardAD(Action preRewardAction = null) => _rewardHandler.LoadAD(preRewardAction);
        public void ShowReward(Action rewardSuccessAction = null, Action rewardFailureAction = null) => _rewardHandler.Show(rewardSuccessAction, rewardFailureAction);

        public void ShowBanner(PlaceOnScreen placeOnScreen = PlaceOnScreen.BottomCenter) => _bannerHandler.Show(placeOnScreen);
        public void HideBanner() => _bannerHandler.Hide();
        public void SuspendDisplayBanner(IAdBlocker adBlocker) => _bannerHandler.SuspendBanner(adBlocker);
        public void ChangeBannerPosition(PlaceOnScreen placeOnScreen, bool reloadBanner = false) => _bannerHandler.ChangePosition(placeOnScreen, reloadBanner);

#if YABBI_AD
        public void OnInitializeFailed(AdException error) => InitializationFailed?.Invoke();

        private IEnumerator StartYabbiService()
        {
            Debug.Log("Advertisement Plugin: start YABBI service.");
#if BUILD_DEBUG
            Yabbi.EnableDebug(true);
#endif
            if(Yabbi.IsInitialized() == false)
                Yabbi.Initialize(AdvertisingSettings.YabbiAds.publisherID, this);

            yield return new WaitUntil(() => Yabbi.IsInitialized());

            if (_userConsentScreen.NeedShowConsentScreen)
            {
                var builder = new ConsentBuilder()
                .AppendPolicyURL(AdvertisingSettings.YabbiAds.PrivacyPolicyURL)
                .AppendGdpr(true);
                _consentManager.RegisterCustomVendor(builder);

                if (_userConsentScreen.AgreementAccepted == false)
                {
                    _consentManager.SetListener(_userConsentScreen);
#if BUILD_DEBUG
                    _consentManager.EnableLog(true);
#endif
                    _consentManager.LoadManager();
                    _consentManager.ShowConsentWindow();
                }
            }
        }
#endif

        private void InitADListeners()
        {
            _rewardHandler = new(_rewardSettings);
            _interstitialHandler = new(this);
            _bannerHandler = new(this, switchADTime: 30, bannerCloseButtonVisibility: false, _standartPlace, _vip);
        }

        private void OnBannerDisplayed() => BannerDisplayed?.Invoke();
        private void OnBannerHided() => BannerHided?.Invoke();
    }
}
