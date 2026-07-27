using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using SmsAuthAPI.DTO;
using SmsAuthAPI.Program;
using System.Collections;
using UnityEngine.Scripting;
using System.Collections.Generic;
using KinDzaDzaGames.AdvertisementPlugin;
using PlayerPrefs = UnityEngine.PlayerPrefs;
using AdsAppView.Utility;

namespace Agava.Wink
{
    /// <summary>
    ///     Handler UI. Input data and view auth process.
    /// </summary>
    [Preserve]
    public class WinkSignInHandlerUI : MonoBehaviour, IWinkSignInHandlerUI, ICoroutine, IInterstitialBlocker, IBannerBlocker
    {
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
        [SerializeField] private Button _signInButton;
        [SerializeField] private Button _winkInfoButton;
        //[SerializeField] private Button[] _tryWinkButtons;
        [SerializeField] private Button[] _switchOrientationButtons;
        [SerializeField] private Button[] _openAdOfferButtons;
        [SerializeField] private Button[] _subscriptionCheckButtons;
        [SerializeField] private Button[] _closeAdOfferButtons;
        [SerializeField] private Button[] _closeAdInfoButtons;
        [SerializeField] private Button[] _closeButtonsFromSettings;
        [SerializeField] private Button[] _rewardButtons;
        [SerializeField] private Button _subscribeButtonRewardWindow;
        [SerializeField] private Button _closeSignInWindowButton;
        [Header("Analytics buttons")]
        [SerializeField] private AnalyticsSender _analyticsSender;
        [Header("Factory components")]
        [SerializeField] private UnlinkDeviceViewContainer _unlinkDeviceViewContainer;
        [Header("Placeholders")]
        [SerializeField] private TextPlaceholder[] _phoneNumberPlaceholders;
        [Header("WebView")]
        [SerializeField] private WebViewPresenter _webViewPresenter;
        [Header("SMS Retriever")]
        [SerializeField] private SmsRetrieverManager _smsRetrieverManager;

        private SignInFuctionsUI _signInFuctionsUI;
        private WinkAccessManager _winkAccessManager;
        private InterstitialPlayer _interstitialPlayer;
        private bool _loginFromSettings = false;
        private bool _useAdWindows = false;
        private bool _forcedChangeOrientation = false;
        private bool _tokenRefreshing = false;

        public static WinkSignInHandlerUI Instance { get; private set; }

        public bool IsAnyWindowEnabled => _notifyWindowHandler.IsAnyWindowEnabled;
        public bool InterstitialDisplayBlocked => IsAnyWindowEnabled;
        public bool BannerDisplayBlocked => IsAnyWindowEnabled;
        public AppAuthenticator AppAuthenticator => _webViewURLHandler.AppAuthenticator;

        public event Action AllWindowsClosed;
        public event Action DemoCompleted;

        public void Construct(BuildVersionHolder buildVersionHolder, AppMetricaInfo appMetricaInfo)
        {
            StartCoroutine(_webViewURLHandler.Construct());

            _webViewURLHandler.CheckAvailabilityURL();
            _smsRetrieverManager.Construct();
            _notifyWindowHandler.Construct(_gameOrientation, _webViewURLHandler, _demoTimer, _screenshotProtector, this, buildVersionHolder.StoreName.ToString(), appMetricaInfo, _smsRetrieverManager);
            _notifyWindowHandler.OpenWindow(WindowType.ProccessOn);
            Links.Instance.SetAppInfo(buildVersionHolder, _webViewURLHandler.AppAuthenticator);
        }

        private void OnApplicationFocus(bool focus) => _signInFuctionsUI?.OnAppFocus(focus);

        private void Update() => _signInFuctionsUI?.Update();

