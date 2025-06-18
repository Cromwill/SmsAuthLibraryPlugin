using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using UnityEngine.Scripting;
using System.Collections.Generic;
using UnityEngine.UIElements;

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

        private readonly int _switchADTime = 30;
        private readonly bool _bannerCloseButtonVisibility = false;
        private readonly ICoroutine _coroutine;

        private bool _vip;
        private bool _bannerDisplayed = false;
        private PlaceOnScreen _placeOnScreen = PlaceOnScreen.BottomCenter;
        private Coroutine _checkBannerBlockCoroutine = null;
        private Coroutine _displayBannerCoroutine = null;
        private List<IAdBlocker> _adBlockers = new List<IAdBlocker>();
        private bool _bannerLoaded = false;

#if YANDEX_AD
        private Banner _banner;
        private BannerAdSize _bannerSize;
#endif

        public event Action BannerDisplayed;
        public event Action BannerHided;

        public BannerHandler(ICoroutine coroutine, int switchADTime, bool bannerCloseButtonVisibility, PlaceOnScreen bannerPlace, bool vip)
        {
            _coroutine = coroutine ?? throw new ArgumentNullException(nameof(coroutine));
            _vip = vip;

            _switchADTime = switchADTime;
            _bannerCloseButtonVisibility = bannerCloseButtonVisibility;
            _placeOnScreen = bannerPlace;

#if UNITY_EDITOR &&  YABBI_AD == false
            Debug.Log("Advertisement Plugin: banner handler inited.");
# elif YABBI_AD
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
            if (_vip || _adBlockers.Count > 0 || _bannerDisplayed)
                return;

            if (_placeOnScreen != placeOnScreen)
            {
                _placeOnScreen = placeOnScreen;
                SetBannerSettings();
            }

            _displayBannerCoroutine ??= _coroutine.StartCoroutine(DisplayBanner());
        }

        public void Hide()
        {
            if (_bannerDisplayed)
            {
                DestroyAd();
                _bannerDisplayed = false;
                _displayBannerCoroutine = null;
                _checkBannerBlockCoroutine = null;
            }
        }

        public void SuspendBanner(IAdBlocker adBlocker)
        {
            if (_vip)
                return;

            _adBlockers.Add(adBlocker);

            if(_displayBannerCoroutine != null)
            {
                _coroutine.StopCoroutine(_displayBannerCoroutine);
                _displayBannerCoroutine = null;
            }

            if(_bannerDisplayed)
            {
                DestroyAd();
                _bannerDisplayed = false;
                _checkBannerBlockCoroutine = _coroutine.StartCoroutine(WaitDisplayPermission());
            }
        }

        public void ChangePosition(PlaceOnScreen bannerPlace, bool reloadBanner = false)
        {
            if (_vip || (_placeOnScreen == bannerPlace && reloadBanner == false))
                return;

            if (reloadBanner && _bannerDisplayed)
            {
                DestroyAd();
                _bannerDisplayed = false;
            }

            _placeOnScreen = bannerPlace;
            SetBannerSettings();

            if(reloadBanner)
                _checkBannerBlockCoroutine ??= _coroutine.StartCoroutine(WaitDisplayPermission());
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

            if (AdIsLoaded())
                DestroyAd();

            _bannerDisplayed = false;
        }

        private IEnumerator WaitDisplayPermission()
        {
            while (_adBlockers.Any(b => b.DisplayBlocked == true))
                yield return new WaitForSeconds(CheckBlockedDelay);

            _adBlockers.Clear();

            Show(_placeOnScreen);
            _checkBannerBlockCoroutine = null;
        }

        private IEnumerator DisplayBanner()
        {
            while (CanLoadAd() == false)
                yield return new WaitForSeconds(RetryLoadAdDelay);

            LoadAd();

            while (AdIsLoaded() == false)
                yield return new WaitForSeconds(RetryLoadAdDelay);

            ShowAd();
            _displayBannerCoroutine = null;
        }

        private
#if UNITY_EDITOR &&  YABBI_AD == false
        PlaceOnScreen
