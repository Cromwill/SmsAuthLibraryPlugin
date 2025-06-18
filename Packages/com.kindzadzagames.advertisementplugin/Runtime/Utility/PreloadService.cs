using UnityEngine;
using Newtonsoft.Json;
using System.Collections;
using UnityEngine.Scripting;
using System.Threading.Tasks;
using UnityEngine.Networking;
using KinDzaDzaGames.AdvertisementPlugin.DTO;

namespace KinDzaDzaGames.AdvertisementPlugin.Utility
{
    [Preserve]
    public class PreloadService
    {
        private const string ControllerName = "AdsApp";
        private const string PayedSettingsRCName = "get-ads-sdk-settings";

        private readonly AdvertisementAPI _api;
        private readonly AppData _appData;
        private readonly int _bundlIdVersion;
        private readonly bool _isVip = true;
        private bool _isEndPrepare = false;

        public AdsSdkSettingsData Settings { get; private set; }

        public PreloadService(AdvertisementAPI api, int bundlIdVersion, bool vip, AppData appData)
        {
            _api = api;
            _isVip = vip;
            _appData = appData;
            _bundlIdVersion = bundlIdVersion;
        }

        public bool IsPluginAvailable { get; private set; } = false;

        public IEnumerator Preparing()
        {
            yield return new WaitUntil(() => _api.Initialized);
            yield return null;

            SetPluginAwailable();
            yield return new WaitUntil(() => _isEndPrepare);

            Debug.Log("#PreloadService# Prepare is done. Start plugin " + IsPluginAvailable);
        }

        private async void SetPluginAwailable()
        {
            IsPluginAvailable = await InitApp();

            _isEndPrepare = true;
        }

        private async Task<bool> InitApp()
        {
            Debug.Log($"#PreloadService in AdsAppView# Used bundle id version = {_bundlIdVersion}, in init app method.");
            string apiName = PayedSettingsRCName;

            RequestAdData data = new() { app_id = _appData.app_id, platform = _appData.platform, store_id = _appData.store_id, vip = _isVip };
            Response response = await _api.GetAppSettings(ControllerName, apiName, data);

            if (response.statusCode == UnityWebRequest.Result.Success)
            {
                if (string.IsNullOrEmpty(response.body))
                {
                    Debug.LogError($"#PreloadService# Fail to recieve remote settings '{apiName}': NULL");
                    return false;
                }
                else
                {
                    Settings = JsonConvert.DeserializeObject<AdsSdkSettingsData>(response.body);

                    Debug.Log($"#PreloadService# Advertisement Plugin settings: State - {Settings.released_state}, release - {Settings.released_version}, vip state - {Settings.vip_state}\n" +
                        $"---->Review: state - {Settings.review_state}, version - {Settings.review_version}\n" +
                        $"---->Review: first ad timer - {Settings.first_timer}, regular ad timer - {Settings.regular_timer}");

                    if (Settings.review_state && _bundlIdVersion == Settings.review_version)
                    {
                        return true;
                    }
                    else if (Settings.review_state == false && _bundlIdVersion == Settings.review_version)
                    {
                        return false;
                    }
                    else if (Settings.released_state && _bundlIdVersion <= Settings.released_version)
                    {
                        if (_isVip)
                            return Settings.vip_state;
                        else
                            return true;
                    }
                    else if (Settings.released_state == false && _bundlIdVersion <= Settings.released_version)
                    {
                        return false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                Debug.LogError($"#PreloadService# Fail to recieve remote settings '{apiName}': " + response.statusCode);
                return false;
            }
        }
    }
}
