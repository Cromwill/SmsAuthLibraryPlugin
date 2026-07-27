using System;
using UnityEngine;
using Io.AppMetrica;
using SmsAuthAPI.DTO;
using UnityEngine.Scripting;
using System.Collections.Generic;

namespace Agava.Wink
{
    [Preserve]
    public class MetrikaDuplicator
    {
        private WinkAccessManager _winkAccessManager;
        private LoginData _loginData;
        private string _sanId = string.Empty;
        private string _platform = string.Empty;
        private string _version = string.Empty;

        public string AppmetricaDeviceId { get; private set; } = string.Empty;

        public bool AppmetricaDeviceIdGetted { get; private set; } = false;

        public MetrikaDuplicator(WinkAccessManager winkAccessManager)
        {
            _winkAccessManager = winkAccessManager;
            GetAppmetricaID();
            DateTime.UtcNow.ToString();

            AnalyticsWinkService.EventSended += OnEventSended;
        }

        public void Dispose()
        {
            AnalyticsWinkService.EventSended -= OnEventSended;
        }

        public void SetPlatformAndVersion(string platform, string version)
        {
            _platform = platform;
            _version = version;
        }

        public void SetLoginData(LoginData loginData) => _loginData = loginData;
        public void SetdSanId(string sanId) => _sanId = sanId;

        private void OnEventSended(string eventName, string eventBody)
        {
            if(_loginData != null)
                _winkAccessManager.SendAnalyticsToBack(eventName, _loginData.phone, _loginData.device_id, _sanId, DateTime.UtcNow, _platform, _version, AppmetricaDeviceId, eventBody);
            else
                _winkAccessManager.SendAnalyticsToBack(eventName, string.Empty, string.Empty, _sanId, DateTime.UtcNow, _platform, _version, AppmetricaDeviceId, eventBody);

#if UNITY_EDITOR
            Debug.Log($"METRIKA DUPLICATOR: send event = {eventName}, event body = {eventBody}");
#endif
        }

        private void GetAppmetricaID()
        {
            IEnumerable<string> keys = new string[] { StartupParamsKey.AppMetricaDeviceIDHash };

            AppMetrica.RequestStartupParams(StartupParamsDelegateStartupParamsDelegate, keys);

            void StartupParamsDelegateStartupParamsDelegate(StartupParamsResult result, StartupParamsErrorReason errorReason)
            {
                if (errorReason != null)
                    Debug.LogError("Appmetrica ERROR reason: " + errorReason);

                AppmetricaDeviceId = result.DeviceIdHash;
                AppmetricaDeviceIdGetted = true;
            }
        }
    }
}
