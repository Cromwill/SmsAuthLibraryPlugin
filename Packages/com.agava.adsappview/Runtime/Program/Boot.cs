using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdsAppView.DTO;
using AdsAppView.Utility;
using KinDzaDzaGames.AdvertisementPlugin;
using Newtonsoft.Json;
using UnityEngine;

namespace AdsAppView.Program
{
    public class Boot : MonoBehaviour
    {
        private const float DelayWhileAuthPluginInit = 10f;
#if UNITY_WEBGL
        private const string Platform = "webgl";
#elif UNITY_STANDALONE
        private const string Platform = "standalone";
#elif UNITY_ANDROID
        private const string Platform = "Android";
#elif UNITY_IOS
        private const string Platform = "iOS";
#endif

        [SerializeField] private Links _links;
        [SerializeField] private ViewPresenterConfigs _viewPresenterConfigs;
        [Header("Web settings")]
        [Tooltip("Bund for plugin settings")]
        [SerializeField] private Utility.BuildVersionHolder _buildVersionHolder;
        [Tooltip("Server name remote data")]
        [SerializeField] private string _serverPath;
        [Tooltip("Assets settings")]
        [SerializeField] private bool _freeApp = true;
        [SerializeField] private bool _useAssetBundles = true;
        [SerializeField] private AssetsBundlesLoader _assetsBundlesLoader;
        [SerializeField] private GameObject _defaultAsset;
        [Tooltip("Content load settings")]
        [SerializeField] private bool _asyncLoadContent = false;
        [SerializeField] private LoadingBarPresenter _loadingBarPresenter;
        [SerializeField] private AppOrientation _appOrientation;
        [Header("Advertisement")]
        [SerializeField] private AdvertisementBoot _advertisementBoot;

#if UNITY_WEBGL
        [Header("WEBGL")]
        [SerializeField] private string _appId;
#endif

#if UNITY_ANDROID || UNITY_IOS
        private string _appId => Application.identifier;
#endif

        private AdsAppAPI _api;
        private AppData _appData;
        private PreloadService _preloadService;

        public bool Constructed { get; private set; }

        public static Boot Instance { get; private set; }

        private IEnumerator Start()
        {
            if (_buildVersionHolder == null)
                throw new NullReferenceException("[Boot] Build version holder is null!!!");

            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (_freeApp)
                yield return Construct(vip: false);
        }

        public IEnumerator Construct(bool vip, string supportLink = null)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
                yield return new WaitWhile(() => Application.internetReachability == NetworkReachability.NotReachable);

            _api = new(_serverPath, _appId);
            _appData = new() { app_id = _appId, store_id = _buildVersionHolder.StoreName.ToString(), platform = Platform };
            _preloadService = new(_api, _buildVersionHolder.BundleId, _freeApp, vip, _appData, _buildVersionHolder.StoreName);
            Debug.Log("#Boot# " + JsonConvert.SerializeObject(_appData));

            yield return _preloadService.Preparing();
            yield return SheetRemoteConfigs.Initialize(_api);
            _loadingBarPresenter.Construct(_appOrientation);

            if (_freeApp)
            {
                yield return _links.Initialize(_api, _advertisementBoot.AppName, _advertisementBoot.StoreName);

                _loadingBarPresenter.FiiRemoteTexts(Links.Instance.Support);
                _loadingBarPresenter.Activate();

                yield return _advertisementBoot.Construct(vip: false, _buildVersionHolder.BundleId, _buildVersionHolder.StoreName.ToString(), Application.identifier, Platform, Links.Instance.Privacy);
            }
            else
            {
                _loadingBarPresenter.FiiRemoteTexts(supportLink);
                _loadingBarPresenter.Activate();
            }

            yield return _viewPresenterConfigs.Initialize(_api);

            if (_freeApp == false)
            {
                if (_preloadService.IsPluginAvailable)
                    yield return Initialize(vip);
                else
                    Debug.Log("#Boot# Popup plugin disabled");
            }
            else
            {
                if (_advertisementBoot.IsPluginAvailable)
                {
                    if (_advertisementBoot.AdvertisementController.WaitConcernPolicy && _advertisementBoot.AdvertisementController.PolicyAccepted == false)
                        yield return new WaitUntil(() => _advertisementBoot.AdvertisementController.AgreementClosed);

                    _loadingBarPresenter.ForceLoad();
                    yield return new WaitUntil(() => _loadingBarPresenter.ForceLoaded);
                    _loadingBarPresenter.Deactivate();

                    AdvertisementController.Instance?.StartInterstitialTimer();
                }
                else if (_preloadService.IsPluginAvailable)
                {
                    yield return Initialize(vip);
                }
            }

            Constructed = true;
        }

        private IEnumerator Initialize(bool vip)
        {
            AnalyticsService.SendStartApp(_appId);
            GameObject created = null;
            Debug.Log("#Boot# Popup plugin enabled");

            if (_useAssetBundles)
            {
                Task<GameObject> task = _assetsBundlesLoader.GetPopupObject();

                yield return new WaitUntil(() => task.IsCompleted);
                GameObject bundlePopupPrefab = task.Result;

                if (bundlePopupPrefab != null)
                {
                    created = Instantiate(bundlePopupPrefab);
                    created.name = "AssetBundle-PopupManager";
                    Debug.Log("#Boot# Created popup: " + created.name);
                }

                _assetsBundlesLoader.Unload();
            }

            if (created == null)
            {
                created = Instantiate(_defaultAsset);
                created.name = "Default-PopupManager";
                Debug.Log("#Boot# Default-PopupManager Instantiated");
            }

            yield return created.GetComponent<PopupManager>().Construct(_appData, _freeApp, vip, _asyncLoadContent, _loadingBarPresenter);
            _loadingBarPresenter.Deactivate();
        }
    }

    public enum AppOrientation
    {
        Landscape,
        Portrait
    }
}