        public void Dispose()
        {
            if (_signInFuctionsUI == null) return;

            _enterCodeContinueButton.onClick.RemoveListener(OnEnterCodeContinueClicked);
            _signInContinueButton.onClick.RemoveListener(OnSignInContinueClicked);
            _signInButton.onClick.RemoveListener(OpenSignWindow);
            _winkInfoButton.onClick.RemoveListener(OpenWinkInfoWindow);

            /*foreach (var button in _signInButtons)
                button.onClick.RemoveListener(OpenSignWindow);

            foreach (var button in _tryWinkButtons)
                button.onClick.RemoveListener(OpenSignWindow);*/

            foreach (var button in _switchOrientationButtons)
                button.onClick.RemoveListener(OpenChangeOrientationWindow);

            foreach (var button in _openAdOfferButtons)
                button.onClick.RemoveListener(OpenAdOfferWindow);

            foreach (var button in _subscriptionCheckButtons)
                button.onClick.RemoveListener(CheckSubscription);

            foreach (var button in _closeAdOfferButtons)
                button.onClick.RemoveListener(CloseAdOfferWindow);

            foreach (var button in _closeAdInfoButtons)
                button.onClick.RemoveListener(CloseAdInfoWindow);

            foreach (var button in _closeButtonsFromSettings)
                button.onClick.RemoveListener(ContinueGame);

            foreach (var button in _rewardButtons)
                button.onClick.RemoveListener(OpenRewardWindow);

            _subscribeButtonRewardWindow.onClick.RemoveListener(RedirectToSubscribe);
            _closeSignInWindowButton.onClick.RemoveListener(CloseSignInWindow);

            _unlinkDeviceViewContainer.DeviceRemoved -= OnUnlinkButtonClicked;

            if (_winkAccessManager == null) return;

            _winkAccessManager.ResetLogin -= OpenSignWindow;
            _winkAccessManager.LimitReached -= OnLimitReached;
            _winkAccessManager.SignInSuccessfully -= OnSignInSuccessfully;
            _winkAccessManager.AuthorizationSuccessfully -= OnAuthorizationSuccessfully;
            _demoTimer.TimerExpired -= OnTimerExpired;
            _demoTimer.FirstChecked -= OnTimerFirstChecked;
            _demoTimer.Dispose();
            _notifyWindowHandler.SunbscriptionBuyed -= OnSunbscriptionBuyed;
            _notifyWindowHandler.WebViewClosed -= TurnOffAdMode;
            _notifyWindowHandler.Dispose();
            _analyticsSender.Dispose();

            if (_interstitialPlayer != null)
                _interstitialPlayer.OpenAdOffer -= OpenTurnOffAdPanel;
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

            RewardContinueWindowPresenter rewardConfigs = FindObjectOfType<RewardContinueWindowPresenter>();

            while (rewardConfigs.Initialized == false)
                yield return null;

            _notifyWindowHandler.ApplyTurnOffABTests();
        }

        public void OpenProcessOnWindow() => _notifyWindowHandler.OpenWindow(WindowType.ProccessOn);
        public void CloseProcessOnWindow() => _notifyWindowHandler.CloseWindow(WindowType.ProccessOn);

        public void DownloadRemoteSettings()
        {
            StartCoroutine(EnternetChecking());
            _signInFuctionsUI.SetRemoteConfig();
        }

