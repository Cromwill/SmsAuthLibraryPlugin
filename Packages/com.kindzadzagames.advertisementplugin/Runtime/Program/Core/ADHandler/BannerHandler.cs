using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using UnityEngine.Scripting;
using System.Collections.Generic;

#if YABBI_AD
using YabbiSDK.Api;
using SspnetSDK.Unfiled;
#elif YANDEX_AD
using YandexMobileAds;
using YandexMobileAds.Base;
#endif

namespace KinDzaDzaGames.AdvertisementPlugin
{
    [Preserve]
    public class BannerHandler : AdvertisementHandler
#if YABBI_AD
        , IBannerAdListener
#endif
    {
        private const float RetryLoadAdDelay = 1f;
        private const float CheckBlockedDelay = 5f;
        private const int BannerWidth = 320;
        private const int BannerHeight = 50;

        private readonly int _switchADTime = 30;
        private readonly bool _bannerCloseButtonVisibility = false;
        private readonly AdvertisingConfigs _advertisingConfigs;
        private readonly ICoroutine _coroutine;

        private bool _vip;
        private bool _bannerShown = false;
        private bool _bannerSuspended = false;
        private PlaceOnScreen _placeOnScreen = PlaceOnScreen.BottomCenter;
        private Coroutine _checkBannerBlockCoroutine = null;
        private Coroutine _displayBannerCoroutine = null;
        private Coroutine _reloadCoroutine = null;
        private List<IBannerBlocker> _adBlockers = new List<IBannerBlocker>();
        private bool _bannerLoaded = false;
        private bool _bannerHidden = false;

        private bool _cannotShow => _adBlockers.Any(b => b.BannerDisplayBlocked == true) || _bannerHidden || _bannerSuspended;

#if YANDEX_AD
        private Banner _banner;
        private BannerAdSize _bannerSize;
#endif

        public bool BannerShown => _bannerShown;

        public event Action BannerDisplayed;
        public event Action BannerHided;

        public BannerHandler(AdvertisingConfigs advertisingConfigs, ICoroutine coroutine, int switchADTime, bool bannerCloseButtonVisibility, PlaceOnScreen bannerPlace, bool vip)
        {
            _advertisingConfigs = advertisingConfigs ?? throw new ArgumentNullException(nameof(advertisingConfigs));
            _coroutine = coroutine ?? throw new ArgumentNullException(nameof(coroutine));
            _vip = vip;

            _switchADTime = switchADTime;
            _bannerCloseButtonVisibility = bannerCloseButtonVisibility;
            _placeOnScreen = bannerPlace;

#if UNITY_EDITOR && YABBI_AD == false && YANDEX_AD == false
            Debug.Log("Advertisement Plugin: banner handler inited.");
#elif YABBI_AD
            Yabbi.SetBannerCallbacks(this);
#endif
            SetBannerSettings();
        }

        public void Dispose()
        {
            DropAd();
        }

        public void ChangeSubscribeStatus(bool vip)
        {
            _vip = vip;

            if (_vip)
                DropAd();
        }

        public void Show(PlaceOnScreen placeOnScreen)
        {
            AdvertisementAnalyticsService.SendAdsShowRequested(AdvertisementAnalyticsService.AdsType.Banner);

            if (_vip)
                return;

            if (_placeOnScreen != placeOnScreen)
            {
                _placeOnScreen = placeOnScreen;
                SetBannerSettings();

                DestroyAd();

                if (_displayBannerCoroutine != null)
                {
                    _coroutine.StopCoroutine(_displayBannerCoroutine);
                    _displayBannerCoroutine = null;
                }

                if (_checkBannerBlockCoroutine != null)
                {
                    _coroutine.StopCoroutine(_checkBannerBlockCoroutine);
                    _checkBannerBlockCoroutine = null;
                }

                _bannerSuspended = false;
                _bannerHidden = false;
            }

            if (_bannerShown || _bannerSuspended)
                return;

            _displayBannerCoroutine ??= _coroutine.StartCoroutine(DisplayBanner());
        }

        public void Hide()
        {
            DestroyAd();

            if (_displayBannerCoroutine != null)
            {
                _coroutine.StopCoroutine(_displayBannerCoroutine);
                _displayBannerCoroutine = null;
            }

            if (_checkBannerBlockCoroutine != null)
            {
                _coroutine.StopCoroutine(_checkBannerBlockCoroutine);
                _checkBannerBlockCoroutine = null;
            }

            if (_reloadCoroutine != null)
            {
                _coroutine.StopCoroutine(_reloadCoroutine);
                _reloadCoroutine = null;
            }

            _bannerSuspended = false;
            _bannerHidden = true;
        }

        public void SuspendBanner(IBannerBlocker adBlocker)
        {
            if (_vip)
                return;

            _adBlockers.Add(adBlocker);
            _bannerHidden = false;
            _bannerSuspended = true;

            if (_displayBannerCoroutine != null)
            {
                _coroutine.StopCoroutine(_displayBannerCoroutine);
                _displayBannerCoroutine = null;
            }

            if (_reloadCoroutine != null)
            {
                _coroutine.StopCoroutine(_reloadCoroutine);
                _reloadCoroutine = null;
            }

            if (_bannerShown)
            {
                DestroyAd();
                _checkBannerBlockCoroutine ??= _coroutine.StartCoroutine(WaitDisplayPermission());
            }
        }

        private void DropAd()
        {
            if (_checkBannerBlockCoroutine != null)
            {
                _coroutine.StopCoroutine(_checkBannerBlockCoroutine);
                _checkBannerBlockCoroutine = null;
            }

            if (_displayBannerCoroutine != null)
            {
                _coroutine.StopCoroutine(_displayBannerCoroutine);
                _displayBannerCoroutine = null;
            }

            if (_reloadCoroutine != null)
            {
                _coroutine.StopCoroutine(_reloadCoroutine);
                _reloadCoroutine = null;
            }

            if (AdIsLoaded())
                DestroyAd();
        }

        private IEnumerator WaitDisplayPermission()
        {
            while (_adBlockers.Any(b => b.BannerDisplayBlocked == true))
                yield return new WaitForSeconds(CheckBlockedDelay);

            _adBlockers.Clear();
            _bannerSuspended = false;
            _bannerHidden = false;

            Show(_placeOnScreen);
            _checkBannerBlockCoroutine = null;
        }

        private IEnumerator DisplayBanner()
        {
            while (CanLoadAd() == false)
                yield return new WaitForSeconds(RetryLoadAdDelay);
#if YABBI_AD
            LoadAd();

            while (AdIsLoaded() == false)
                yield return new WaitForSeconds(RetryLoadAdDelay);

            while (_adBlockers.Any(b => b.BannerDisplayBlocked == true))
                yield return new WaitForSeconds(CheckBlockedDelay);

            _adBlockers.Clear();
            _bannerSuspended = false;
            _bannerHidden = false;

            ShowAd();
            _displayBannerCoroutine = null;
#elif YANDEX_AD
            while (_adBlockers.Any(b => b.BannerDisplayBlocked == true))
                yield return new WaitForSeconds(CheckBlockedDelay);

            _adBlockers.Clear();
            _bannerSuspended = false;
            _bannerHidden = false;

            LoadAd();

#if UNITY_IOS
            while (AdIsLoaded() == false)
                yield return new WaitForSeconds(RetryLoadAdDelay);

            ShowAd();
#endif

            _displayBannerCoroutine = null;
#endif
        }

        private
#if UNITY_EDITOR && YABBI_AD == false && YANDEX_AD == false
        PlaceOnScreen
#elif YABBI_AD
        int
#elif YANDEX_AD
        AdPosition
#endif
        DeterminePosition() => _placeOnScreen switch
        {
#if UNITY_EDITOR && YABBI_AD == false && YANDEX_AD == false
            PlaceOnScreen.TopCenter => PlaceOnScreen.TopCenter,
            _ => PlaceOnScreen.BottomCenter,
#elif YABBI_AD
            PlaceOnScreen.TopLeft => BannerPosition.TOP_LEFT,
            PlaceOnScreen.TopCenter => BannerPosition.TOP_CENTER,
            PlaceOnScreen.TopRight => BannerPosition.TOP_RIGHT,
            PlaceOnScreen.BottomLeft => BannerPosition.BOTTOM_LEFT,
            PlaceOnScreen.BottomRight => BannerPosition.BOTTOM_RIGHT,
            _ => BannerPosition.BOTTOM_CENTER,
#elif YANDEX_AD

            PlaceOnScreen.TopLeft or PlaceOnScreen.TopCenter or PlaceOnScreen.TopRight => AdPosition.TopCenter,
            _ => AdPosition.BottomCenter,

                /*PlaceOnScreen.TopLeft => AdPosition.TopLeft,
                PlaceOnScreen.TopCenter => AdPosition.TopCenter,
                PlaceOnScreen.TopRight => AdPosition.TopRight,
                PlaceOnScreen.CenterLeft => AdPosition.CenterLeft,
                PlaceOnScreen.Center => AdPosition.Center,
                PlaceOnScreen.CenterRight => AdPosition.CenterRight,
                PlaceOnScreen.BottomLeft => AdPosition.BottomLeft,
                PlaceOnScreen.BottomRight => AdPosition.BottomRight,
                _ => AdPosition.BottomCenter,*/
#endif
        };

        private void SetBannerSettings()
        {
#if UNITY_EDITOR && YABBI_AD == false && YANDEX_AD == false
            Debug.Log("Advertisement Plugin: banner settings setted.");
#elif YABBI_AD
            Yabbi.SetBannerCustomSettings(new BannerSettings().SetRefreshIntervalSeconds(_switchADTime).SetShowCloseButton(_bannerCloseButtonVisibility).SetBannerPosition(DeterminePosition()));
#elif YANDEX_AD
            _bannerSize = BannerAdSize.InlineSize(BannerWidth, BannerHeight);
#endif
        }

        private IEnumerator ReloadAd()
        {
            yield return new WaitForSeconds(CheckBlockedDelay);

            if (_displayBannerCoroutine != null)
            {
                _coroutine.StopCoroutine(_displayBannerCoroutine);
                _displayBannerCoroutine = null;
            }

            Show(_placeOnScreen);
            _reloadCoroutine = null;
        }

        protected override string GetPlacementName()
        {
#if UNITY_EDITOR && YABBI_AD == false && YANDEX_AD == false
            return AdvertisingSettings.EditorTest.Test;
#elif YABBI_AD
            return _advertisingConfigs.BannerUnitID;
#elif YANDEX_AD
            return _advertisingConfigs.BannerUnitID;
#endif
        }

        protected override bool CanLoadAd()
        {
#if UNITY_EDITOR && YABBI_AD == false && YANDEX_AD == false
            return true;
#elif YABBI_AD
            return Yabbi.CanLoadAd(GetAdType(), GetPlacementName());
#elif YANDEX_AD
            return _banner == null;
#endif
        }

        protected override void LoadAd()
        {
#if YABBI_AD
            Yabbi.LoadAd(GetAdType(), GetPlacementName());
#elif YANDEX_AD
            _banner = new Banner(GetPlacementName(), _bannerSize, DeterminePosition());

            _banner.OnAdLoaded += HandleAdLoaded;
            _banner.OnAdFailedToLoad += HandleAdFailedToLoad;
            _banner.OnReturnedToApplication += HandleReturnedToApplication;
            _banner.OnLeftApplication += HandleLeftApplication;
            _banner.OnAdClicked += HandleAdClicked;
            _banner.OnImpression += HandleImpression;

            _banner.LoadAd(CreateAdRequest());
#endif
        }

        protected override bool AdIsLoaded()
        {
#if UNITY_EDITOR && YABBI_AD == false && YANDEX_AD == false
            return true;
#elif YABBI_AD
            return Yabbi.IsAdLoaded(GetAdType(), GetPlacementName());
#elif YANDEX_AD
            return _bannerLoaded;
#endif
        }

        protected override void ShowAd()
        {
#if YABBI_AD
            Yabbi.ShowAd(GetAdType(), GetPlacementName());
#elif YANDEX_AD
           _banner.Show();
#endif
        }

        protected override void DestroyAd()
        {
#if YABBI_AD
            Yabbi.DestroyAd(GetAdType(), GetPlacementName());
#elif YANDEX_AD
            if (_banner != null)
            {
                _banner.OnAdLoaded -= HandleAdLoaded;
                _banner.OnAdFailedToLoad -= HandleAdFailedToLoad;
                _banner.OnReturnedToApplication -= HandleReturnedToApplication;
                _banner.OnLeftApplication -= HandleLeftApplication;
                _banner.OnAdClicked -= HandleAdClicked;
                _banner.OnImpression -= HandleImpression;
                _banner.Destroy();
                _banner = null;
                _bannerLoaded = false;
            }
#endif
            _bannerShown = false;
            BannerHided?.Invoke();
        }

        #region YABBI_AD
#if YABBI_AD
        public void OnBannerLoaded(AdPayload adPayload)
        {
            AdvertisementAnalyticsService.SendAdsLoadSuccess(AdvertisementAnalyticsService.AdsType.Banner);
        }

        public void OnBannerLoadFailed(AdPayload adPayload, AdException error)
        {
            AdvertisementAnalyticsService.SendAdsLoadFailed(AdvertisementAnalyticsService.AdsType.Banner);
        }

        public void OnBannerShown(AdPayload adPayload)
        {
            if (_cannotShow)
            {
                DropAd();
            }
            else
            {
                AdvertisementAnalyticsService.SendAdsShowSuccess(AdvertisementAnalyticsService.AdsType.Banner);
            }
        }

        public void OnBannerShowFailed(AdPayload adPayload, AdException error)
        {
            AdvertisementAnalyticsService.SendAdsShowFailed(AdvertisementAnalyticsService.AdsType.Banner);
        }

        public void OnBannerClosed(AdPayload adPayload)
        {
            AdvertisementAnalyticsService.SendAdsClosed(AdvertisementAnalyticsService.AdsType.Banner);
        }

        public void OnBannerImpression(AdPayload adPayload)
        {
            if (_cannotShow)
            {
                DropAd();
            }
            else
            {
                AdvertisementAnalyticsService.SendAdsImpression(AdvertisementAnalyticsService.AdsType.Banner);
                _bannerShown = true;
                BannerDisplayed?.Invoke();
            }
        }

        private int GetAdType() => Yabbi.Banner;
#endif
        #endregion

        #region YANDEX_AD
#if YANDEX_AD
        private AdRequest CreateAdRequest() => new AdRequest.Builder().Build();
        private void HandleAdLoaded(object sender, EventArgs args) => _bannerLoaded = true;
        private void HandleAdFailedToLoad(object sender, AdFailureEventArgs args) => _reloadCoroutine ??= _coroutine.StartCoroutine(ReloadAd());
        private void HandleLeftApplication(object sender, EventArgs args) { }
        private void HandleReturnedToApplication(object sender, EventArgs args) { }
        private void HandleAdLeftApplication(object sender, EventArgs args) { }
        private void HandleAdClicked(object sender, EventArgs args) { }

        private void HandleImpression(object sender, ImpressionData impressionData)
        {
            if (_cannotShow)
            {
                DropAd();
            }
            else
            {
                AdvertisementAnalyticsService.SendAdsImpression(AdvertisementAnalyticsService.AdsType.Banner);
                _bannerShown = true;
                BannerDisplayed?.Invoke();
            }
        }
#endif
        #endregion
    }
}
