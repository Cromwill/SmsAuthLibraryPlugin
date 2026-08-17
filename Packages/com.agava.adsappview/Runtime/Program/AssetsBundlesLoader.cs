using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using AdsAppView.DTO;
using AdsAppView.Utility;
using Newtonsoft.Json;

namespace AdsAppView.Program
{
    [Serializable]
    public class AssetsBundlesLoader
    {
#if UNITY_WEBGL
        private const string Platform = "webgl";
#elif UNITY_STANDALONE
        private const string Platform = "standalone";
#elif UNITY_ANDROID
        private const string Platform = "Android";
#elif UNITY_IOS
        private const string Platform = "iOS";
#endif
        private class AssetPath
        {
            public string[] m_InternalIds;
        }

        private const string ControllerName = "AdsApp";
        private const string FtpCredsRCName = "ftp-creds";

        [SerializeField] private string _catalogPath;
        [SerializeField] private string _assetName;

        private AssetBundle _assetBundle;
        private string _filePath;

        public void Unload()
        {
            if (File.Exists(_filePath))
            {
                _assetBundle.UnloadAsync(false);
                File.Delete(_filePath);
            }
        }

        public async Task<GameObject> GetPopupObject()
        {
            Response ftpCredentialResponse = await AdsAppAPI.Instance.GetRemoteConfig(ControllerName, FtpCredsRCName);

            if (ftpCredentialResponse.statusCode != UnityWebRequest.Result.Success)
                return null;

            FtpCreds creds = JsonConvert.DeserializeObject<FtpCreds>(ftpCredentialResponse.body);

            if (creds == null)
            {
                Debug.LogError("#AssetsBundlesLoader# Fail get creds data");
                return null;
            }

            string cataloPath = $"{_catalogPath}/{Platform}/catalog_1_1.json";
            Debug.Log("#AssetsBundlesLoader# Try download catalog: " + cataloPath);
            string pathFile = await DownloadConfigFile(cataloPath, creds.login, creds.password);

            if (string.IsNullOrEmpty(pathFile))
            {
                Debug.LogError("#AssetsBundlesLoader# Fail download catalog: " + cataloPath);
                return null;
            }

            AssetPath path = JsonConvert.DeserializeObject<AssetPath>(pathFile);
            List<string> list = path.m_InternalIds.ToList();

            string assetPath = list.FirstOrDefault(s => s.StartsWith("http"));
            assetPath = assetPath.Replace("http", "ftp");
            Debug.Log("#AssetsBundlesLoader# " + assetPath);

            _assetBundle = await DownloadAssetBundleFile(assetPath, savePath: Application.persistentDataPath, creds.login, creds.password);

            if (_assetBundle == null)
            {
                Debug.LogError("#AssetsBundlesLoader# Fail load bundle: " + _assetName);
                return null;
            }

            AssetBundleRequest assetRequest = _assetBundle.LoadAssetAsync<GameObject>(_assetName);

            while (assetRequest.isDone == false)
                await Task.Yield();

            GameObject target = assetRequest.asset as GameObject;

            if (target == null)
                Debug.LogError("#AssetsBundlesLoader# Fail load obj from asset bundle: " + _assetName);

            return target;
        }

        private async Task<string> DownloadConfigFile(string ftpUrl, string userName, string password)
        {
            try
            {
                byte[] bytes = await WebClient.DownloadFtpBytes(ftpUrl, userName, password);
                return bytes == null ? null : Encoding.Default.GetString(bytes);
            }
            catch (Exception exception)
            {
                Debug.LogError("#AssetsBundlesLoader# Fail to download catalog: " + exception.Message);
                return null;
            }
        }

        private async Task<AssetBundle> DownloadAssetBundleFile(string ftpUrl, string savePath, string userName, string password)
        {
            if (Uri.TryCreate(ftpUrl, UriKind.Absolute, out _) == false)
                throw new NullReferenceException("Cant create uri: " + ftpUrl);

            string fileName = ftpUrl.Replace("ftp://ftp-p.ctcmedia.ru/mediartk/AssetsBundles/", "");
            fileName = fileName.Replace("/", "");
            fileName = fileName.Replace(Platform, "");
            Debug.Log("#AssetsBundlesLoader# Asset bundle name to load: " + fileName);

            try
            {
                return await DownloadAndSave(ftpUrl, savePath, fileName, userName, password);
            }
            catch
            {
                Debug.LogError("#AssetsBundlesLoader# Fail to download asset bundle #DownloadAssetBundleFile()#: " + fileName);
                return null;
            }
        }

        private async Task<AssetBundle> DownloadAndSave(string ftpUrl, string savePath, string name, string userName, string password)
        {
            savePath += "/Assets";

            if (Directory.Exists(savePath) == false)
            {
                Directory.CreateDirectory(savePath);
                Debug.Log($"#AssetsBundlesLoader# Created folder: " + savePath);
            }
            else
            {
                Debug.Log($"#AssetsBundlesLoader# Folder exist: " + savePath);
            }

            string path = string.IsNullOrEmpty(name) ? savePath : savePath + "/" + name;
            _filePath = path;

            await WebClient.DownloadFtpToFile(ftpUrl, path, userName, password);

            Debug.Log($"#AssetsBundlesLoader# Try load resource {name} from: " + savePath);

            try
            {
                AssetBundleCreateRequest createRequest = AssetBundle.LoadFromFileAsync(path);

                while (createRequest.isDone == false)
                    await Task.Yield();

                AssetBundle assetBundle = createRequest.assetBundle;

                if (assetBundle == null)
                {
                    Debug.Log("#AssetsBundlesLoader# Failed to load AssetBundle!");
                    return null;
                }

                Debug.Log($"#AssetsBundlesLoader# Loaded bundle web {assetBundle.GetAllAssetNames()[0]}");
                return assetBundle;
            }
            catch
            {
                Debug.LogError($"#AssetsBundlesLoader# Fail to load bundle web");
                return null;
            }
        }
    }
}
