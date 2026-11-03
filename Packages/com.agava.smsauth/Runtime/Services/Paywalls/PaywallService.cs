using System.Threading.Tasks;
using KinDzaDzaGames.AdvertisementPlugin;
using UnityEngine.Scripting;

namespace Agava.Wink
{
    [Preserve]
    public class PaywallService
    {
        private const string PaywallVariantKey = "paywall_variant";

        private WinkAccessManager _winkAccessManager;
        private NotifyWindowHandler _notifyWindowHandler;
        private GameOrientation _gameOrientation;
        private ScreenshotProtector _screenshotProtector;
        private PhoneService _phoneService;
        private DemoTimer _demoTimer;
        private IInterstitialBlocker _adBlocker;
        private IBannerBlocker _bannerBlocker;
        private PaywallVariants _paywallVariant;

        public PaywallService(WinkAccessManager winkAccessManager, NotifyWindowHandler notifyWindowHandler, GameOrientation gameOrientation, ScreenshotProtector screenshotProtector, PhoneService phoneService, DemoTimer demoTimer, IInterstitialBlocker adBlocker, IBannerBlocker bannerBlocker)
        {
            _winkAccessManager = winkAccessManager;
            _notifyWindowHandler = notifyWindowHandler;
            _gameOrientation = gameOrientation;
            _screenshotProtector = screenshotProtector;
            _phoneService = phoneService;
            _demoTimer = demoTimer;
            _adBlocker = adBlocker;
            _bannerBlocker = bannerBlocker;

            InitializePaywallVariants();

            _demoTimer.TimerExpired += OnTimerExpired;
        }

        public void Dispose()
        {
            _demoTimer.TimerExpired -= OnTimerExpired;
        }

        public async Task<bool> TryShowPaywall(PaywallVariants paywallVariant)
        {
            if (_paywallVariant != PaywallVariants.None && _winkAccessManager.Authenficated)
            {
                bool isCorrectPaywall = _paywallVariant == paywallVariant;
                bool isUserHasSubscription = await _winkAccessManager.CheckSubscription();

                if (isCorrectPaywall && !isUserHasSubscription)
                {
                    ShowPaywall();
                }

                return isCorrectPaywall && !isUserHasSubscription;
            }

            return false;
        }

        private void ShowPaywall()
        {
            if (_gameOrientation.NeedChangeOrientation)
            {
                _gameOrientation.SetPortraitOrientation();
            }

            _notifyWindowHandler.ChangeDemoModeOption(enabled: false);

            if (AdvertisementController.Instance != null && AdvertisementController.Instance.CanShowReward())
            {
                _notifyWindowHandler.OpenWindow(WindowType.RewardContinue);
            }
            else
            {
                _phoneService.SetPhone();
                _notifyWindowHandler.OpenHelloWindowWOAccess();
            }

            _screenshotProtector.TryDisableScreenshots();

            AdvertisementController.Instance?.AddInterstitialBlocker(_adBlocker);
            AdvertisementController.Instance?.SuspendDisplayBanner(_bannerBlocker);
        }

        private void InitializePaywallVariants()
        {
            _paywallVariant = PaywallVariants.FirstCustomPlace;
        }

        private void OnTimerExpired()
        {
            TryShowPaywall(PaywallVariants.DemoTimer);
        }
    }
}
