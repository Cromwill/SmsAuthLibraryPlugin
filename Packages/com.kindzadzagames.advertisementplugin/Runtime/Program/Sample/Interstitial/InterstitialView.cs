#if YABBI_AD
using YabbiSDK.Api;
using SspnetSDK.Unfiled;
#endif

namespace KinDzaDzaGames.AdvertisementPlugin
{
    public class InterstitialView : AdvertisementView
#if YABBI_AD
        , IInterstitialAdListener
#endif
    {
        public void Construct()
        {
#if YABBI_AD
            Yabbi.SetInterstitialCallbacks(this);
#endif
        }
        #if YABBI_AD
        public void OnInterstitialLoaded(AdPayload adPayload)
        {
            AddLog("OnInterstitialLoaded");
        }

        public void OnInterstitialLoadFailed(AdPayload adPayload, AdException error)
        {
            AddLog($"OnInterstitialLoadFailed: {error.Description}");
        }

        public void OnInterstitialShown(AdPayload adPayload)
        {
            AddLog("OnInterstitialShown");
        }

        public void OnInterstitialShowFailed(AdPayload adPayload, AdException error)
        {
            AddLog($"OnInterstitialShowFailed: {error.Description}");
        }

        public void OnInterstitialClosed(AdPayload adPayload)
        {
            AddLog("OnInterstitialClosed");
        }
#endif

        protected override string GetPlacementName() => AdvertisingSettings.YabbiAds.yabbiInterstitialUnitID;
        protected override int GetAdType()
        {
#if YABBI_AD
            return Yabbi.Interstitial;
#else
            return 0;
#endif
        }
    }
}
