using UnityEngine;
using Newtonsoft.Json;
using System.Collections;
using UnityEngine.Scripting;
using System.Threading.Tasks;
using KinDzaDzaGames.AdvertisementPlugin.DTO;
using KinDzaDzaGames.AdvertisementPlugin.Utility;

#if UNITY_EDITOR
using KinDzaDzaGames.AdvertisementPlugin.Editor;
#endif

namespace KinDzaDzaGames.AdvertisementPlugin
{
    [Preserve]
    public class AdvertisementBoot : MonoBehaviour
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

        [SerializeField] private AdvertisementController _advertisementController;
        [Header("Advertisement configs")]
        [SerializeField] private AppName _appName;
        [SerializeField] private Store _storeName;
        [SerializeField
#if UNITY_EDITOR
            , ReadOnly
#endif
            ] private AdvertisingConfigs _advertisingConfigs;
        [Header("Remote reward data")]
        [SerializeField] private RewardSettings _rewardSettings;
        [Header("Server name remote data")]
        [SerializeField] private string _serverPath;
        [Header("Application")]
        [SerializeField] private int _bundleId;
        [SerializeField] private bool _selfInit = false;

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

        public AdvertisementController AdvertisementController => _advertisementController;
        public bool IsPluginAvailable => _preloadService.IsPluginAvailable;

        private void OnEnable()
        {
            _advertisementController.InitializationFailed += OnInitializationFailed;
        }

        private void Awake()
        {
            if(_selfInit)
                StartCoroutine(Construct(vip: false, _bundleId, _storeName.ToString(), _appId, Platform, _advertisingConfigs.AppPrivacyPolicyURL));
        }

        public IEnumerator Construct(bool vip, int bundleId, string storeName, string appId, string platform, string privacy)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
                yield return new WaitWhile(() => Application.internetReachability == NetworkReachability.NotReachable);

            _advertisingConfigs = AdvertisementID.GetConfig(_appName, _storeName);
            _advertisingConfigs.AppPrivacyPolicyURL = privacy;
            _api = new(_serverPath, appId);
            _appData = new() { app_id = appId, store_id = storeName, platform = platform };
            _preloadService = new(_api, bundleId, vip, _appData);
            Debug.Log("#Advertisement Boot# " + JsonConvert.SerializeObject(_appData));

            yield return _preloadService.Preparing();

            if(_preloadService.IsPluginAvailable)
            {
                yield return GetRewardRemote();

                _advertisementController.Construct(vip, _rewardSettings, _preloadService.Settings, _advertisingConfigs);

                yield return new WaitUntil(() => _advertisementController.Initialized || _advertisementController.Breaked);

                if (_advertisementController.Breaked)
                    Destroy(_advertisementController.gameObject);
            }

            Debug.Log($"Advertisement Plugin: constructed. Plugin available = {_preloadService.IsPluginAvailable}, breaked = {_advertisementController.Breaked}");
        }

        private IEnumerator GetRewardRemote()
        {
            Task<int> countTask = RemoteConfig.IntRemoteConfig(_rewardSettings.RewardCountKey, _rewardSettings.DefaultRewardCount);
            yield return new WaitUntil(() => countTask.IsCompleted);

            Task<bool> availableTask = RemoteConfig.BoolRemoteConfig(_rewardSettings.RewardAvailableKey, _rewardSettings.DefaultRewardAvailable);
            yield return new WaitUntil(() => availableTask.IsCompleted);

            _rewardSettings.Contruct(countTask.Result, availableTask.Result);
        }

        private void OnDisable()
        {
            _advertisementController.InitializationFailed -= OnInitializationFailed;
        }

        private void OnInitializationFailed()
        {

        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _advertisingConfigs = AdvertisementID.GetConfig(_appName, _storeName);
        }
#endif
    }
}
