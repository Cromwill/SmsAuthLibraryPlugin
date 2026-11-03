using System;
using System.Collections.Generic;
using System.Linq;
using AdsAppView.Utility;
using UnityEngine;
using UnityEngine.Scripting;

namespace Agava.Wink
{
    [Serializable, Preserve]
    internal class NotifyWindowHandler : IWebViewLoader
    {
        [Header("UI Windows")]
        [SerializeField] private SignInWindowPresenter _signInWindow;
        [SerializeField] private NotifyWindowPresenter _failWindow;
        [SerializeField] private ProcessWindowPresenter _proccesOnWindow;
        [SerializeField] private HelloWindowPresenter _helloWindow;
        [SerializeField] private UnlinkWindowPresenter _unlinkWindow;
        //[SerializeField] private RedirectWindowPresenter _demoTimerExpiredWindow;
        [SerializeField] private NotifyWindowPresenter _noEnternetWindow;
        [SerializeField] private RedirectWindowPresenter _redirectToWebsiteWindow;
        [SerializeField] private InputWindowPresenter _enterCodeWindow;
        [SerializeField] private WinkProfileWindow _winkProfileWindow;
        [SerializeField] private DeleteAccountWindowPresenter _deleteAccountWindow;
        [SerializeField] private TurnOffAdWindowPresenter _verticalTurnOffAdWindow;
        [SerializeField] private TurnOffAdWindowPresenter _horizontalTurnOffAdWindow;
        [SerializeField] private WinkInfoVericalWindowPresenter _winkInfoVericalWindowPresenter;
        [SerializeField] private WinkInfoHorizontalWindowPresenter _winkInfoHorizontalWindowPresenter;
        [SerializeField] private SubscriptionCheckWindowPresenter _subscriptionCheckWindow;
        [SerializeField] private HelloWOAccessWindowPresenter _helloWOAccessWindow;
        [SerializeField] private OrientationСhangeWindowPresenter _orientationСhangeWindow;
        [SerializeField] private WebViewPresenter _webViewPresenter;
        [SerializeField] private RewardContinueWindowPresenter _rewardContinueWindowPresenter;
        [SerializeField] private RewardSettings _rewardSettings;
        [SerializeField] private bool _hasDemoMode = true;
        [Header("All UI Windows")]
        [SerializeField] private List<WindowPresenter> _windows;
        [Header("Corousel data")]
        [SerializeField] private CarouselSettings _carouselSettings;

        private WinkWebViewURLHandler _winkWebViewURLHandler;
        private GameOrientation _gameOrientation;
        private ScreenshotProtector _screenshotProtector;
        private DemoTimer _demoTimer;
        private bool _subscriptionChecked = false;
        private bool _useAdMechanics = false;
        private Store _storeName;

        //private bool _choosedFreeTrial => (_redirectToWebsiteWindow.TryFreeWink || _demoTimerExpiredWindow.TryFreeWink) && _subscriptionChecked == false;

        public bool IsAnyWindowEnabled => _windows.Any(window => window.Enabled);
        public bool ZeroSecondsCodeTimer => _enterCodeWindow.ZeroSeconds;
        public bool EnterCodeWindowInitialized => _enterCodeWindow.Initialized;
        public bool Loaded { get; private set; } = false;

        public event Action SunbscriptionBuyed;
        public event Action WebViewClosed;