#elif YABBI_AD
        int
#elif YANDEX_AD
        PlaceOnScreen
#endif
        DeterminePosition() => _placeOnScreen switch
        {
#if UNITY_EDITOR &&  YABBI_AD == false
            PlaceOnScreen.TopCenter => PlaceOnScreen.TopCenter,
            _ => PlaceOnScreen.BottomCenter,
#elif YABBI_AD
            PlaceOnScreen.TopLeft or PlaceOnScreen.TopCenter or PlaceOnScreen.TopRight => BannerPosition.TOP,
            _ => BannerPosition.BOTTOM,
#elif YANDEX_AD
                PlaceOnScreen.TopLeft => AdPosition.TopLeft,
                PlaceOnScreen.TopCenter => AdPosition.TopCenter,
                PlaceOnScreen.TopRight => AdPosition.TopRight,
                PlaceOnScreen.CenterLeft => AdPosition.CenterLeft,
                PlaceOnScreen.Center => AdPosition.Center,
                PlaceOnScreen.CenterRight => AdPosition.CenterRight,
                PlaceOnScreen.BottomLeft => AdPosition.BottomLeft,
                PlaceOnScreen.BottomRight => AdPosition.BottomRight,
                _ => AdPosition.BottomCenter,
#endif
        };

        private void SetBannerSettings()
        {
#if UNITY_EDITOR &&  YABBI_AD == false
            Debug.Log("Advertisement Plugin: banner settings setted.");
#elif YABBI_AD
            Yabbi.SetBannerCustomSettings(new BannerSettings().SetRefreshIntervalSeconds(_switchADTime).SetShowCloseButton(_bannerCloseButtonVisibility).SetBannerPosition(DeterminePosition()));
#elif YANDEX_AD
            _bannerSize = BannerAdSize.InlineSize((int)_widthSlider.value, (int)_heightSlider.value);
#endif
        }

        protected override string GetPlacementName()
        {
#if UNITY_EDITOR &&  YABBI_AD == false
            return AdvertisingSettings.EditorTest.Test;
#elif YABBI_AD
            return AdvertisingSettings.YabbiAds.yabbiBannerUnitID;
#elif YANDEX_AD
            return AdvertisingSettings.YandexAds.Release.BannerUnitId;
#endif
        }

        protected override bool CanLoadAd()
        {
#if UNITY_EDITOR &&  YABBI_AD == false
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
            _banner = new Banner(AdvertisementSettings.BannerUnitId, _bannerSize, _bannerPosition);

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
#if UNITY_EDITOR &&  YABBI_AD == false
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
            BannerHided?.Invoke();
        }

        #region YABBI_AD
#if YABBI_AD
        public void OnBannerLoaded(AdPayload adPayload) { }
        public void OnBannerLoadFailed(AdPayload adPayload, AdException error) { }
        public void OnBannerShown(AdPayload adPayload) { }
        public void OnBannerShowFailed(AdPayload adPayload, AdException error) { }
        public void OnBannerClosed(AdPayload adPayload) { }
        public void OnBannerImpression(AdPayload adPayload)
        {
            _bannerDisplayed = true;
            BannerDisplayed?.Invoke();
        }

        private int GetAdType() => Yabbi.Banner;
#endif
        #endregion

        #region YANDEX_AD
#if YANDEX_AD
        private AdRequest CreateAdRequest() => new AdRequest.Builder().Build();
        private void HandleAdLoaded(object sender, EventArgs args) => _bannerLoaded = true;
        private void HandleAdFailedToLoad(object sender, AdFailureEventArgs args) { }
        private void HandleLeftApplication(object sender, EventArgs args) { }
        private void HandleReturnedToApplication(object sender, EventArgs args) { }
        private void HandleAdLeftApplication(object sender, EventArgs args) { }
        private void HandleAdClicked(object sender, EventArgs args) { }

        private void HandleImpression(object sender, ImpressionData impressionData)
        {
            _bannerDisplayed = true;
            BannerDisplayed?.Invoke();
        }
#endif
        #endregion
    }
}
