using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Scripting;
using System.Collections.Generic;

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
        [SerializeField] private UserConsentScreen _userConsentScreen;
        [SerializeField] private PlaceOnScreen _standartPlace = PlaceOnScreen.BottomCenter;
        [SerializeField] private List<BannerPlace> _bannerPlaces;

        private bool _vip;
        private RewardHandler _rewardHandler;
        private InterstitialHandler _interstitialHandler;
        private BannerHandler _bannerHandler;
#if YABBI_AD
        ConsentManager _consentManager = new ConsentManager();
#endif

        public static AdvertisementController Instance { get; private set; }

        public RewardSettings RewardSettings { get; private set; }
        public bool Initialized { get; private set; } = false;
        public InterstitialHandler InterstitialHandler => _interstitialHandler;

        public event Action InitializationFailed;

        public void Construct(bool vip, RewardSettings rewardSettings)
        {
            _vip = vip;
            RewardSettings = rewardSettings;

            if (Instance == null)
                Instance = this;

            DontDestroyOnLoad(this);

#if UNITY_EDITOR
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
            _interstitialHandler?.Dispose();
            _rewardHandler?.Dispose();
            _bannerHandler?.Dispose();
        }

        public void OnInitializeSuccess()
        {
            Debug.Log("Advertisement Plugin: initialize success!");
            InitADListeners();
            Initialized = true;
        }

        public void ShowInterstitial(Action interstitialCloseAction = null) => _interstitialHandler.Show(interstitialCloseAction);
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
            _rewardHandler = new(RewardSettings);
            _interstitialHandler = new(this);
            _bannerHandler = new(this, switchADTime: 30, bannerCloseButtonVisibility: false, _standartPlace);
        }
    }
}
