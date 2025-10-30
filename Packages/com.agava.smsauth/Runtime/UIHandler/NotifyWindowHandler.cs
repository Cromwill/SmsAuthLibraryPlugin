using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;
using System.Collections.Generic;

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
        [SerializeField] private RedirectWindowPresenter _demoTimerExpiredWindow;
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
        [Header("All UI Windows")]
        [SerializeField] private List<WindowPresenter> _windows;

        private WinkWebViewURLHandler _winkWebViewURLHandler;
        private GameOrientation _gameOrientation;
        private ScreenshotProtector _screenshotProtector;
        private bool _subscriptionChecked = false;
        private bool _useAdMechanics = false;

        private bool _choosedFreeTrial => (_redirectToWebsiteWindow.TryFreeWink || _demoTimerExpiredWindow.TryFreeWink) && _subscriptionChecked == false;

        public bool IsAnyWindowEnabled => _windows.Any(window => window.Enabled);
        public bool ZeroSecondsCodeTimer => _enterCodeWindow.ZeroSeconds;
        public bool EnterCodeWindowInitialized => _enterCodeWindow.Initialized;
        public bool Loaded { get; private set; } = false;

        public event Action SunbscriptionBuyed;
        public event Action WebViewClosed;

        internal void Construct(GameOrientation gameOrientation, WinkWebViewURLHandler winkWebViewURLHandler, DemoTimer demoTimer, ScreenshotProtector screenshotProtector, ICoroutine coroutine, string storeName, AppMetricaInfo appMetricaInfo, SmsRetrieverManager smsRetrieverManager)
        {
            _winkWebViewURLHandler = winkWebViewURLHandler ?? throw new ArgumentNullException(nameof(winkWebViewURLHandler));
            _gameOrientation = gameOrientation ?? throw new ArgumentNullException(nameof(gameOrientation));
            _screenshotProtector = screenshotProtector ?? throw new ArgumentNullException(nameof(screenshotProtector));

            _orientationСhangeWindow.Construct(_gameOrientation, _noEnternetWindow);
            _subscriptionCheckWindow.Construct(_noEnternetWindow);
            _webViewPresenter.Construct(this, OpenHelloAfterCloseWebView, ConfirmPurchaseSubscriptionOnWebView);
            coroutine.StartCoroutine(_rewardContinueWindowPresenter.Construct(demoTimer, storeName, appMetricaInfo, _rewardSettings));
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

        internal void OpenDemoExpiredWindow(bool closeButton)
        {
            _enterCodeWindow.ResetCodeTimer();
            _redirectToWebsiteWindow.ResetFreeChoise();
            _demoTimerExpiredWindow.Enable(closeButton);
        }

        internal void OpenRewardWindow()
        {
            _rewardContinueWindowPresenter.Enable();
            _redirectToWebsiteWindow.Disable();
            _helloWOAccessWindow.Disable();
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
                if(_choosedFreeTrial)
                    _winkInfoVericalWindowPresenter.Enable();
                else
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
            _redirectToWebsiteWindow.TryShowCloseButton(enabled: enabled);
            _redirectToWebsiteWindow.TryShowRewardButton(enabled: enabled);
            _demoTimerExpiredWindow.TryShowRewardButton(enabled: enabled);
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
            _winkInfoVericalWindowPresenter.FillRemoteTexts();
            _winkInfoHorizontalWindowPresenter.FillRemoteTexts();
            _helloWOAccessWindow.FillRemoteTexts();
            _redirectToWebsiteWindow.FillRemoteTexts();
            _demoTimerExpiredWindow.FillRemoteTexts();
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
            _redirectToWebsiteWindow.EnableFreeChoise();
            _demoTimerExpiredWindow.EnableFreeChoise();
            _winkInfoVericalWindowPresenter.InstallAdTexts();
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
            OpenHelloWindow(hasAccess: true);
            SunbscriptionBuyed?.Invoke();
            _subscriptionCheckWindow.Disable();
        }

        private void OnRewardSuccessed()
        {
            CloseAllWindows(null);

            if (_gameOrientation.NeedChangeOrientation)
                _gameOrientation.SetLandscapeOrientation();

            _screenshotProtector.TryEnableScreenshots();
        }
    }
}
