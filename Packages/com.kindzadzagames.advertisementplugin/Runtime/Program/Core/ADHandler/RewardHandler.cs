using System;
using UnityEngine;
using UnityEngine.Scripting;

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
    public class RewardHandler : AdvertisementHandler
#if YABBI_AD
        , IRewardedAdListener
#endif
    {
        private const string ViewedAds = nameof(ViewedAds);

        private readonly AdvertisingConfigs _advertisingConfigs;
        private readonly bool _canShowRewards = false;
        private readonly int _rewardsMaxCount = 0;

        private int _rewardsCount = 0;
        private bool _rewardReceived = false;
        private bool _AdShown = false;
        private Action _preRewardAction;
        private Action _rewardSuccessAction;
        private Action _rewardFailureAction;

#if YANDEX_AD
        private RewardedAdLoader _rewardedAdLoader;
        private RewardedAd _rewardedAd;
#endif

        public RewardHandler(AdvertisingConfigs advertisingConfigs, RewardSettings rewardSettings)
        {
            _advertisingConfigs = advertisingConfigs ?? throw new ArgumentNullException(nameof(advertisingConfigs));

            _canShowRewards = rewardSettings.RewardAvailable;
            _rewardsMaxCount = rewardSettings.RewardCount;
            _rewardsCount = PlayerPrefs.GetInt(ViewedAds, 0);

#if UNITY_EDITOR && YABBI_AD == false && YANDEX_AD == false
            Debug.Log("Advertisement Plugin: reward handler inited.");
# elif YABBI_AD
            Yabbi.SetRewardedCallbacks(this);
#elif YANDEX_AD
            _rewardedAdLoader = new RewardedAdLoader();
            _rewardedAdLoader.OnAdLoaded += HandleAdLoaded;
            _rewardedAdLoader.OnAdFailedToLoad += HandleAdFailedToLoad;
#endif
        }

        public void Dispose()
        {
#if YANDEX_AD
            _rewardedAdLoader.OnAdLoaded -= HandleAdLoaded;
            _rewardedAdLoader.OnAdFailedToLoad -= HandleAdFailedToLoad;
#endif
            DestroyAd();
        }

        public void ChangeFocusState(bool focus)
        {
            if (focus && _AdShown)
            {
                DestroyAd();
                CancelReward();
            }
        }

        public bool CanShow() => _canShowRewards && (_rewardsCount < _rewardsMaxCount || _rewardsMaxCount == 0);

        public void LoadAD(Action preRewardAction)
        {
            if(AdIsLoaded())
            {
                preRewardAction?.Invoke();
                return;
            }

            if (CanLoadAd() == false)
                return;

            _preRewardAction = preRewardAction;
            LoadAd();
        }

        public void Show(Action rewardSuccessAction = null, Action rewardFailureAction = null)
        {
            if (AdIsLoaded())
            {
                _rewardSuccessAction = rewardSuccessAction;
                _rewardFailureAction = rewardFailureAction;
                ShowAd();
            }
        }

        private void ApplyReward()
        {
            _rewardReceived = true;
            _rewardsCount++;
            PlayerPrefs.SetInt(ViewedAds, _rewardsCount);
            PlayerPrefs.Save();
            _rewardSuccessAction?.Invoke();
        }

        private void CancelReward()
        {
            if (_rewardReceived == false)
                _rewardFailureAction?.Invoke();

            DropRewardActions();
        }

        private void DropRewardActions()
        {
            _preRewardAction = null;
            _rewardSuccessAction = null;
            _rewardFailureAction = null;
            _rewardReceived = false;
            _AdShown = false;
        }

        protected override string GetPlacementName()
        {
#if UNITY_EDITOR && YABBI_AD == false && YANDEX_AD == false
            return AdvertisingSettings.EditorTest.Test;
#elif YABBI_AD
            return _advertisingConfigs.RewardedUnitID;
#elif YANDEX_AD
            return _advertisingConfigs.RewardedUnitID;
#endif
        }

        protected override bool CanLoadAd()
        {
#if UNITY_EDITOR && YABBI_AD == false && YANDEX_AD == false
            return true;
#elif YABBI_AD
            return Yabbi.CanLoadAd(GetAdType(), GetPlacementName());
#elif YANDEX_AD
            return _rewardedAd == null;
#endif
        }

        protected override void LoadAd()
        {
#if YABBI_AD
            Yabbi.LoadAd(GetAdType(), GetPlacementName());
#elif YANDEX_AD
            _rewardedAdLoader.LoadAd(CreateAdRequest(GetPlacementName()));
#endif
        }

        protected override bool AdIsLoaded()
        {
#if UNITY_EDITOR && YABBI_AD == false && YANDEX_AD == false
            return true;
#elif YABBI_AD
            return Yabbi.IsAdLoaded(GetAdType(), GetPlacementName());
#elif YANDEX_AD
            return _rewardedAd != null;
#endif
        }

        protected override void ShowAd()
        {
#if UNITY_EDITOR && YABBI_AD == false && YANDEX_AD == false
            ApplyReward();
            CancelReward();
#elif YABBI_AD
            Yabbi.ShowAd(GetAdType(), GetPlacementName());
#elif YANDEX_AD
            _rewardedAd.OnAdClicked += HandleAdClicked;
            _rewardedAd.OnAdShown += HandleAdShown;
            _rewardedAd.OnAdFailedToShow += HandleAdFailedToShow;
            _rewardedAd.OnAdImpression += HandleImpression;
            _rewardedAd.OnAdDismissed += HandleAdDismissed;
            _rewardedAd.OnRewarded += HandleRewarded;

            _rewardedAd.Show();
#endif
        }

        protected override void DestroyAd()
        {
#if YABBI_AD
            Yabbi.DestroyAd(GetAdType(), GetPlacementName());
#elif YANDEX_AD
            if (_rewardedAd != null)
            {
                _rewardedAd.OnAdClicked -= HandleAdClicked;
                _rewardedAd.OnAdShown -= HandleAdShown;
                _rewardedAd.OnAdFailedToShow -= HandleAdFailedToShow;
                _rewardedAd.OnAdImpression -= HandleImpression;
                _rewardedAd.OnAdDismissed -= HandleAdDismissed;
                _rewardedAd.OnRewarded -= HandleRewarded;
                _rewardedAd.Destroy();
                _rewardedAd = null;
            }
#endif
        }

        #region YABBI_AD
#if YABBI_AD
        public void OnRewardedLoaded(AdPayload adPayload) => _preRewardAction?.Invoke();
        public void OnRewardedLoadFailed(AdPayload adPayload, AdException error) => CancelReward();
        public void OnRewardedShowFailed(AdPayload adPayload, AdException error) => CancelReward();
        public void OnUserRewarded(AdPayload adPayload) => ApplyReward();
        public void OnRewardedClosed(AdPayload adPayload) => CancelReward();

        public void OnRewardedShown(AdPayload adPayload) => _AdShown = true;
        public void OnRewardedVideoStarted(AdPayload adPayload) { }
        public void OnRewardedVideoCompleted(AdPayload adPayload) { }

        private int GetAdType() => Yabbi.Rewarded;
#endif
        #endregion

        #region YANDEX_AD
#if YANDEX_AD
        private void HandleAdLoaded(object sender, RewardedAdLoadedEventArgs args)
        {
            _rewardedAd = args.RewardedAd;
            _preRewardAction?.Invoke();
        }

        private void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args) => DropRewardActions();
        private AdRequestConfiguration CreateAdRequest(string adUnitId) => new AdRequestConfiguration.Builder(adUnitId).Build();
        private void HandleAdClicked(object sender, EventArgs args) { }
        private void HandleAdShown(object sender, EventArgs args) { }
        private void HandleImpression(object sender, ImpressionData impressionData) { }
        private void HandleAdFailedToShow(object sender, AdFailureEventArgs args) => DropRewardActions();

        private void HandleRewarded(object sender, Reward args)
        {
            ApplyReward();
        }

        private void HandleAdDismissed(object sender, EventArgs args)
        {
            DestroyAd();
            CancelReward();
        }
#endif
        #endregion
    }
}