        public void StartService(WinkAccessManager winkAccessManager, InterstitialPlayer interstitialPlayer)
        {
            if (Instance == null)
                Instance = this;

            _signInFuctionsUI = new(_notifyWindowHandler, _demoTimer, winkAccessManager, this, this);
            _winkAccessManager = winkAccessManager;
            _interstitialPlayer = interstitialPlayer;
            _analyticsSender.Construct();

            _enterCodeContinueButton.onClick.AddListener(OnEnterCodeContinueClicked);
            _signInContinueButton.onClick.AddListener(OnSignInContinueClicked);
            _signInButton.onClick.AddListener(OpenSignWindow);
            _winkInfoButton.onClick.AddListener(OpenWinkInfoWindow);

            /*foreach (var button in _signInButtons)
                button.onClick.AddListener(OpenSignWindow);

            foreach (var button in _tryWinkButtons)
                button.onClick.AddListener(OpenSignWindow);*/

            foreach (var button in _switchOrientationButtons)
                button.onClick.AddListener(OpenChangeOrientationWindow);

            foreach (var button in _openAdOfferButtons)
                button.onClick.AddListener(OpenAdOfferWindow);

            foreach (var button in _subscriptionCheckButtons)
                button.onClick.AddListener(CheckSubscription);

            foreach (var button in _closeAdOfferButtons)
                button.onClick.AddListener(CloseAdOfferWindow);

            foreach (var button in _closeAdInfoButtons)
                button.onClick.AddListener(CloseAdInfoWindow);

            foreach (var button in _closeButtonsFromSettings)
                button.onClick.AddListener(ContinueGame);

            foreach (var button in _rewardButtons)
                button.onClick.AddListener(OpenRewardWindow);

            _subscribeButtonRewardWindow.onClick.AddListener(RedirectToSubscribe);
            _closeSignInWindowButton.onClick.AddListener(CloseSignInWindow);

            _unlinkDeviceViewContainer.DeviceRemoved += OnUnlinkButtonClicked;

            CloseAllWindows();

            _winkAccessManager.ResetLogin += OpenSignWindow;
            _winkAccessManager.LimitReached += OnLimitReached;
            _winkAccessManager.SignInSuccessfully += OnSignInSuccessfully;
            _winkAccessManager.AuthorizationSuccessfully += OnAuthorizationSuccessfully;
            _demoTimer.TimerExpired += OnTimerExpired;
            _demoTimer.FirstChecked += OnTimerFirstChecked;
            _notifyWindowHandler.SunbscriptionBuyed += OnSunbscriptionBuyed;
            _notifyWindowHandler.WebViewClosed += TurnOffAdMode;

            if(_interstitialPlayer != null)
                _interstitialPlayer.OpenAdOffer += OpenTurnOffAdPanel;
        }

        public void OpenStartWindow() => OpenSubscriptionWindow();

        public void OpenSignWindow()
        {
            _notifyWindowHandler.OpenSignInWindow();
            AnalyticsWinkService.SendEnterPhoneWindow();
        }