        internal void Construct(GameOrientation gameOrientation, WinkWebViewURLHandler winkWebViewURLHandler, DemoTimer demoTimer, ScreenshotProtector screenshotProtector, ICoroutine coroutine, Store storeName, AppMetricaInfo appMetricaInfo, SmsRetrieverManager smsRetrieverManager)
        {
            _winkWebViewURLHandler = winkWebViewURLHandler ?? throw new ArgumentNullException(nameof(winkWebViewURLHandler));
            _gameOrientation = gameOrientation ?? throw new ArgumentNullException(nameof(gameOrientation));
            _screenshotProtector = screenshotProtector ?? throw new ArgumentNullException(nameof(screenshotProtector));
            _demoTimer = demoTimer ?? throw new ArgumentNullException(nameof(demoTimer));
            _storeName = storeName;

            _orientationСhangeWindow.Construct(_gameOrientation, _noEnternetWindow);
            _subscriptionCheckWindow.Construct(_noEnternetWindow);
            _webViewPresenter.Construct(this, OpenHelloAfterCloseWebView, ConfirmPurchaseSubscriptionOnWebView);
            coroutine.StartCoroutine(_rewardContinueWindowPresenter.Construct(_demoTimer, storeName.ToString(), appMetricaInfo, _rewardSettings));
            _enterCodeWindow.Construct(smsRetrieverManager);

            _subscriptionCheckWindow.LoadingStarted += OnLoadingStarted;
            _subscriptionCheckWindow.LoadingCompleted += OnLoadingCompleted;
            _rewardContinueWindowPresenter.RewardSuccessed += OnRewardSuccessed;
        }

        internal void Dispose()
        {
            _enterCodeWindow.Dispose();
            _subscriptionCheckWindow.LoadingStarted -= OnLoadingStarted;
            _subscriptionCheckWindow.LoadingCompleted -= OnLoadingCompleted;
            _rewardContinueWindowPresenter.RewardSuccessed -= OnRewardSuccessed;
        }

        internal void ApplyTurnOffABTests()
        {
            _verticalTurnOffAdWindow.Construct(_rewardSettings);
            _horizontalTurnOffAdWindow.Construct(_rewardSettings);
        }

        internal void OpenSignInWindow(Action closeCallback = null) => _signInWindow.Enable(closeCallback);
        internal void OpenWindow(WindowType type) => GetWindowByType(type).Enable();
        internal void CloseWindow(WindowType type) => GetWindowByType(type).Disable();

        internal void OpenInputOtpCodeWindow(string phone, string appHash, Action<string> onInputDone = null, Action onBackClicked = null)
        {
            _enterCodeWindow.Enable(phone, appHash, onInputDone, onBackClicked);
            _signInWindow.Clear();
        }

        internal void ActivateOtpCodeSetter() => _enterCodeWindow.ActivateOtpCodeSetter();

        /*internal void OpenDemoExpiredWindow(bool closeButton)
        {
            _enterCodeWindow.ResetCodeTimer();
            _redirectToWebsiteWindow.ResetFreeChoise();
            _demoTimerExpiredWindow.Enable();
        }*/

        internal void OpenRewardWindow()
        {
            _rewardContinueWindowPresenter.Enable();
            //_redirectToWebsiteWindow.Disable();
            _helloWOAccessWindow.Disable();
            _winkInfoVericalWindowPresenter.Disable();
        }

        internal void OpenDeleteAccountWindow(Action onDeleteAccount) => _deleteAccountWindow.Enable(onDeleteAccount);

        internal void OpenHelloWindow(bool hasAccess)
        {
            if(hasAccess)
            {
                AnalyticsWinkService.SendHelloWindow();
                _helloWindow.Enable(hasAccess);
            }
            else
            {
                OpenHelloWindowWOAccess();
            }
        }

        internal void OpenHelloWindowWOAccess()
        {
            AnalyticsWinkService.SendHelloWOAccessWindow();
            _helloWOAccessWindow.Enable();
        }

        internal void ChangeDemoModeOption(bool enabled)
        {
            if(_hasDemoMode == false)
                _helloWOAccessWindow.HideCloseButton();
            else
                _helloWOAccessWindow.TryShowCloseButton(enabled: enabled);

        }

        internal void Response(bool accepted) => _enterCodeWindow.Response(accepted);

        internal void CloseAllWindows(Action onClosed)
        {
            _windows.ForEach(window => window.Disable());
            onClosed?.Invoke();
        }

