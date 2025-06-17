using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SmsAuthAPI.DTO;
using SmsAuthAPI.Program;
using UnityEngine.Scripting;
using System.Threading.Tasks;
using KinDzaDzaGames.AdvertisementPlugin;

namespace Agava.Wink
{
    /// <summary>
    ///     Auth process logic.
    /// </summary>
    [Preserve]
    public class WinkAccessManager : MonoBehaviour, IWinkAccessManager, ICoroutine
    {
        private const string Platform =
#if UNITY_ANDROID
            "android";
#elif UNITY_IOS
            "ios";
#elif UNITY_WEBGL
            "webgl";
#else
            "editor";
#endif

        private const string StartDataSend = nameof(StartDataSend);
        private const string FirstRegist = nameof(FirstRegist);
        private const string UniqueId = nameof(UniqueId);

        [SerializeField] private string _ip;
        [SerializeField] private string _additiveId;

#if UNITY_WEBGL
        [Header("WEBGL")]
        [SerializeField] private string _appId;
#endif

        private RequestHandler _requestHandler;
        private TimespentService _timespentService;
        private SubscriptionSearchSystem _subscribeSearchSystem;
        private Action<bool, bool> _winkSubscriptionAccessRequest;
        private Action<bool> _otpCodeAccepted;
        private string _uniqueId;
        private string _sanId = null;
        private DateTime _lastTimeSendAnalytics = DateTime.Now;
        private Coroutine _sendEventCoroutine;
        private bool _subscriptionEventSent = false;

        public readonly string PhoneNumber = nameof(PhoneNumber);
        public readonly string SanId = nameof(SanId);

        public LoginData LoginData { get; private set; }
        public bool Authenficated { get; private set; } = false;
        public bool HasAccess { get; private set; } = false;
        public bool HasTempAccess { get; private set; } = false;

#if UNITY_ANDROID || UNITY_IOS
        public string AppId => Application.identifier;
#elif UNITY_WEBGL
        public string AppId => _appId;
#endif

        public static WinkAccessManager Instance { get; private set; }

        public event Action<IReadOnlyList<string>> LimitReached;
        public event Action ResetLogin;
        public event Action<bool, bool> SignInSuccessfully;
        public event Action AuthorizationSuccessfully;
        public event Action AccountDeleted;

        private void OnApplicationFocus(bool focus)
        {
            if (focus == false && _timespentService != null)
                _timespentService.OnAppFocusFalse();
            else if (focus && _timespentService != null)
                _timespentService.OnStartedApp();
        }

        private void OnDestroy()
        {
            _subscribeSearchSystem?.Stop();
        }

        public void Initialize()
        {
            if (SmsAuthApi.Initialized == false)
                SmsAuthApi.Initialize(_ip, AppId);

            if (Instance == null)
                Instance = this;
        }

        public IEnumerator Construct()
        {
            _requestHandler = new();
            DeeplinkHandler deeplink = new();
            deeplink.Init();

            DontDestroyOnLoad(this);

            if (UnityEngine.PlayerPrefs.HasKey(UniqueId) == false)
                _uniqueId = SystemInfo.deviceUniqueIdentifier + _additiveId;
            else
                _uniqueId = UnityEngine.PlayerPrefs.GetString(UniqueId);

            if (UnityEngine.PlayerPrefs.HasKey(PhoneNumber))
                LoginData = new LoginData() { phone = UnityEngine.PlayerPrefs.GetString(PhoneNumber), device_id = _uniqueId, app_id = AppId };

            if (LoginData != null)
                StartTimespentAnalytics();

            yield return null;

            StartCoroutine(DelayedSendStatistic());

            if (UnityEngine.PlayerPrefs.HasKey(StartDataSend) == false)
                SendStartData(string.Empty);
        }

        public async void SendStartData(string phone)
        {
            await _requestHandler.SendStartData(SystemInfo.deviceName, phone, DateTime.UtcNow);
            PlayerPrefs.SetString(StartDataSend, "send");
        }

