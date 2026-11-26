using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace KinDzaDza.RateUs
{
    public class RateUsConfigProvider
    {
        private readonly Store _store;
        private RateUsConfig _config;
        private string _serverPath;

        public RateUsConfigProvider(Store store, string serverPath)
        {
            _store = store;
            _serverPath = serverPath;
        }

        public async Task<RateUsConfig> GetConfig()
        {
            if (_config != null)
            {
                return _config;
            }

            string json = JsonConvert.SerializeObject(new
            {
                app_id = Application.identifier,
                store_id = _store.ToString(),
                platform = "",
            });

            Debug.LogError($"app_id: {Application.identifier}, store_id: {_store.ToString()}");

            string url = $"https://{_serverPath}/api/{Constants.ControllerName}/{Constants.RateUsConfigName}";
            byte[] payload = Encoding.UTF8.GetBytes(json);
            UploadHandlerRaw upload = new UploadHandlerRaw(payload) { contentType = "application/json" };
            using UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET, new DownloadHandlerBuffer(), upload);

            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");
            request.timeout = 59;
            request.SendWebRequest();

            while (request.result == UnityWebRequest.Result.InProgress)
            {
                await Task.Yield();
            }

            if (request.result != UnityWebRequest.Result.Success || string.IsNullOrEmpty(request.downloadHandler.text))
            {
                Debug.LogError($"[{nameof(RateUsConfigProvider)}]: Fail to receive {Constants.RateUsConfigName}: {request.responseCode} {request.error} {request.downloadHandler.text}");
                return null;
            }

            _config = JsonConvert.DeserializeObject<RateUsConfig>(request.downloadHandler.text);

            return _config;
        }

        public async Task<Response> GetRemoteServerConfig()
        {
            string url = $"https://{_serverPath}/api/remoteconfig/getremote-table/";

            using (UnityWebRequest webRequest = CreateWebRequest(url, UnityWebRequest.kHttpVerbPOST, timeOut: true))
            {
                webRequest.SendWebRequest();

                await WaitProccessing(webRequest);
                TryShowRequestInfo(webRequest, "remoteconfig/getremote-table");

                var body = webRequest.downloadHandler.text;
                return new Response(webRequest.result, webRequest.result.ToString(), body, false);
            }
        }

        private async Task WaitProccessing(UnityWebRequest webRequest, Action<float> progress = null)
        {
            while (webRequest.result == UnityWebRequest.Result.InProgress)
            {
                progress?.Invoke(webRequest.downloadProgress);
                await Task.Yield();
            }
        }

        private void TryShowRequestInfo(UnityWebRequest webRequest, string method)
        {
#if UNITY_EDITOR || TEST
            Debug.Log($"response {method} done {webRequest.result}. Result: {webRequest.downloadHandler.text}");
#endif
            if (webRequest.result != UnityWebRequest.Result.Success)
                Debug.Log($"Response {method} fail: {webRequest.error}, {webRequest.result}");
        }

        private UnityWebRequest CreateWebRequest(string path, string requestType, string accessToken = null, string uploadBody = null, bool timeOut = true)
        {
            var webRequest = new UnityWebRequest(path, requestType);
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            if (timeOut)
                webRequest.timeout = Constants.TimeOut;

            if (string.IsNullOrEmpty(uploadBody) == false)
                webRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(uploadBody));

            webRequest.SetRequestHeader(Constants.ContentType, Constants.AppJson);

            if (string.IsNullOrEmpty(accessToken) == false)
                webRequest.SetRequestHeader("Authorization", $"Bearer {accessToken}");

            return webRequest;
        }
    }
}