        internal void FillTextFields()
        {
            _carouselSettings.Construct(_storeName);
            _redirectToWebsiteWindow.FillRemoteTexts();
            _signInWindow.FillRemoteTexts();
            _enterCodeWindow.FillRemoteTexts();
            _subscriptionCheckWindow.FillRemoteTexts();
            _winkInfoVericalWindowPresenter.FillRemoteTexts();
            _winkInfoHorizontalWindowPresenter.FillRemoteTexts();
            _helloWOAccessWindow.FillRemoteTexts();
            _verticalTurnOffAdWindow.FillRemoteTexts(_carouselSettings);
            _horizontalTurnOffAdWindow.FillRemoteTexts(_carouselSettings);
            _helloWindow.ConstructCorousel(_carouselSettings);
            _rewardContinueWindowPresenter.ConstructCorousel(_carouselSettings);
            _winkProfileWindow.ConstructCorousel(_carouselSettings);
        }

        internal bool HasOpenedWindow(WindowType type)
            => _windows.Any(window => window.Type == type && window.isActiveAndEnabled == true);

        internal void OpenAdOffOffer(bool isHorizontal)
        {
            SetAdOption(adOption: true);

            if (isHorizontal)
                _winkInfoHorizontalWindowPresenter.EnableAdVariant();
            else
                _winkInfoVericalWindowPresenter.EnableAdVariant();
        }

        internal void EnableAdOfferInfo()
        {
            _subscriptionChecked = false;
            _winkInfoVericalWindowPresenter.InstallAdTexts();
        }

        internal void EnableOriginalOfferInfo(bool enableClose)
        {
            _winkInfoVericalWindowPresenter.InstallOriginalTexts();

            if (_hasDemoMode == false)
                _winkInfoVericalWindowPresenter.HideCloseButton();
            else
                _winkInfoVericalWindowPresenter.TryShowCloseButton(enabled:  enableClose);
        }

        internal void CloseAdOffer()
        {
            CloseWindow(_gameOrientation.NeedChangeOrientation ? WindowType.TurnOffAdHorizontal : WindowType.TurnOffAdVertical);
            _winkInfoVericalWindowPresenter.Disable();
            _winkInfoHorizontalWindowPresenter.Disable();
            SetAdOption(adOption: false);
        }

        internal void SetAdOption(bool adOption) => _useAdMechanics = adOption;

        private WindowPresenter GetWindowByType(WindowType type)
            => _windows.FirstOrDefault(window => window.Type == type);

        private void OnLoadingStarted()
        {
            Loaded = false;
            WebViewPresenter.ShowWebView(_winkWebViewURLHandler.GetURL());
        }

        private void OnLoadingCompleted()
        {
            _subscriptionChecked = true;
            Loaded = true;
        }

        private void OpenHelloAfterCloseWebView()
        {
            if (_useAdMechanics == false)
            {
                OpenHelloWindowWOAccess();
                _subscriptionCheckWindow.Disable();
            }
            else
            {
                _subscriptionCheckWindow.Disable();
                WebViewClosed?.Invoke();
            }
        }

        private void ConfirmPurchaseSubscriptionOnWebView()
        {
            //OpenHelloWindow(hasAccess: true);
            SunbscriptionBuyed?.Invoke();
            //_subscriptionCheckWindow.Disable();
        }

        private void OnRewardSuccessed()
        {
            CloseAllWindows(null);

            if (_gameOrientation.NeedChangeOrientation)
                _gameOrientation.SetLandscapeOrientation();

            _screenshotProtector.TryEnableScreenshots();

            _helloWOAccessWindow.HideCloseButton();
            _winkInfoVericalWindowPresenter.HideCloseButton();

            if (_hasDemoMode)
            {
                _helloWOAccessWindow.TryShowCloseButton(enabled: _demoTimer.Expired == false);
                _winkInfoVericalWindowPresenter.TryShowCloseButton(enabled: _demoTimer.Expired == false);
            }
        }
    }
}
