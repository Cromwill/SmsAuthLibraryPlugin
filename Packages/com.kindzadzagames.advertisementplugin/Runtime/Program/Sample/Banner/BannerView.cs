using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

#if YABBI_AD
using YabbiSDK.Api;
using SspnetSDK.Unfiled;
#endif

namespace KinDzaDzaGames.AdvertisementPlugin
{
    public class BannerView : AdvertisementView
#if YABBI_AD
        , IBannerAdListener
#endif
    {
        private const string ShowCloseBunnerButtonText = "Close banner button is shown";
        private const string HideCloseBunnerButtonText = "Close banner button is hidden";
        private const string BannerPositionPattern = "Banner position: {0}";
        private const string BannerBottomPosition = "BOTTOM";
        private const string BannerTopPosition = "TOP";
        private const float RetryLoadBannerDelay = 1f;

        [SerializeField] private Button _showBannerButton;
        [SerializeField] private TMP_Text _showBannerButtonLabel;
        [SerializeField] private Button _changeBannerPositionButton;
        [SerializeField] private TMP_Text _changeBannerPositionButtonLabel;
        [SerializeField] private int _switchADTime = 10;
        [SerializeField] private Button _autoBannerButton;
        [SerializeField] private Button _reloadSceneButton;

        private bool _bannerVisibility = true;
        private bool _bannerAtBottom = true;
        private bool _autoBanner = false;
        private Coroutine _bannerCoroutine = null;

        public void Construct()
        {
#if YABBI_AD
            Yabbi.SetBannerCallbacks(this);
            Yabbi.SetBannerCustomSettings(new BannerSettings().SetRefreshIntervalSeconds(_switchADTime).SetShowCloseButton(true));
#endif
            _showBannerButtonLabel.text = ShowCloseBunnerButtonText;
            _changeBannerPositionButtonLabel.text = _bannerAtBottom ? string.Format(BannerPositionPattern, BannerBottomPosition) : string.Format(BannerPositionPattern, BannerTopPosition);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _showBannerButton.onClick.AddListener(ChangeBunnerCloseButtonVisibility);
            _changeBannerPositionButton.onClick.AddListener(ChangeBunnerPosition);
            _autoBannerButton.onClick.AddListener(EnableAutoHideOpenBanner);
            _reloadSceneButton.onClick.AddListener(ReloadScene);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            _showBannerButton.onClick.RemoveListener(ChangeBunnerCloseButtonVisibility);
            _changeBannerPositionButton.onClick.RemoveListener(ChangeBunnerPosition);
            _autoBannerButton.onClick.RemoveListener(EnableAutoHideOpenBanner);
            _reloadSceneButton.onClick.RemoveListener(ReloadScene);
        }

        public override void Show()
        {
            base.Show();

            if(_autoBanner)
                _bannerCoroutine = StartCoroutine(ResumeDisplayBanner());
        }
#if YABBI_AD
        public void OnBannerLoaded(AdPayload adPayload) => AddLog("OnBannerLoaded");
        public void OnBannerLoadFailed(AdPayload adPayload, AdException error) => AddLog($"OnBannerLoadFailed: {error.Description}");
        public void OnBannerShown(AdPayload adPayload) => AddLog("OnBannerShown");
        public void OnBannerShowFailed(AdPayload adPayload, AdException error) => AddLog($"OnBannerShowFailed: {error.Description}");
        public void OnBannerClosed(AdPayload adPayload) => AddLog("OnBannerClosed");
        public void OnBannerImpression(AdPayload adPayload) => AddLog("OnBannerImpression");
#endif

        protected override void Hide()
        {
            if(_bannerCoroutine != null)
            {
                StopCoroutine(_bannerCoroutine);
                _autoBanner = false;
                _bannerCoroutine = null;
            }

            base.Hide();
        }

        protected override string GetPlacementName() => AdvertisingSettings.YabbiAds.yabbiBannerUnitID;
        protected override int GetAdType()
        {
#if YABBI_AD
            return Yabbi.Banner;
#else
            return 0;
#endif
        }

        private void ChangeBunnerCloseButtonVisibility()
        {
            _bannerVisibility = !_bannerVisibility;
#if YABBI_AD
            Yabbi.SetBannerCustomSettings(new BannerSettings().SetRefreshIntervalSeconds(_switchADTime).SetShowCloseButton(_bannerVisibility));
#endif
            _showBannerButtonLabel.text = _bannerVisibility ? ShowCloseBunnerButtonText : HideCloseBunnerButtonText;
        }

        private void ChangeBunnerPosition()
        {
            _bannerAtBottom = !_bannerAtBottom;
#if YABBI_AD
            Yabbi.SetBannerCustomSettings(new BannerSettings().SetRefreshIntervalSeconds(_switchADTime).SetShowCloseButton(_bannerVisibility).SetBannerPosition(_bannerAtBottom ? BannerPosition.BOTTOM : BannerPosition.TOP));
#endif
            _changeBannerPositionButtonLabel.text = _bannerAtBottom ? string.Format(BannerPositionPattern, BannerBottomPosition) : string.Format(BannerPositionPattern, BannerTopPosition);
        }

        private void EnableAutoHideOpenBanner() => _autoBanner = true;

        private IEnumerator ResumeDisplayBanner()
        {
#if YABBI_AD
            while (Yabbi.CanLoadAd(GetAdType(), GetPlacementName()) == false)
                yield return new WaitForSeconds(RetryLoadBannerDelay);

            Yabbi.LoadAd(GetAdType(), GetPlacementName());

            while (Yabbi.IsAdLoaded(GetAdType(), GetPlacementName()) == false)
                yield return new WaitForSeconds(RetryLoadBannerDelay);

            Yabbi.ShowAd(GetAdType(), GetPlacementName());
#else
            yield return null;
#endif
            _autoBanner = false;
            _bannerCoroutine = null;
        }

        private void ReloadScene() => SceneManager.LoadScene(0);
    }
}
