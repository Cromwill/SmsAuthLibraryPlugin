using UnityEngine;
using Newtonsoft.Json;
using System.Collections;
using KinDzaDzaGames.AdvertisementPlugin.DTO;
using KinDzaDzaGames.AdvertisementPlugin.Utility;

#if YABBI_AD
using YabbiSDK.Api;
using SspnetSDK.Unfiled;
using YabbiSDK.ConsentManagerSDK.Api;
using SspnetSDK.ConsentManagerSDK.Unfiled;
#endif

namespace KinDzaDzaGames.AdvertisementPlugin
{
    public class YabbiInitializer : MonoBehaviour
#if YABBI_AD
        , IInitializationListener
#endif
    {
#if UNITY_WEBGL
        private const string Platform = "webgl";
#elif UNITY_STANDALONE
        private const string Platform = "standalone";
#elif UNITY_ANDROID
        private const string Platform = "Android";
#elif UNITY_IOS
        private const string Platform = "iOS";
#endif

        [SerializeField] private UserConsentScreen _userConsentScreen;
        [SerializeField] private ADSNavigationView _aDSNavigationView;
        [Tooltip("Server name remote data")]
        [SerializeField] private string _serverPath;
        [SerializeField] private Store _storeName;
        [SerializeField] private int _bundleId;

#if YABBI_AD
        ConsentManager _consentManager = new ConsentManager();
#endif
        private AdvertisementAPI _api;
        private AppData _appData;
        private PreloadService _preloadService;

#if UNITY_WEBGL
        [Header("WEBGL")]
        [SerializeField] private string _appId;
#endif

#if UNITY_ANDROID || UNITY_IOS
        private string _appId => Application.identifier;
#endif

        private IEnumerator Start()
        {
            yield return Construct(vip: false, _bundleId, _storeName.ToString(), _appId, Platform);
        }

        public IEnumerator Construct(bool vip, int bundleId, string storeName, string appId, string platform)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
                yield return new WaitWhile(() => Application.internetReachability == NetworkReachability.NotReachable);
#if YABBI_AD
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
#if YABBI_AD
                    _consentManager.SetListener(_userConsentScreen);
#endif
#if BUILD_DEBUG
                    _consentManager.EnableLog(true);
#endif
                    _consentManager.LoadManager();
                    _consentManager.ShowConsentWindow();
                }
            }
#endif

            _aDSNavigationView.Construct();

            _api = new(_serverPath, appId);
            _appData = new() { app_id = appId, store_id = storeName, platform = platform };
            _preloadService = new(_api, bundleId, vip, _appData);
            Debug.Log("#Boot# " + JsonConvert.SerializeObject(_appData));

            yield return _preloadService.Preparing();
        }

        public void OnInitializeSuccess()
        {
#if BUILD_DEBUG
            Debug.Log($"YABBI PLUGIN: initialization is successful!");
#endif
        }

#if YABBI_AD
        public void OnInitializeFailed(AdException error)
        {
#if BUILD_DEBUG
            Debug.Log($"YABBI PLUGIN: yabbi initialization failed... Error: {error.Description}");
#endif
        }
#endif
    }
}
