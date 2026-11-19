using System.Text;
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
                Debug.LogError($"#RateUs# Fail to receive {Constants.RateUsConfigName}: {request.responseCode} {request.error} {request.downloadHandler.text}");
                return null;
            }

            _config = JsonConvert.DeserializeObject<RateUsConfig>(request.downloadHandler.text);

            return _config;
        }
    }
}
