using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Scripting;
using Newtonsoft.Json;
using AdsAppView.DTO;
using System.IO;
using System.Net;

namespace AdsAppView.Utility
{
    [Preserve]
    public class WebClient
    {
        private const string RootApi = "/api";
        private const string AppJson = "application/json";
        private const string ContentType = "Content-Type";
        private const int TimeOut = 59;
        private const int FtpBufferSize = 64 * 1024;

        private readonly string _serverPath;

        protected enum RequestType { POST, GET, PUT }

        public WebClient(string serverPath) => _serverPath = serverPath;

        public async Task<Response> GetRemote(string apiName, string key)
        {
            string path = $"{GetHttpPath(apiName, key.ToLower())}";

            using (UnityWebRequest webRequest = CreateWebRequest(path, RequestType.GET))
            {
                webRequest.SendWebRequest();

                await WaitProccessing(webRequest);
                TryShowRequestInfo(webRequest, apiName);

                var body = JsonConvert.DeserializeObject<string>(webRequest.downloadHandler.text);
                return new Response(webRequest.result, webRequest.result.ToString(), body, false, null);
            }
        }

        public async Task<Response> GetRemote(Request request)
        {
            string path = $"{GetHttpPath(request.api_name)}";

            using (UnityWebRequest webRequest = CreateWebRequest(path, RequestType.GET, uploadBody: request.body))
            {
                webRequest.SendWebRequest();

                await WaitProccessing(webRequest);
                TryShowRequestInfo(webRequest, request.api_name);

                var body = webRequest.downloadHandler.text;
                return new Response(webRequest.result, webRequest.result.ToString(), body, false, null);
            }
        }

        public async Task<Response> GetFilePath(Request request)
        {
            string path = $"{GetHttpPath(request.api_name)}";

            using (UnityWebRequest webRequest = CreateWebRequest(path, RequestType.POST, uploadBody: request.body))
            {
                webRequest.SendWebRequest();

                await WaitProccessing(webRequest);
                TryShowRequestInfo(webRequest, request.api_name);

                var body = webRequest.downloadHandler.text;
                return new Response(webRequest.result, webRequest.result.ToString(), body, false, null);
            }
        }

        public async Task<Response> GetAppSettings(Request request)
        {
            string path = $"{GetHttpPath(request.api_name)}";

            using (UnityWebRequest webRequest = CreateWebRequest(path, RequestType.POST, uploadBody: request.body))
            {
                webRequest.SendWebRequest();

                await WaitProccessing(webRequest);
                TryShowRequestInfo(webRequest, request.api_name);

                var body = webRequest.downloadHandler.text;
                return new Response(webRequest.result, webRequest.result.ToString(), body, false, null);
            }
        }

        public async Task<Response> GetPluginSettings(string apiName, string key)
        {
            string path = $"{GetHttpPath(apiName, key.ToLower())}";

            using (UnityWebRequest webRequest = CreateWebRequest(path, RequestType.GET))
            {
                webRequest.SendWebRequest();

                await WaitProccessing(webRequest);
                TryShowRequestInfo(webRequest, apiName);

                string body = webRequest.downloadHandler.text;
                return new Response(webRequest.result, webRequest.result.ToString(), body, false, null);
            }
        }

        public async Task<Response> GetBytesData(Request request)
        {
            string address = request.api_name;
            Debug.Log("#WebClient# Web address: " + address);

            try
            {
                byte[] downloaded = await DownloadFtpBytes(address, request.login, request.password);
                UnityWebRequest.Result result = downloaded != null ? UnityWebRequest.Result.Success : UnityWebRequest.Result.DataProcessingError;
                return new Response(result, result.ToString(), "", false, downloaded);
            }
            catch (Exception exception)
            {
                Debug.LogError("#WebClient# FTP download fail: " + exception.Message);
                return new Response(UnityWebRequest.Result.ConnectionError, exception.Message, "", false, null);
            }
        }

        public async Task<Response> DownloadToFile(string ftpUrl, string savePath, string userName, string password)
        {
            Debug.Log("#WebClient# Web address: " + ftpUrl);

            try
            {
                await DownloadFtpToFile(ftpUrl, savePath, userName, password);
                UnityWebRequest.Result result = UnityWebRequest.Result.Success;
                return new Response(result, result.ToString(), "", false, null);
            }
            catch (Exception exception)
            {
                Debug.LogError("#WebClient# FTP download to file fail: " + exception.Message);
                return new Response(UnityWebRequest.Result.ConnectionError, exception.Message, "", false, null);
            }
        }

