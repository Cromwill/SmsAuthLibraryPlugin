using UnityEngine;
using SmsAuthAPI.DTO;
using Newtonsoft.Json;
using AdsAppView.Utility;
using SmsAuthAPI.Program;
using System.Collections;
using UnityEngine.Scripting;
using UnityEngine.Networking;

namespace Agava.Wink
{
    [Preserve]
    public class UpdateService
    {
        private readonly int _bundlIdVersion;
        private readonly Store _storeName;

        private bool _isEndPrepare = false;

        public bool NeedUpdateApp { get; private set; } = false;
        public string AppLinkInStore { get; private set; }

        public UpdateService(int bundlIdVersion, Store storeName)
        {
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

            var updateInfoResponse = await SmsAuthApi.GetUpdatePopupInfo(json);

            if (updateInfoResponse.statusCode == UnityWebRequest.Result.Success)
            {
                if (string.IsNullOrEmpty(updateInfoResponse.body))
                {
                    Debug.LogError($"<color=cyan>Update service:</color> Fail to recieve app link: is Null");
                    NeedUpdateApp = false;
                }
                else
                {
                    UpdateAppInfo appLinks = JsonConvert.DeserializeObject<UpdateAppInfo>(updateInfoResponse.body);
#if UNITY_EDITOR
                    Debug.Log($"<color=cyan>Update service:</color> AppId - {appLinks.app_id}, store_id - {appLinks.store_id}\n" +
                        $"update_bundle - {appLinks.update_bundle},  store_link - {appLinks.store_link}");
#endif
                    AppLinkInStore = appLinks.store_link;
                    NeedUpdateApp = _bundlIdVersion < appLinks.update_bundle;
                }
            }
            else
            {
                Debug.LogError($"<color=cyan>Update service:</color> Fail to recieve app link: " + updateInfoResponse.statusCode);
                NeedUpdateApp = false;
            }

            _isEndPrepare = true;
        }
    }
}