        public IEnumerator TryQuickAccess()
        {
            if (UnityEngine.PlayerPrefs.HasKey(TokenLifeHelper.Tokens))
            {
                Task task = _requestHandler.QuickAccess(LoginData.phone, ResetLogin, null, OnSignInSuccessfully);
                yield return new WaitUntil(() => task.IsCompleted);
            }
            else
            {
                yield return null;
            }
        }

        public void SendOtpCode(string enteredOtpCode)
        {
            LoginData.otp_code = enteredOtpCode;
            Login(LoginData);
        }

        public async void Regist(string phoneNumber, Action<bool> otpCodeRequest, Action<bool> otpCodeAccepted, Action onFail = null, bool skipRegistration = false)
        {
            _winkSubscriptionAccessRequest = OnSignInSuccessfully;
            _otpCodeAccepted = otpCodeAccepted;
            UnityEngine.PlayerPrefs.SetString(PhoneNumber, phoneNumber);
            LoginData = await _requestHandler.Regist(phoneNumber, _uniqueId, AppId, otpCodeRequest, skipRegistration);

            if (LoginData == null)
                onFail?.Invoke();

            if (_timespentService == null)
                StartTimespentAnalytics();
        }

        public void Unlink(string deviceId, Action onUnlinkDevice = null) => _requestHandler.Unlink(new UnlinkData() { device_id = deviceId, app_id = AppId }, onUnlinkDevice);

        public void Login()
        {
            if (LoginData == null)
            {
                Debug.Log("[WinkAccessManager] Login data is null");
                return;
            }

            Login(LoginData);
        }

#if UNITY_EDITOR || TEST
        public void TestEnableSubsription()
        {
            HasAccess = true;
            HasTempAccess = true;
            Authenficated = true;
            AuthorizationSuccessfully?.Invoke();
            Debug.Log("Test Access succesfully. No cloud saves");
        }
#endif

        public async Task<bool> CheckSubscription()
        {
            bool hasSubs = await _requestHandler.CheckSubscription(LoginData.phone);

            if (hasSubs)
            {
                TrySendAnalyticsDataByNewUser(LoginData.phone);
                OnSubscriptionExist();
            }

            return hasSubs;
        }

        public async void ActivateTempSubscription()
        {
             await _requestHandler.ActivateTempSubscription(LoginData.phone);
            HasTempAccess = true;
        }

        private void Login(LoginData data) => _requestHandler.Login(data, LimitReached, _winkSubscriptionAccessRequest, _otpCodeAccepted);

        public void DeleteAccount(Action<bool> onComplete)
        {
            if (_timespentService != null)
            {
                _timespentService.OnAppFocusFalse();
                _timespentService = null;
            }

            _requestHandler.UnlinkDevices(AppId, _uniqueId,
                onUnlink: () =>
                {
                    _requestHandler.DeleteAccount(SignOut);
                    onComplete?.Invoke(true);
                },
                onTokensNull: () =>
                {
                    SignOut();
                    onComplete?.Invoke(true);
                },
                onFail: () =>
                {
                    onComplete?.Invoke(false);
                });

            void SignOut()
            {
                HasAccess = false;
                HasTempAccess = false;
                Authenficated = false;
                AccountDeleted?.Invoke();
                AdsAppView.Program.PopupManager.Instance.AccoundDeleted();
                AdvertisementController.Instance?.ChangeSubscribeStatus(false);
                TokenLifeHelper.ClearTokens();
            }
        }

        private void OnSignInSuccessfully(bool hasAccess, bool hasTempAccess)
        {
            Authenficated = true;
            SignInSuccessfully?.Invoke(hasAccess, hasTempAccess);
            SearchSubscription(LoginData.phone);
            Debug.Log("Authentication successfully");

            if (hasAccess)
            {
                TrySendAnalyticsDataByNewUser(LoginData.phone);
                OnSubscriptionExist();
            }
            else
            {
                if (hasTempAccess)
                {
                    HasTempAccess = true;
                    AdsAppView.Program.PopupManager.Instance.OnSubscribeDetected();
                    AdvertisementController.Instance?.ChangeSubscribeStatus(HasTempAccess);
                }
            }
        }

