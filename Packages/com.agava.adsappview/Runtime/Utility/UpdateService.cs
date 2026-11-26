using UnityEngine;
using AdsAppView.DTO;
using Newtonsoft.Json;
using System.Collections;
using UnityEngine.Networking;

namespace AdsAppView.Utility
{
    public class UpdateService
    {
        private readonly AdsAppAPI _api;
        private readonly int _bundlIdVersion;
        private readonly Store _storeName;

        private bool _isEndPrepare = false;

        public bool NeedUpdateApp { get; private set; } = false;
        public string AppLinkInStore { get; private set; }

        public UpdateService(AdsAppAPI api, int bundlIdVersion, Store storeName)
        {
            _api = api;
            _bundlIdVersion = bundlIdVersion;
            _storeName = storeName;
        }

        public IEnumerator Preparing()
        {
            GetUpdateAppInfo();
            yield return new WaitUntil(() => _isEndPrepare);
        }

        public async void GetUpdateAppInfo()
        {
            string json = JsonConvert.SerializeObject(new UpdateAppInfo
            {
                app_id = Application.identifier,
                store_id = _storeName.ToString(),
            });

            var updateInfoResponse = await _api.GetUpdatePopupInfo(json);

            if (updateInfoResponse.statusCode == UnityWebRequest.Result.Success)
            {
                if (string.IsNullOrEmpty(updateInfoResponse.body))
                {
                    Debug.LogError($"<color=yellow>Update service:</color> Fail to recieve app link: is Null");
                    NeedUpdateApp = false;
                }
                else
                {
                    UpdateAppInfo appLinks = JsonConvert.DeserializeObject<UpdateAppInfo>(updateInfoResponse.body);
#if UNITY_EDITOR
                    Debug.Log($"<color=yellow>Update service:</color> AppId - {appLinks.app_id}, store_id - {appLinks.store_id}\n" +
                        $"update_bundle - {appLinks.update_bundle},  store_link - {appLinks.store_link}");
#endif
                    AppLinkInStore = appLinks.store_link;
                    NeedUpdateApp = _bundlIdVersion < appLinks.update_bundle;
                }
            }
            else
            {
                Debug.LogError($"<color=yellow>Update service:</color> Fail to recieve app link: " + updateInfoResponse.statusCode);
                NeedUpdateApp = false;
            }

            _isEndPrepare = true;
        }
    }
}