        public static async Task<byte[]> DownloadFtpBytes(string ftpUrl, string userName, string password)
        {
            return await RunOffMainThread(async () =>
            {
                FtpWebRequest request = CreateFtpRequest(ftpUrl, userName, password);

                using (WebResponse response = await request.GetResponseAsync().ConfigureAwait(false))
                using (Stream input = response.GetResponseStream())
                await using (MemoryStream memoryStream = new MemoryStream())
                {
                    if (input == null)
                        return null;

                    byte[] buffer = new byte[FtpBufferSize];
                    int read;

                    while ((read = await input.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false)) > 0)
                        await memoryStream.WriteAsync(buffer, 0, read).ConfigureAwait(false);

                    return memoryStream.ToArray();
                }
            });
        }

        public static async Task DownloadFtpToFile(string ftpUrl, string savePath, string userName, string password)
        {
            await RunOffMainThread(async () =>
            {
                FtpWebRequest request = CreateFtpRequest(ftpUrl, userName, password);
                string directory = Path.GetDirectoryName(savePath);

                if (string.IsNullOrEmpty(directory) == false && Directory.Exists(directory) == false)
                    Directory.CreateDirectory(directory);

                using (WebResponse response = await request.GetResponseAsync().ConfigureAwait(false))
                using (Stream input = response.GetResponseStream())
                await using (FileStream fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None, FtpBufferSize, useAsync: true))
                {
                    if (input == null)
                        throw new InvalidOperationException("FTP response stream is null: " + ftpUrl);

                    byte[] buffer = new byte[FtpBufferSize];
                    int read;

                    while ((read = await input.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false)) > 0)
                        await fileStream.WriteAsync(buffer, 0, read).ConfigureAwait(false);
                }
            });
        }

        private static FtpWebRequest CreateFtpRequest(string ftpUrl, string userName, string password)
        {
            if (Uri.TryCreate(ftpUrl, UriKind.Absolute, out Uri uri) == false)
                throw new NullReferenceException("Cant create uri: " + ftpUrl);

#pragma warning disable SYSLIB0014
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
#pragma warning restore SYSLIB0014
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = true;
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            if (string.IsNullOrEmpty(userName) == false && string.IsNullOrEmpty(password) == false)
                request.Credentials = new NetworkCredential(userName, password);

            return request;
        }

        private static async Task<T> RunOffMainThread<T>(Func<Task<T>> action)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return await action();
#else
            return await Task.Run(action);
#endif
        }

        private static async Task RunOffMainThread(Func<Task> action)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            await action();
#else
            await Task.Run(action);
#endif
        }

        private string GetHttpPath(string apiName, string apiData = null, bool api = true)
        {
            apiData ??= string.Empty;
            string apiRoute = string.Empty;

            if (api)
                apiRoute = RootApi;

            string path = $"{_serverPath}{apiRoute}/{apiName.ToLower()}/{apiData}";
            return $"https://{path}";
        }

        private UnityWebRequest CreateWebRequest(string path, RequestType type, string accessToken = null, string uploadBody = null, bool timeOut = true)
        {
            var httpRequest = new UnityWebRequest(path, type.ToString());
            httpRequest.downloadHandler = new DownloadHandlerBuffer();

            if (timeOut)
                httpRequest.timeout = TimeOut;

            if (string.IsNullOrEmpty(uploadBody) == false)
                httpRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(uploadBody));

            httpRequest.SetRequestHeader(ContentType, AppJson);

            if (string.IsNullOrEmpty(accessToken) == false)
                httpRequest.SetRequestHeader("Authorization", $"Bearer {accessToken}");

            return httpRequest;
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
            Debug.Log($"#WebClient# response {method} to {webRequest.url} done {webRequest.result}. Result: {webRequest.downloadHandler.text}");

            if (webRequest.result != UnityWebRequest.Result.Success)
                Debug.LogError($"#WebClient# Response {method} fail: {webRequest.error}, {webRequest.result}");
        }
    }
}
