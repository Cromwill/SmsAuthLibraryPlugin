using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;
using System.Collections.Generic;
using SmsAuthAPI.Program;

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
        [SerializeField] private WinkInfoWindowPresenter _winkInfoWindow;
        [SerializeField] private SubscriptionCheckWindowPresenter _subscriptionCheckWindow;
        [SerializeField] private HelloWOAccessWindowPresenter _helloWOAccessWindow;
        [SerializeField] private OrientationСhangeWindowPresenter _orientationСhangeWindow;
        [SerializeField] private WebViewPresenter _webViewPresenter;
        [Header("All UI Windows")]
        [SerializeField] private List<WindowPresenter> _windows;

        private WinkWebViewURLHandler _winkWebViewURLHandler;
        private bool _subscriptionChecked = false;

        private bool _choosedFreeTrial => (_redirectToWebsiteWindow.TryFreeWink || _demoTimerExpiredWindow.TryFreeWink) && _subscriptionChecked == false;

        public bool IsAnyWindowEnabled => _windows.Any(window => window.Enabled);
        public bool ZeroSecondsCodeTimer => _enterCodeWindow.ZeroSeconds;
        public bool EnterCodeWindowInitialized => _enterCodeWindow.Initialized;
        public bool Loaded { get; private set; } = false;

        public event Action SunbscriptionBuyed;

        internal void Construct(GameOrientation gameOrientation, WinkWebViewURLHandler winkWebViewURLHandler)
        {
            _winkWebViewURLHandler = winkWebViewURLHandler ?? throw new ArgumentNullException(nameof(winkWebViewURLHandler));

            _orientationСhangeWindow.Construct(gameOrientation, _noEnternetWindow);
            _subscriptionCheckWindow.Construct(_noEnternetWindow);
            _webViewPresenter.Construct(this, OpenHelloAfterCloseWebView, ConfirmPurchaseSubscriptionOnWebView);

            _subscriptionCheckWindow.LoadingStarted += OnLoadingStarted;
            _subscriptionCheckWindow.LoadingCompleted += OnLoadingCompleted;
        }

        internal void Dispose()
        {
            _subscriptionCheckWindow.LoadingStarted -= OnLoadingStarted;
            _subscriptionCheckWindow.LoadingCompleted -= OnLoadingCompleted;
        }

        internal void OpenSignInWindow(Action closeCallback = null) => _signInWindow.Enable(closeCallback);
        internal void OpenWindow(WindowType type) => GetWindowByType(type).Enable();
        internal void CloseWindow(WindowType type) => GetWindowByType(type).Disable();
        internal void OpenInputOtpCodeWindow(string phone, Action<string> onInputDone = null, Action onBackClicked = null)
        {
            _enterCodeWindow.Enable(phone, onInputDone, onBackClicked);
            _signInWindow.Clear();
        }
        internal void OpenDemoExpiredWindow(bool closeButton) => _demoTimerExpiredWindow.Enable(closeButton);
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
                    _winkInfoWindow.Enable();
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
            _winkInfoWindow.FillRemoteTexts();
            _helloWOAccessWindow.FillRemoteTexts();
            _redirectToWebsiteWindow.FillRemoteTexts();
            _demoTimerExpiredWindow.FillRemoteTexts();
        }

        internal bool HasOpenedWindow(WindowType type)
            => _windows.Any(window => window.Type == type && window.isActiveAndEnabled == true);

        internal void ConfirmPurchaseSubscriptionOnWebView()
        {
            OpenHelloWindow(hasAccess: true);
            SunbscriptionBuyed?.Invoke();
            _subscriptionCheckWindow.Disable();
        }

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
            OpenHelloWindowWOAccess();
            _subscriptionCheckWindow.Disable();
        }
    }
}
