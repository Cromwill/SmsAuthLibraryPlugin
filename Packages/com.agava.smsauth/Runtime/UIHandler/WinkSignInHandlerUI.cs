using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Scripting;
using System.Collections.Generic;

namespace Agava.Wink
{
    /// <summary>
    ///     Handler UI. Input data and view auth process.
    /// </summary>
    [Preserve]
    public class WinkSignInHandlerUI : MonoBehaviour, IWinkSignInHandlerUI, ICoroutine
    {
        private const float RedirectWindowDelay = 1.0f;
        private const float ChangeOrientationDelay = 1.0f;

        [SerializeField] private ScreenshotProtector _screenshotProtector;
        [SerializeField] private DemoTimer _demoTimer;
        [SerializeField] private NotifyWindowHandler _notifyWindowHandler;
        [Header("App name (for WebView)")]
        [SerializeField] private WinkWebViewURLHandler _webViewURLHandler;
        [Header("Game orientation")]
        [SerializeField] private GameOrientation _gameOrientation;
        [Header("UI Input")]
        [SerializeField] private PhoneNumberFormatting _numbersInputField;
        [Header("UI Buttons")]
        [SerializeField] private Button _signInContinueButton;
        [SerializeField] private Button _enterCodeContinueButton;
        [SerializeField] private Button[] _signInButtons;
        [SerializeField] private Button[] _tryWinkButtons;
        [SerializeField] private Button[] _switchOrientationButtons;
        [SerializeField] private Button[] _subscriptionCheckButtons;
        [SerializeField] private Button[] _closeButtonsFromSettings;
        [SerializeField] private Button _closeWinkInfoButton;
        [Header("Analytics buttons")]
        [SerializeField] private AnalyticsSender _analyticsSender;
        [Header("Factory components")]
        [SerializeField] private UnlinkDeviceViewContainer _unlinkDeviceViewContainer;
        [Header("Placeholders")]
        [SerializeField] private TextPlaceholder[] _phoneNumberPlaceholders;
        [Header("WebView")]
        [SerializeField] private WebViewPresenter _webViewPresenter;

        private SignInFuctionsUI _signInFuctionsUI;
        private WinkAccessManager _winkAccessManager;
        private bool _logInFromSettings = false;

        public static WinkSignInHandlerUI Instance { get; private set; }

        public bool IsAnyWindowEnabled => _notifyWindowHandler.IsAnyWindowEnabled;

        public event Action AllWindowsClosed;

        private void Awake()
        {
            StartCoroutine(_webViewURLHandler.Construct());

            _webViewURLHandler.CheckAvailabilityURL();
            _notifyWindowHandler.Construct(_gameOrientation, _webViewURLHandler);
            _notifyWindowHandler.OpenWindow(WindowType.ProccessOn);
        }

        private void OnApplicationFocus(bool focus) => _signInFuctionsUI?.OnAppFocus(focus);

        private void Update() => _signInFuctionsUI?.Update();

        public void Dispose()
        {
            if (_signInFuctionsUI == null) return;

            _enterCodeContinueButton.onClick.RemoveListener(OnEnterCodeContinueClicked);
            _signInContinueButton.onClick.RemoveListener(OnSignInContinueClicked);

            foreach (var button in _signInButtons)
                button.onClick.RemoveListener(OpenSignWindow);

            foreach (var button in _tryWinkButtons)
                button.onClick.RemoveListener(OpenSignWindow);

            foreach (var button in _switchOrientationButtons)
                button.onClick.RemoveListener(OpenChangeOrientationWindow);

            foreach (var button in _subscriptionCheckButtons)
                button.onClick.RemoveListener(CheckSubscription);

            foreach (var button in _closeButtonsFromSettings)
                button.onClick.RemoveListener(ContinueGame);

            _closeWinkInfoButton.onClick.RemoveListener(OnCloseWinkInfoButtonClick);

            _unlinkDeviceViewContainer.DeviceRemoved -= OnUnlinkButtonClicked;

            if (_winkAccessManager == null) return;

            _winkAccessManager.ResetLogin -= OpenSignWindow;
            _winkAccessManager.LimitReached -= OnLimitReached;
            _winkAccessManager.SignInSuccessfully -= OnSignInSuccessfully;
            _demoTimer.TimerExpired -= OnTimerExpired;
            _demoTimer.FirstChecked -= OnTimerFirstChecked;
            _demoTimer.Dispose();
            _notifyWindowHandler.SunbscriptionBuyed -= OnSunbscriptionBuyed;
            _notifyWindowHandler.Dispose();
            _analyticsSender.Dispose();
        }

