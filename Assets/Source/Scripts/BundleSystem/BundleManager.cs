using System;
using System.IO;
using UnityEngine;
using System.Threading;
using System.Collections;
using UnityEngine.Scripting;
using UnityEngine.Networking;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[Preserve]
public class BundleManager : MonoBehaviour
{
    private const string Manifest = "manifest.json";
    private const string BundleData = "uipanels";
    private const string BundleCache = nameof(BundleCache);

    [Header("Server")]
    public string _serverUrl = "https://storage.yandexcloud.net/winkpopupdata/PopupData/";

    private AssetBundle _mainBundle;
    private bool _isLoaded = false;
    private List<PopupData> _popupsData = new List<PopupData>();
    private string _cacheFolder;

    public void Construct()
    {
        _cacheFolder = Path.Combine(Application.persistentDataPath, BundleCache);

        if (Directory.Exists(_cacheFolder) == false)
            Directory.CreateDirectory(_cacheFolder);
    }

    public IEnumerator LoadManifest(Action<ManifestData> onDone)
    {
        string url = _serverUrl + Manifest;

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Manifest loading error: {request.error}");
                onDone?.Invoke(null);
                yield break;
            }

            string json = request.downloadHandler.text;
            ManifestData data = JsonUtility.FromJson<ManifestData>(json);
            onDone?.Invoke(data);
        }
    }

    public IEnumerator LoadBundle(int version, Action<float> onProgress, Action<bool> onComplete)
    {
        string url = _serverUrl + BundleData;
        string localPath = GetBundleFilePath(version);

        yield return CheckLocalCache(localPath, version, onProgress, onComplete);

        if (_isLoaded)
            yield break;

        Debug.Log($"Downloading bundle from server: {url}");

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            UnityWebRequestAsyncOperation operation = request.SendWebRequest();

            while (operation.isDone == false)
            {
                onProgress?.Invoke(operation.progress);
                yield return null;
            }

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Bundle loading error: {request.error}");
                onProgress?.Invoke(1f);
                onComplete?.Invoke(false);
                yield break;
            }

            // Получаем байты и сохраняем в файл (асинхронно)
            byte[] bundleData = request.downloadHandler.data;
            yield return SaveBundleAsync(bundleData, localPath);

            // Загружаем бандл из сохранённого файла
            _mainBundle = AssetBundle.LoadFromFile(localPath);

            if (_mainBundle == null)
            {
                Debug.LogError("Couldn't load bundle from saved file");
                onProgress?.Invoke(1f);
                onComplete?.Invoke(false);
                yield break;
            }

            _isLoaded = true;
            onProgress?.Invoke(1f);
            onComplete?.Invoke(true);
        }
    }

    public IEnumerator LoadPopupsData(string[] names, Action<float> onProgress, Action onComplete)
    {
        if (_isLoaded == false || _mainBundle == null)
        {
            Debug.LogError("Bundle not loaded");
            onComplete?.Invoke();
            yield break;
        }

        _popupsData.Clear();
        int total = names.Length;
        int loaded = 0;

        foreach (string name in names)
        {
            AssetBundleRequest request = _mainBundle.LoadAssetAsync<PopupData>(name);
            yield return request;

            if (request.asset != null)
            {
                _popupsData.Add(request.asset as PopupData);
                Debug.Log($"Object downloaded: {name}");
            }
            else
            {
                Debug.LogWarning($"Object '{name}' not found in the bundle");
            }

            loaded++;
            float progress = (float)loaded / total;
            onProgress?.Invoke(progress);
        }

        onComplete?.Invoke();
    }

    public PopupData GetRandomPrefab()
    {
        if (_popupsData.Count == 0)
            return null;

        int idx = Random.Range(0, _popupsData.Count);

        return _popupsData[idx];
    }

    public void UnloadBundle()
    {
        if (_mainBundle != null)
        {
            _mainBundle.Unload(true);
            _mainBundle = null;
            _isLoaded = false;
            _popupsData.Clear();
            Debug.Log("Bundle unloaded.");
        }
    }

    void OnApplicationQuit()
    {
        UnloadBundle();
    }

    private IEnumerator CheckLocalCache(string path, int version, Action<float> onProgress, Action<bool> onComplete)
    {
        if (File.Exists(path))
        {
            Debug.Log($"Loading bundle from local cache: {path}");

            _mainBundle = AssetBundle.LoadFromFile(path);

            if (_mainBundle != null)
            {
                _isLoaded = true;
                onProgress?.Invoke(1f);
                onComplete?.Invoke(true);
                yield break;
            }
            else
            {
                File.Delete(path);
                Debug.LogWarning("Local cache file corrupted, deleted him!");
            }
        }
    }

    private IEnumerator SaveBundleAsync(byte[] data, string path)
    {
        bool done = false;

        ThreadPool.QueueUserWorkItem((state) =>
        {
            try
            {
                File.WriteAllBytes(path, data);
                Debug.Log($"Bundle saved to cache: {path}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save bundle: {e.Message}");
            }
            done = true;
        });

        while (done == false)
            yield return null;
    }

    private string GetBundleFilePath(int version) => Path.Combine(_cacheFolder, $"{BundleData}_v{version}.bundle");
}