        private void OpenWinkInfoWindow()
        {
            _useAdWindows = false;
            _analyticsSender.SetAdInfo(adEvents: _useAdWindows);
            _notifyWindowHandler.EnableOriginalOfferInfo(enableClose: _demoTimer.Expired == false);
            _notifyWindowHandler.SetAdOption(adOption: _useAdWindows);
            _notifyWindowHandler.OpenWindow(WindowType.WinkInfoVertical);
            _notifyWindowHandler.CloseWindow(WindowType.HelloWOAccess);
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
            _loginFromSettings = true;

            if (_winkAccessManager.Authenficated)
            {
                AnalyticsWinkService.SendSubscribeButtonClickOnSettings();

                if (_winkAccessManager.HasAccess || _winkAccessManager.HasTempAccess)
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

            AdvertisementController.Instance?.AddInterstitialBlocker(this);
            AdvertisementController.Instance?.SuspendDisplayBanner(this);
        }

        private void ContinueGame()
        {
            if (_loginFromSettings)
            {
                _screenshotProtector.TryEnableScreenshots();
                _loginFromSettings = false;

                if (_gameOrientation.NeedChangeOrientation)
                {
                    _gameOrientation.SetLandscapeOrientationPosibility();
                    _gameOrientation.SetSavedOrientation();
                }
            }
        }

        private void OpenRewardWindow()
        {
            _notifyWindowHandler.OpenRewardWindow();
            //_notifyWindowHandler.CloseWindow(WindowType.DemoTimerExpired);
        }

        private void RedirectToSubscribe()
        {
            if (_winkAccessManager.Authenficated)
            {
                _notifyWindowHandler.OpenWindow(WindowType.SubscriptionCheck);
            }
            else
            {
                _notifyWindowHandler.OpenWindow(WindowType.Redirect);
                OpenSignWindow();
            }

            _notifyWindowHandler.CloseWindow(WindowType.RewardContinue);
        }

        public void OnDeleteAccountButtonClick()
        {
            _screenshotProtector.TryDisableScreenshots();
            _loginFromSettings = true;
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
                            _interstitialPlayer?.Continue();

                            if (resultSuccess == false)
                            {
                                _notifyWindowHandler.OpenWindow(WindowType.Fail);

                                if (_tokenRefreshing == false)
                                    StartCoroutine(RefreshFailedToken());
                            }
                            else
                            {
                                _loginFromSettings = false;
                                _notifyWindowHandler.OpenWindow(WindowType.Redirect);

                                //ContinueGame();
                            }
                        });
                    })));

            AdvertisementController.Instance?.AddInterstitialBlocker(this);
            AdvertisementController.Instance?.SuspendDisplayBanner(this);
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

        public void OpenTurnOffAdPanel()
        {
            _useAdWindows = true;
            _gameOrientation.SaveGameOrientation();
            _screenshotProtector.TryDisableScreenshots();
            _interstitialPlayer?.Suspend();
            _analyticsSender.SetAdInfo(adEvents: _useAdWindows);
            _notifyWindowHandler.EnableAdOfferInfo();

            StartCoroutine(ActionWithDelay(ChangeOrientationDelay, () => _notifyWindowHandler.OpenWindow(_gameOrientation.NeedChangeOrientation ? WindowType.TurnOffAdHorizontal : WindowType.TurnOffAdVertical)));

            AdvertisementController.Instance?.SuspendDisplayBanner(this);
            AdvertisementController.Instance?.AddInterstitialBlocker(this);
        }

        private void OpenAdOfferWindow()
        {
            _notifyWindowHandler.OpenAdOffOffer(isHorizontal: _gameOrientation.NeedChangeOrientation);
            _notifyWindowHandler.CloseWindow(_gameOrientation.NeedChangeOrientation ? WindowType.TurnOffAdHorizontal : WindowType.TurnOffAdVertical);
        }

        private void CloseAdOfferWindow()
        {
            if (_useAdWindows)
            {
                TurnOffAdMode();
            }
            else
            {
                OpenChangeOrientationWindow();
                //_notifyWindowHandler.OpenHelloWindowWOAccess();
                _notifyWindowHandler.CloseWindow(WindowType.WinkInfoVertical);
            }
        }

        private void CloseAdInfoWindow()
        {
            if (_useAdWindows)
            {
                TurnOffAdMode();
            }
            else
            {
                OpenChangeOrientationWindow();
                _notifyWindowHandler.CloseWindow(WindowType.WinkInfoVertical);
            }
        }

        private void TurnOffAdMode()
        {
            _useAdWindows = false;
            _analyticsSender.SetAdInfo(adEvents: _useAdWindows);
            _notifyWindowHandler.CloseAdOffer();
            _interstitialPlayer?.Continue();
            _screenshotProtector.TryEnableScreenshots();

            if (_forcedChangeOrientation && _gameOrientation.NeedChangeOrientation)
            {
                _forcedChangeOrientation = false;
                _gameOrientation.SetLandscapeOrientationPosibility();
                _gameOrientation.SetSavedOrientation();
            }
        }

        private void OnSignInContinueClicked()
        {
            _smsRetrieverManager.ReloadRetriever();
            string number = _numbersInputField.Number;
            string formattedNumber = PhoneNumber.FormatNumber(number);

            foreach (TextPlaceholder placeholder in _phoneNumberPlaceholders)
                placeholder.ReplaceValue(formattedNumber);

            _signInFuctionsUI.OnSignInClicked(number, _smsRetrieverManager.HashCode, _notifyWindowHandler.ZeroSecondsCodeTimer == false);
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

        private void OnSignInSuccessfully(bool hasAccess, bool hasTempAccess)
        {
            _screenshotProtector.TryDisableScreenshots();
            _numbersInputField.Clear();
            _signInFuctionsUI.OnSignInSuccesfully(hasAccess);

            SetPhone();
            _webViewURLHandler.SetPhone(_winkAccessManager.LoginData.phone);
            _notifyWindowHandler.CloseWindow(WindowType.Redirect);
            _notifyWindowHandler.OpenHelloWindow(hasAccess || (hasTempAccess && _demoTimer.Expired == false));
        }

        private void SetPhone()
        {
            string number = "N/A";

            if (PlayerPrefs.HasKey(_winkAccessManager.PhoneNumber))
                number = PhoneNumber.FormatNumber(PlayerPrefs.GetString(_winkAccessManager.PhoneNumber));

            foreach (TextPlaceholder placeholder in _phoneNumberPlaceholders)
                placeholder.ReplaceValue(number);
        }

        private void CloseSignInWindow()
        {
            if (_useAdWindows)
            {
                _forcedChangeOrientation = false;

                if (_gameOrientation.NeedChangeOrientation)
                {
                    StartCoroutine(ActionWithDelay(ChangeOrientationDelay, () => TurnOffAdMode()));
                    _gameOrientation.SetLandscapeOrientationPosibility();
                    _gameOrientation.SetSavedOrientation();
                }
                else
                {
                    TurnOffAdMode();
                }
            }

            _notifyWindowHandler.CloseWindow(WindowType.SignIn);
        }

        private void CheckSubscription()
        {
            if (_winkAccessManager.Authenficated)
            {
                _notifyWindowHandler.OpenWindow(WindowType.SubscriptionCheck);
                _notifyWindowHandler.CloseWindow(WindowType.WinkInfoVertical);

                if (_useAdWindows)
                {
                    _notifyWindowHandler.CloseAdOffer();
                    _notifyWindowHandler.SetAdOption(adOption: true);

                    if (_gameOrientation.NeedChangeOrientation && _gameOrientation.ChangedToLandscape)
                    {
                        _forcedChangeOrientation = true;
                        _gameOrientation.SaveGameOrientation();
                        _gameOrientation.SetPortraitOrientation();
                    }
                }
            }
            else
            {
                if (_gameOrientation.NeedChangeOrientation)
                {
                    _forcedChangeOrientation = true;

                    StartCoroutine(ActionWithDelay(ChangeOrientationDelay, () =>
                    {
                        _notifyWindowHandler.OpenSignInWindow();
                        _notifyWindowHandler.CloseAdOffer();
                        _notifyWindowHandler.SetAdOption(adOption: true);
                    }));

                    _gameOrientation.SaveGameOrientation();
                    _gameOrientation.SetPortraitOrientation();
                }
                else
                {
                    _notifyWindowHandler.OpenSignInWindow();
                    _notifyWindowHandler.CloseAdOffer();
                    _notifyWindowHandler.SetAdOption(adOption: true);
                }
            }
        }

        private async void OnTimerExpired()
        {
            if(_winkAccessManager.Authenficated)
            {
                bool hasSubsc = await _winkAccessManager.CheckSubscription();

                if (hasSubsc)
                    return;
            }

            if (_gameOrientation.NeedChangeOrientation)
                _gameOrientation.SetPortraitOrientation();

            _notifyWindowHandler.ChangeDemoModeOption(enabled: false);

            if (AdvertisementController.Instance != null && AdvertisementController.Instance.CanShowReward())
            {
                _notifyWindowHandler.OpenWindow(WindowType.RewardContinue);
            }
            else
            {
                SetPhone();
                _notifyWindowHandler.OpenHelloWindowWOAccess();

                /*if (_winkAccessManager.Authenficated)
                {
                    SetPhone();
                    _notifyWindowHandler.OpenHelloWindowWOAccess();
                }
                else
                {
                    AnalyticsWinkService.SendSubscribeOfferWindow();
                    _notifyWindowHandler.OpenDemoExpiredWindow(false);
                }*/
            }

            DemoCompleted?.Invoke();
            _screenshotProtector.TryDisableScreenshots();
            AdvertisementController.Instance?.AddInterstitialBlocker(this);
            AdvertisementController.Instance?.SuspendDisplayBanner(this);
        }

        private void OnTimerFirstChecked() => _notifyWindowHandler.ChangeDemoModeOption(enabled: _demoTimer.Expired == false);

        private void OnSunbscriptionBuyed()
        {
            _forcedChangeOrientation = false;

            if (_useAdWindows)
                TurnOffAdMode();

            _demoTimer.AddTempSubsDemoTime();
            _notifyWindowHandler.ChangeDemoModeOption(enabled: _demoTimer.Expired == false);
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

        private IEnumerator RefreshFailedToken()
        {
            _tokenRefreshing = true;

            yield return SmsAuthApi.Refresh(TokenLifeHelper.GetTokens().refresh);

            _tokenRefreshing = false;
        }

        public void RemoveRestriction() { }
    }
}
