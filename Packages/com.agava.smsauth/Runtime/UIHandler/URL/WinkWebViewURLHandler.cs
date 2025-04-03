using System;
using UnityEngine;
using UnityEngine.Scripting;
using System.Collections.Generic;
using SmsAuthAPI.Program;
using System.Threading.Tasks;
using System.Collections;

namespace Agava.Wink
{
    [Serializable, Preserve]
    public class WinkWebViewURLHandler
    {
        private const string DefaultLink = "https://wink.ru/embed/buy?type=service&id=winkkids&auth_phone={AUTH_PHONE}&auth_app={app}";
        private const string PlayerPhonePattern = "AUTH_PHONE";
        private const string AppAuthenticatorPattern = "app";

        [SerializeField] private AppAuthenticator _appAuthenticator;
        [SerializeField] private string _remoteConfigName = "wink-website-subs";

        private string _phoneNumber = string.Empty;
        private string _correctLink = string.Empty;

        public IEnumerator Construct()
        {
            yield return new WaitUntil(() => SmsAuthApi.Initialized);

            Task<string> task = RemoteConfig.StringRemoteConfig(_remoteConfigName, string.Empty);
            yield return new WaitUntil(() => task.IsCompleted);

            string result = task.Result;

            if(string.IsNullOrEmpty(result) == false)
                _correctLink = result.Replace($"{{{AppAuthenticatorPattern}}}", _appAuthenticator.ToString());
            else
                _correctLink = DefaultLink.Replace($"{{{AppAuthenticatorPattern}}}", _appAuthenticator.ToString());
        }

        public void SetPhone(string phoneNumber) => _phoneNumber = phoneNumber;

        public void CheckAvailabilityURL()
        {
            if(_appAuthenticator == AppAuthenticator.None)
                throw new Exception("There is no link URL for the app!");
        }

        public string GetURL()
        {
            string url = _correctLink.Replace($"{{{PlayerPhonePattern}}}", _phoneNumber);

            return url;
        }
    }
}