        public IEnumerator Initialize()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                _notifyWindowHandler.OpenWindow(WindowType.NoEnternet);
                yield return new WaitWhile(() => Application.internetReachability == NetworkReachability.NotReachable);
            }

            _notifyWindowHandler.CloseWindow(WindowType.NoEnternet);

            yield return new WaitUntil(() => _notifyWindowHandler.EnterCodeWindowInitialized);
            yield return new WaitUntil(() => _webViewPresenter.Initialized);

            var textConfigs = FindObjectsOfType<RemoteConfigText>();

            while (textConfigs.Any(config => config.Initialized == false))
                yield return null;
        }

        public void OpenProcessOnWindow()
        {
            _notifyWindowHandler.OpenWindow(WindowType.ProccessOn);
        }

        public void CloseProcessOnWindow()
        {
            _notifyWindowHandler.CloseWindow(WindowType.ProccessOn);
        }

        public void Construct()
        {
            StartCoroutine(EnternetChecking());
            _signInFuctionsUI.SetRemoteConfig();
        }

        public void StartService(WinkAccessManager winkAccessManager)
        {
            if (Instance == null)
                Instance = this;

            _signInFuctionsUI = new(_notifyWindowHandler, _demoTimer, winkAccessManager, this, this);
            _winkAccessManager = winkAccessManager;
            _analyticsSender.Construct();

            _enterCodeContinueButton.onClick.AddListener(OnEnterCodeContinueClicked);
            _signInContinueButton.onClick.AddListener(OnSignInContinueClicked);

            foreach (var button in _signInButtons)
                button.onClick.AddListener(OpenSignWindow);

            foreach (var button in _tryWinkButtons)
                button.onClick.AddListener(OpenSignWindow);

            foreach (var button in _switchOrientationButtons)
                button.onClick.AddListener(OpenChangeOrientationWindow);

            foreach (var button in _subscriptionCheckButtons)
                button.onClick.AddListener(CheckSubscription);

            foreach (var button in _closeButtonsFromSettings)
                button.onClick.AddListener(ContinueGame);

            _closeWinkInfoButton.onClick.AddListener(OnCloseWinkInfoButtonClick);

            _unlinkDeviceViewContainer.DeviceRemoved += OnUnlinkButtonClicked;

            CloseAllWindows();

            _winkAccessManager.ResetLogin += OpenSignWindow;
            _winkAccessManager.LimitReached += OnLimitReached;
            _winkAccessManager.SignInSuccessfully += OnSignInSuccessfully;
            _winkAccessManager.AuthorizationSuccessfully += OnAuthorizationSuccessfully;
            _demoTimer.TimerExpired += OnTimerExpired;
            _demoTimer.FirstChecked += OnTimerFirstChecked;
            _notifyWindowHandler.SunbscriptionBuyed += OnSunbscriptionBuyed;
        }

        public void OpenStartWindow() => OpenSubscriptionWindow();

        public void OpenSignWindow()
        {
            _notifyWindowHandler.OpenSignInWindow();
            AnalyticsWinkService.SendEnterPhoneWindow();
        }

        private void OpenChangeOrientationWindow()
        {
            _screenshotProtector.TryEnableScreenshots();

            if (_gameOrientation.NeedChangeOrientation)
                _notifyWindowHandler.OpenWindow(WindowType.OrientationСhange);
        }

        public void OpenSubscriptionWindow()
        {
            _screenshotProtector.TryDisableScreenshots();
            _notifyWindowHandler.OpenWindow(WindowType.Redirect);
            AnalyticsWinkService.SendSubscribeOfferWindow();
        }

        public void OpenWindow(WindowType type) => _notifyWindowHandler.OpenWindow(type);

        public void CloseAllWindows() => _notifyWindowHandler.CloseAllWindows(AllWindowsClosed);

        public void OnWinkButtonClick()
        {
            _screenshotProtector.TryDisableScreenshots();
            Action action = null;
            _logInFromSettings = true;

            if (_winkAccessManager.Authenficated)
            {
                AnalyticsWinkService.SendSubscribeButtonClickOnSettings();

                if (_winkAccessManager.HasAccess)
                    action = () => _notifyWindowHandler.OpenWindow(WindowType.WinkProfile);
                else
                    action = () => _notifyWindowHandler.OpenHelloWindowWOAccess();
            }
            else
            {
                action = OpenSignWindow;
            }

            _gameOrientation.SaveGameOrientation();
            StartCoroutine(ActionWithDelay(ChangeOrientationDelay, action));

            if (_gameOrientation.NeedChangeOrientation)
                _gameOrientation.SetPortraitOrientation();
        }

        private void ContinueGame()
        {
            if (_logInFromSettings)
            {
                _screenshotProtector.TryEnableScreenshots();
                _logInFromSettings = false;

                if (_gameOrientation.NeedChangeOrientation)
                {
                    _gameOrientation.SetLandscapeOrientationPosibility();
                    _gameOrientation.SetSavedOrientation();
                }
            }
        }

        public void OnDeleteAccountButtonClick()
        {
            _screenshotProtector.TryDisableScreenshots();
            _logInFromSettings = true;
            _gameOrientation.SaveGameOrientation();
            AnalyticsWinkService.SendDeleteAccountButtonClickOnSetting();

            if (_gameOrientation.NeedChangeOrientation)
                _gameOrientation.SetPortraitOrientation();

            StartCoroutine(ActionWithDelay(ChangeOrientationDelay, () =>
                _notifyWindowHandler.OpenDeleteAccountWindow(onDeleteAccount: () =>
                    {
                        _winkAccessManager.DeleteAccount(
                        onComplete: (resultSuccess) =>
                        {
                            AnalyticsWinkService.SendDeleteWindow();

                            if (resultSuccess == false)
                                _notifyWindowHandler.OpenWindow(WindowType.Fail);
                            else
                                ContinueGame();
                        });
                    })));

            /*_notifyWindowHandler.OpenDeleteAccountWindow(onDeleteAccount: () =>
                {
                    _winkAccessManager.DeleteAccount(
                    onComplete: (resultSuccess) =>
                    {
                        if (resultSuccess == false)
                        {
                            _notifyWindowHandler.OpenWindow(WindowType.Fail);
                        }
                        else
                        {
                            AnalyticsWinkService.SendDeleteWindow();
                        }
                    });
                });*/
        }

        public void SetRemoteTexts()
        {
            _notifyWindowHandler.FillTextFields();
        }

        public void TrySetCorrectOrientation()
        {
            if (_gameOrientation.NeedChangeOrientation)
                _gameOrientation.SetLandscapeOrientation();
        }

        private void OnSignInContinueClicked()
        {
            string number = _numbersInputField.Number;
            string formattedNumber = PhoneNumber.FormatNumber(number);

            foreach (TextPlaceholder placeholder in _phoneNumberPlaceholders)
                placeholder.ReplaceValue(formattedNumber);

            _signInFuctionsUI.OnSignInClicked(number, _notifyWindowHandler.ZeroSecondsCodeTimer == false);
        }

        private void OnLimitReached(IReadOnlyList<string> devicesList)
        {
            CloseAllWindows();

            _notifyWindowHandler.OpenWindow(WindowType.Unlink);
            _unlinkDeviceViewContainer.Initialize(devicesList);
        }

        private void OnUnlinkButtonClicked(UnlinkDeviceView unlinkDeviceView)
            => _signInFuctionsUI.OnUnlinkClicked(unlinkDeviceView.DeviceId);

        private void OnAuthorizationSuccessfully() => _signInFuctionsUI.OnAuthorizationSuccessfully();

        private void OnEnterCodeContinueClicked()
        {
            _notifyWindowHandler.CloseWindow(WindowType.Redirect);
            _notifyWindowHandler.CloseWindow(WindowType.EnterOtpCode);
        }

        private void OnSignInSuccessfully(bool hasAccess)
        {
            _screenshotProtector.TryDisableScreenshots();
            _numbersInputField.Clear();
            _signInFuctionsUI.OnSignInSuccesfully(hasAccess);

            SetPhone();
            _webViewURLHandler.SetPhone(_winkAccessManager.LoginData.phone);
            _notifyWindowHandler.CloseWindow(WindowType.Redirect);
            _notifyWindowHandler.OpenHelloWindow(hasAccess);

            /*if (hasAccess)
            {
                SetPhone();
                _notifyWindowHandler.CloseWindow(WindowType.Redirect);
                _notifyWindowHandler.OpenHelloWindow(hasAccess);
            }
            else
            {
                SetPhone();
                _notifyWindowHandler.CloseWindow(WindowType.Redirect);
                _notifyWindowHandler.OpenHelloWOAccessWindow();
            }*/
        }

        private void SetPhone()
        {
            string number = "N/A";

            if (UnityEngine.PlayerPrefs.HasKey(_winkAccessManager.PhoneNumber))
                number = PhoneNumber.FormatNumber(UnityEngine.PlayerPrefs.GetString(_winkAccessManager.PhoneNumber));

            foreach (TextPlaceholder placeholder in _phoneNumberPlaceholders)
                placeholder.ReplaceValue(number);
        }

        private void OnCloseWinkInfoButtonClick() => _notifyWindowHandler.OpenHelloWindowWOAccess();
        private void CheckSubscription() => _notifyWindowHandler.OpenWindow(WindowType.SubscriptionCheck);

        private void OnTimerExpired()
        {
            if (_gameOrientation.NeedChangeOrientation)
                _gameOrientation.SetPortraitOrientation();

            Debug.Log($"WINK PLUGIN: Timer Expired");
            //_notifyWindowHandler.OpenDemoExpiredWindow(false);
            _notifyWindowHandler.ChangeDemoModeOption(enabled: false);

            if (_winkAccessManager.Authenficated)
            {
                SetPhone();
                _notifyWindowHandler.OpenHelloWindowWOAccess();
            }
            else
            {
                AnalyticsWinkService.SendSubscribeOfferWindow();
                _notifyWindowHandler.OpenDemoExpiredWindow(false);
            }

            _screenshotProtector.TryDisableScreenshots();
        }

        private void OnTimerFirstChecked() => _notifyWindowHandler.ChangeDemoModeOption(enabled: _demoTimer.Expired == false);

        private void OnSunbscriptionBuyed()
        {
            _demoTimer.Stop();
            _winkAccessManager.ActivateTempSubscription();
        }

        private IEnumerator EnternetChecking()
        {
            var wait = new WaitForSecondsRealtime(1f);

            while (true)
            {
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    _notifyWindowHandler.OpenWindow(WindowType.NoEnternet);
                    Debug.LogError("NO CONNECTION");
                }
                else
                {
                    if (_notifyWindowHandler.HasOpenedWindow(WindowType.NoEnternet))
                        _notifyWindowHandler.CloseWindow(WindowType.NoEnternet);
                }

                yield return wait;
            }
        }

        private IEnumerator ActionWithDelay(float delay, Action action = null)
        {
            _notifyWindowHandler.OpenWindow(WindowType.ProccessOn);

            yield return new WaitForSeconds(delay);

            action?.Invoke();
            _notifyWindowHandler.CloseWindow(WindowType.ProccessOn);
        }
    }
}
