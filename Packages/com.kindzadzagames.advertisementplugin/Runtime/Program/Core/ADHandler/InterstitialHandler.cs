using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using UnityEngine.Scripting;
using System.Collections.Generic;
using TMPro;

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
    public class InterstitialHandler : AdvertisementHandler
#if YABBI_AD
        , IInterstitialAdListener
#endif
    {
        private const float RetryLoadAdDelay = 1f;
        private const float CheckBlockedDelay = 5f;

        private readonly AdvertisingConfigs _advertisingConfigs;
        private readonly ICoroutine _coroutine;

        private Coroutine _preloadCoroutine = null;
        private Coroutine _reloadCoroutine = null;
        private Coroutine _showCoroutine = null;
        private Action _interstitialCloseAction;
        private List<IInterstitialBlocker> _adBlockers = new List<IInterstitialBlocker>();
        private bool _AdShown = false;

#if YANDEX_AD
        private InterstitialAdLoader _interstitialAdLoader;
        private Interstitial _interstitial;
#endif

        public event Action InterstitialClosed;

        public InterstitialHandler(AdvertisingConfigs advertisingConfigs, ICoroutine coroutine)
        {
            _advertisingConfigs = advertisingConfigs ?? throw new ArgumentNullException(nameof(advertisingConfigs));
            _coroutine = coroutine ?? throw new ArgumentNullException(nameof(coroutine));

#if YABBI_AD == false && YANDEX_AD == false
            Debug.Log("Advertisement Plugin: interstitial handler inited.");
# elif YABBI_AD
            Yabbi.SetInterstitialCallbacks(this);
#elif YANDEX_AD
            _interstitialAdLoader = new InterstitialAdLoader();
            _interstitialAdLoader.OnAdLoaded += HandleAdLoaded;
            _interstitialAdLoader.OnAdFailedToLoad += HandleAdFailedToLoad;
#endif
        }

        public void Dispose()
        {
#if YANDEX_AD          
            _interstitialAdLoader.OnAdLoaded -= HandleAdLoaded;
            _interstitialAdLoader.OnAdFailedToLoad -= HandleAdFailedToLoad;
#endif
            DropAd();
        }

        public void ChangeFocusState(bool focus)
        {
            if (focus && _AdShown)
            {
                DestroyAd();
                ReportClosure();
            }
        }

        public void Show(Action interstitialCloseAction = null)
        {
            AdvertisementAnalyticsService.SendAdsShowRequested(AdvertisementAnalyticsService.AdsType.Interstitial);
            _interstitialCloseAction = interstitialCloseAction;
            _preloadCoroutine ??= _coroutine.StartCoroutine(PreloadAd());
        }

        public void AddBlocker(IInterstitialBlocker adBlocker) => _adBlockers.Add(adBlocker);

        public void DropAd()
        {
            if (_preloadCoroutine != null)
            {
                _coroutine.StopCoroutine(_preloadCoroutine);
                _preloadCoroutine = null;
            }

            if (_showCoroutine != null)
            {
                _coroutine.StopCoroutine(_showCoroutine);
                _showCoroutine = null;
            }

            if (_reloadCoroutine != null)
            {
                _coroutine.StopCoroutine(_reloadCoroutine);
                _reloadCoroutine = null;
            }

            if (AdIsLoaded())
                DestroyAd();
        }

        protected override string GetPlacementName()
        {
#if YABBI_AD == false && YANDEX_AD == false
            return AdvertisingSettings.EditorTest.Test;
#elif YABBI_AD
            return _advertisingConfigs.InterstitialUnitID;
#elif YANDEX_AD
            return _advertisingConfigs.InterstitialUnitID;
#endif
        }

        private IEnumerator PreloadAd()
        {
            if (AdIsLoaded())
                DestroyAd();

            while (CanLoadAd() == false)
                yield return new WaitForSeconds(RetryLoadAdDelay);

            LoadAd();
            _preloadCoroutine = null;
        }

        private IEnumerator DisplayAd()
        {
            while (_adBlockers.Any(b => b.InterstitialDisplayBlocked == true))
                yield return new WaitForSeconds(CheckBlockedDelay);

            _adBlockers.Clear();

            ShowAd();
            _showCoroutine = null;
        }

        private IEnumerator ReloadAd()
        {
            yield return new WaitForSeconds(CheckBlockedDelay);
            Show();
            _reloadCoroutine = null;
        }

        private void ReportClosure()
        {
            _interstitialCloseAction?.Invoke();
            _interstitialCloseAction = null;
            InterstitialClosed?.Invoke();
            _AdShown = false;
        }

        protected override bool CanLoadAd()
        {
#if YABBI_AD == false && YANDEX_AD == false
            return true;
#elif YABBI_AD
            return Yabbi.CanLoadAd(GetAdType(), GetPlacementName());
#elif YANDEX_AD
            return _interstitial == null;
#endif
        }

        protected override void LoadAd()
        {
#if YABBI_AD == false && YANDEX_AD == false
            Debug.Log("Advertisement Plugin: load interstitial");
            _showCoroutine ??= _coroutine.StartCoroutine(DisplayAd());
#elif YABBI_AD
            Yabbi.LoadAd(GetAdType(), GetPlacementName());
#elif YANDEX_AD
            _interstitialAdLoader.LoadAd(CreateAdRequest(GetPlacementName()));
#endif
        }

        protected override bool AdIsLoaded()
        {
#if YABBI_AD == false && YANDEX_AD == false
            return true;
#elif YABBI_AD
            return Yabbi.IsAdLoaded(GetAdType(), GetPlacementName());
#elif YANDEX_AD
            return _interstitial != null;
#endif
        }

        protected override void ShowAd()
        {
#if YABBI_AD == false && YANDEX_AD == false
            Debug.Log("Advertisement Plugin: show interstitial");
            ReportClosure();
#elif YABBI_AD
            Yabbi.ShowAd(GetAdType(), GetPlacementName());
#elif YANDEX_AD
            _interstitial.OnAdClicked += HandleAdClicked;
            _interstitial.OnAdShown += HandleAdShown;
            _interstitial.OnAdFailedToShow += HandleAdFailedToShow;
            _interstitial.OnAdImpression += HandleImpression;
            _interstitial.OnAdDismissed += HandleAdDismissed;

            _interstitial.Show();
#endif
        }

        protected override void DestroyAd()
        {
#if YABBI_AD
            Yabbi.DestroyAd(GetAdType(), GetPlacementName());
#elif YANDEX_AD
            if (_interstitial != null)
            {
                _interstitial.OnAdClicked -= HandleAdClicked;
                _interstitial.OnAdShown -= HandleAdShown;
                _interstitial.OnAdFailedToShow -= HandleAdFailedToShow;
                _interstitial.OnAdImpression -= HandleImpression;
                _interstitial.OnAdDismissed -= HandleAdDismissed;
                _interstitial.Destroy();
                _interstitial = null;
            }
#endif
        }

        #region YABBI_AD
#if YABBI_AD
        public void OnInterstitialLoaded(AdPayload adPayload)
        {
            AdvertisementAnalyticsService.SendAdsLoadSuccess(AdvertisementAnalyticsService.AdsType.Interstitial);
            _showCoroutine ??= _coroutine.StartCoroutine(DisplayAd());
        }

        public void OnInterstitialLoadFailed(AdPayload adPayload, AdException error)
        {
            AdvertisementAnalyticsService.SendAdsLoadFailed(AdvertisementAnalyticsService.AdsType.Interstitial);
            _reloadCoroutine ??= _coroutine.StartCoroutine(ReloadAd());
        }

        public void OnInterstitialShown(AdPayload adPayload)
        {
            AdvertisementAnalyticsService.SendAdsShowSuccess(AdvertisementAnalyticsService.AdsType.Interstitial);
            _AdShown = true;
        }

        public void OnInterstitialShowFailed(AdPayload adPayload, AdException error)
        {
            AdvertisementAnalyticsService.SendAdsShowFailed(AdvertisementAnalyticsService.AdsType.Interstitial);
            ReportClosure();
        }

        public void OnInterstitialClosed(AdPayload adPayload)
        {
            AdvertisementAnalyticsService.SendAdsClosed(AdvertisementAnalyticsService.AdsType.Interstitial);
            ReportClosure();
        }

        private int GetAdType() => Yabbi.Interstitial;
#endif
        #endregion

        #region YANDEX_AD
#if YANDEX_AD
        private void HandleAdLoaded(object sender, InterstitialAdLoadedEventArgs args)
        {
            _interstitial = args.Interstitial;
            _showCoroutine ??= _coroutine.StartCoroutine(DisplayAd());
        }

        private void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args) => _reloadCoroutine ??= _coroutine.StartCoroutine(ReloadAd());
        private AdRequestConfiguration CreateAdRequest(string adUnitId) => new AdRequestConfiguration.Builder(adUnitId).Build();
        private void HandleAdClicked(object sender, EventArgs args) { }
        private void HandleAdShown(object sender, EventArgs args) { }
        private void HandleImpression(object sender, ImpressionData impressionData) { }
        private void HandleAdDismissed(object sender, EventArgs args)
        {
            DestroyAd();
            ReportClosure();
        }

        private void HandleAdFailedToShow(object sender, AdFailureEventArgs args)
        {
            DestroyAd();
            ReportClosure();
        }
#endif
        #endregion
    }
}