        private void OnSubscriptionExist()
        {
            _subscribeSearchSystem?.Stop();
            SendEventSubscriberData();

            HasAccess = true;
            HasTempAccess = true;
            AuthorizationSuccessfully?.Invoke();
            SendStartData(LoginData.phone);
            AdsAppView.Program.PopupManager.Instance?.OnSubscribeDetected();
            AdvertisementController.Instance?.ChangeSubscribeStatus(HasAccess);

            Debug.Log("Wink access successfully");
        }

        private void SearchSubscription(string phone)
        {
            if (_subscribeSearchSystem != null)
                return;

            _subscribeSearchSystem = new(phone);
            _subscribeSearchSystem.StartSearching(onSubscriptionExist: () =>
            {
                TrySendAnalyticsDataByNewUser(LoginData.phone);
                OnSubscriptionExist();
            });
        }

        private async void TrySendAnalyticsDataByNewUser(string phone)
        {
            if (PlayerPrefs.HasKey(FirstRegist) == false)
            {
                var responseActiveAccount = await SmsAuthApi.HasActiveAccount(phone);

                if (responseActiveAccount.statusCode == UnityEngine.Networking.UnityWebRequest.Result.Success)
                {
                    AnalyticsWinkService.SendFirstOpen();

                    var responseGetSanId = await SmsAuthApi.GetSanId(phone);

                    if (responseGetSanId.statusCode == UnityEngine.Networking.UnityWebRequest.Result.Success)
                    {
                        _sanId = responseGetSanId.body;

                        AnalyticsWinkService.SendSanId(_sanId);
                        SmsAuthApi.OnUserAddApp(LoginData.phone, _sanId, AppId);

                        PlayerPrefs.SetString(FirstRegist, "done");
                    }
                }
            }
        }

        private void StartTimespentAnalytics()
        {
            StartCoroutine(WaitForData());

            IEnumerator WaitForData()
            {
                yield return new WaitUntil(() => LoginData != null);

                _timespentService = new(this, LoginData.phone, _uniqueId, AppId);
                _timespentService.OnStartedApp();
            }
        }

        private IEnumerator DelayedSendStatistic()
        {
            yield return new WaitForEndOfFrame();

            if (_subscriptionEventSent && (HasAccess || HasTempAccess) && DateTime.Now.Day != _lastTimeSendAnalytics.Day && _sendEventCoroutine == null)
            {
                SendEventSubscriberData();
            }
        }

        private void SendEventSubscriberData()
        {
#if UNITY_EDITOR
            return;
#endif

            if (_sendEventCoroutine == null)
                _sendEventCoroutine = StartCoroutine(WaitForSanId());

            IEnumerator WaitForSanId()
            {
                if (string.IsNullOrEmpty(_sanId))
                {
                    Task<Response> task = SmsAuthApi.GetSanId(LoginData.phone);

                    yield return new WaitUntil(() => task.IsCompleted);

                    if (task.Result.statusCode == UnityEngine.Networking.UnityWebRequest.Result.Success)
                    {
                        _sanId = task.Result.body;
                    }
                    else
                    {
                        Debug.Log("#WinkAccessManager# sanId is null!");
                        _sendEventCoroutine = null;
                        yield return null;
                    }
                }

                _lastTimeSendAnalytics = DateTime.Now;
                SmsAuthApi.SendEventSubscriberData(_sanId, LoginData.phone, DateTime.UtcNow.ToString(), AppId, Application.version, Platform);
                _sendEventCoroutine = null;
                _subscriptionEventSent = true;
            }
        }
    }
}
