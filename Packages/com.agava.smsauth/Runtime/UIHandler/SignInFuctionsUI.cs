using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using SmsAuthAPI.Program;
using UnityEngine.Scripting;

namespace Agava.Wink
{
    [Preserve]
    internal class SignInFuctionsUI
    {
        private const string RemoteName = "max-demo-minutes";
        private const string RemoteTempName = "demo-overtime-minutes";
        private const int MinutesFactor = 60;

        private readonly DemoTimer _demoTimer;
        private readonly WinkAccessManager _winkAccessManager;
        private readonly NotifyWindowHandler _notifyWindowHandler;

        private readonly ICoroutine _coroutine;
        private readonly IWinkSignInHandlerUI _winkSignInHandlerUI;

        private int _inputOtpCodeCount = 0;

        public SignInFuctionsUI(NotifyWindowHandler notifyWindowHandler, DemoTimer demoTimer, WinkAccessManager winkAccessManager,
            IWinkSignInHandlerUI winkSignInHandlerUI, ICoroutine coroutine)
        {
            _demoTimer = demoTimer;
            _coroutine = coroutine;
            _winkAccessManager = winkAccessManager;
            _notifyWindowHandler = notifyWindowHandler;
            _winkSignInHandlerUI = winkSignInHandlerUI;
        }

        internal void OnAppFocus(bool focus) => _demoTimer.OnAppFocus(focus);
        internal void Update() => _demoTimer?.Update();

        internal void OnSignInClicked(string phone, string appHash, bool skipRegistration = false)
        {
            _notifyWindowHandler.CloseWindow(WindowType.SignIn);
            _notifyWindowHandler.OpenWindow(WindowType.ProccessOn);

            AnalyticsWinkService.SendOnEnteredPhoneWindow();
            _notifyWindowHandler.ActivateOtpCodeSetter();

            _winkAccessManager.Regist(phoneNumber: phone, appHash: appHash,
            otpCodeRequest: (hasOtpCode) =>
            {
                OnOtpCodeRequested(phone, appHash, hasOtpCode);
            },
            otpCodeAccepted: (accepted) =>
            {
                OnOtpCodeAccepted(accepted);
            },
            onFail: () =>
            {
                _notifyWindowHandler.OpenWindow(WindowType.Fail);
                _notifyWindowHandler.CloseWindow(WindowType.ProccessOn);
            },
            skipRegistration);
        }

        internal void OnUnlinkClicked(string device)
        {
            _winkAccessManager.Unlink(device);
        }

        internal async void SetRemoteConfig()
        {
            await Task.Yield();

            var response = await SmsAuthApi.GetRemoteConfig(RemoteName);
            var responseTemp = await SmsAuthApi.GetRemoteConfig(RemoteTempName);

            if (response.statusCode == UnityWebRequest.Result.Success && responseTemp.statusCode == UnityWebRequest.Result.Success)
            {
                int seconds;
                int tempSeconds;

                if (string.IsNullOrEmpty(response.body))
                    seconds = 0;
                else
                    seconds = Convert.ToInt32(response.body) * MinutesFactor;

                if (string.IsNullOrEmpty(responseTemp.body))
                    tempSeconds = 0;
                else
                    tempSeconds = Convert.ToInt32(responseTemp.body) * MinutesFactor;

                _demoTimer.Construct(_winkAccessManager, seconds, tempSeconds, _winkSignInHandlerUI);
                _demoTimer.Start();
                _demoTimer.CheckOutTime();

                Debug.Log("Remote setted: " + response.body);
            }
            else
            {
                _demoTimer.Construct(_winkAccessManager, 0, 0, _winkSignInHandlerUI);
                _demoTimer.Start();
                _demoTimer.CheckOutTime();
                Debug.LogError("Fail to recieve remote config: " + response.statusCode);
            }
        }

        private void OnOtpCodeRequested(string phone, string appHash, bool hasOtpCode)
        {
            if (hasOtpCode)
            {
                AnalyticsWinkService.SendEnterOtpCodeWindow();

                _notifyWindowHandler.CloseWindow(WindowType.ProccessOn);
                _notifyWindowHandler.OpenInputOtpCodeWindow(phone, appHash,
                onInputDone: (code) =>
                {
                    _winkAccessManager.SendOtpCode(code);

                    if(_inputOtpCodeCount > 0)
                        AnalyticsWinkService.SendOnEnteredOtpCodeAgainWindow();
                    else
                        AnalyticsWinkService.SendOnEnteredOtpCodeWindow();

                    _inputOtpCodeCount++;
                },
                onBackClicked: () =>
                {
                    _notifyWindowHandler.CloseWindow(WindowType.EnterOtpCode);

                    if (_winkAccessManager.Authenficated == false)
                        _notifyWindowHandler.OpenSignInWindow();
                });
            }
            else
            {
                _notifyWindowHandler.OpenWindow(WindowType.Fail);
            }
        }

        private void OnOtpCodeAccepted(bool accepted) => _notifyWindowHandler.Response(accepted);

        internal void OnSignInSuccesfully(bool hasAccess)
        {
            if (hasAccess)
                _demoTimer.Stop();

            //_notifyWindowHandler.CloseWindow(WindowType.DemoTimerExpired);
            _notifyWindowHandler.CloseWindow(WindowType.SignIn);
        }

        internal void OnAuthorizationSuccessfully()
        {
            _demoTimer.Stop();
            //_notifyWindowHandler.CloseWindow(WindowType.DemoTimerExpired);
        }
    }
}
