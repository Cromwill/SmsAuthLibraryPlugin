using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Scripting;
using SmsAuthAPI.Program;
using Newtonsoft.Json;
using SmsAuthAPI.DTO;
using AdsAppView.Utility;

namespace Agava.Wink
{
    [Preserve]
    public class PreloadService
    {
        private const string True = "true";
        private const string On = "on";
#if UNITY_WEBGL
        private const string Platform = "webgl";
#elif UNITY_STANDALONE
        private const string Platform = "standalone";
#elif UNITY_ANDROID
        private const string Platform = "Android";
#elif UNITY_IOS
        private const string Platform = "iOS";
#endif
        private readonly WinkSignInHandlerUI _winkSignInHandlerUI;
        private readonly Store _storeName;
        private int _bundlIdVersion;
        private bool _isEndPrepare = false;

        public PreloadService(WinkSignInHandlerUI winkSignInHandlerUI, int bundlIdVersion, Store storeName)
        {
            Instance ??= this;
            _bundlIdVersion = bundlIdVersion;
            _winkSignInHandlerUI = winkSignInHandlerUI;
            _storeName = storeName;
        }

        public static PreloadService Instance { get; private set; }
        public bool IsPluginAwailable { get; private set; } = false;

        public IEnumerator Preparing()
        {
            yield return _winkSignInHandlerUI.Initialize();
            yield return new WaitUntil(() => SmsAuthApi.Initialized);
            yield return null;

            SetPluginAwailable();
            yield return new WaitUntil(() => _isEndPrepare);
#if UNITY_EDITOR || TEST
            Debug.Log("Prepare is done. Start plugin " + IsPluginAwailable);
#endif
        }

        private async void SetPluginAwailable()
        {
            Debug.Log($"#PreloadService in Agava.Wink# Used bundle id version = {_bundlIdVersion}, in set plugin awailable method.");
            string remoteName = $"{Application.identifier}/{Platform}/{_storeName}";
            var response = await SmsAuthApi.GetPluginSettings(remoteName);

            if (response.statusCode == UnityWebRequest.Result.Success)
            {
                if (string.IsNullOrEmpty(response.body))
                {
                    IsPluginAwailable = false;
                    Debug.LogError($"Fail to recieve remote config '{remoteName}': NULL");
                }
                else
                {
                    PluginSettings remotePluginSettings = JsonConvert.DeserializeObject<PluginSettings>(response.body);
                    Debug.Log($"Plugin settings: State - {remotePluginSettings.plugin_state}, release - {remotePluginSettings.released_version}\n" +
                        $"Test state - {remotePluginSettings.test_review},  review - {remotePluginSettings.review_version}");

                    if (remotePluginSettings.test_review == True && _bundlIdVersion == remotePluginSettings.review_version)
                        IsPluginAwailable = true;
                    else if (remotePluginSettings.test_review != True && _bundlIdVersion == remotePluginSettings.review_version)
                        IsPluginAwailable = false;
                    else if (remotePluginSettings.plugin_state == On && _bundlIdVersion <= remotePluginSettings.released_version)
                        IsPluginAwailable = true;
                    else if(remotePluginSettings.plugin_state != On && _bundlIdVersion <= remotePluginSettings.released_version)
                        IsPluginAwailable = false;
                    else
                        IsPluginAwailable = false;
                }
            }
            else
            {
                IsPluginAwailable = false;
                Debug.LogError($"Fail to recieve remote config '{remoteName}': " + response.statusCode);
            }

            _isEndPrepare = true;
        }
    }
}
