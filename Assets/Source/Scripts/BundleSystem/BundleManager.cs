using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

public class BundleManager : MonoBehaviour
{
    public static BundleManager Instance { get; private set; }

    [Header("Сервер")]
    public string baseUrl = "https://your-server.com/bundles/";

    private AssetBundle _mainBundle;
    private bool _isLoaded = false;
    private List<PopupData> _popupsData = new List<PopupData>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator LoadManifest(Action<ManifestData> onDone)
    {
        string url = baseUrl + "manifest.json";

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
        string url = baseUrl + "uipanels";
        using (UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(url, (uint)version))
        {
            UnityWebRequestAsyncOperation operation = request.SendWebRequest();
            while (operation.isDone == false)
            {
                // Прогресс от 0 до 1 (но может быть неточным, но работает)
                float progress = operation.progress;
                onProgress?.Invoke(progress);
                yield return null;
            }

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Bundle loading error: {request.error}");
                onProgress?.Invoke(1f);
                onComplete?.Invoke(false);
                yield break;
            }

            _mainBundle = DownloadHandlerAssetBundle.GetContent(request);

            if (_mainBundle == null)
            {
                Debug.LogError("Couldn't unpack the bundle");
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
        if (_popupsData.Count == 0) return null;
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
        }
    }

    void OnApplicationQuit()
    {
        UnloadBundle();
    }
}
