using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private BundleManager _bundleManager;

    private ManifestData _manifestData;

    public Slider progressSlider;
    public Text progressText;

    public GameObject loadingPanel;
    public GameObject mainMenu;      // появится после загрузки
    public Transform popupContainer; // контейнер для отображения префабов

    void Start()
    {
        StartCoroutine(Load());
    }

    private IEnumerator Load()
    {
        yield return new WaitForSeconds(3);

        yield return _bundleManager.LoadManifest(HandleManifest);
    }

    private void HandleManifest(ManifestData manifestData)
    {
        _manifestData = manifestData;
        Debug.Log($"The manifest was successfully downloaded, version = {_manifestData.bundleVersion}, data count = {_manifestData.prefabNames.Length}");

        StartCoroutine(_bundleManager.LoadBundle(_manifestData.bundleVersion, ShowBundleProgress, LoadPopupData));
    }

    private void ShowBundleProgress(float value) => Debug.Log($"Download bundle progress = {value}");
    private void ShowUnpackProgress(float value) => Debug.Log($"Download popup data progress = {value}");

    private void LoadPopupData(bool value)
    {
        Debug.Log($"Download completed, result = {value}");

        if (value)
            StartCoroutine(_bundleManager.LoadPopupsData(_manifestData.prefabNames, ShowUnpackProgress, () => Debug.Log("COMPLETE!")));
    }


    IEnumerator LoadProcess()
    {
        // Показываем загрузочный экран
        if (loadingPanel != null) loadingPanel.SetActive(true);
        if (mainMenu != null) mainMenu.SetActive(false);

        // ---------- 1. Загружаем манифест ----------
        ManifestData manifest = null;
        yield return StartCoroutine(BundleManager.Instance.LoadManifest((data) => manifest = data));

        if (manifest == null)
        {
            Debug.LogError("Не удалось загрузить манифест. Проверьте интернет.");
            // Можно показать кнопку "Повторить"
            yield break;
        }

        // ---------- 2. Загружаем бандл с версией ----------
        bool bundleSuccess = false;
        yield return StartCoroutine(BundleManager.Instance.LoadBundle(
            manifest.bundleVersion,
            (progress) =>
            {
                // Обновляем прогресс-бар (этап 1 - скачивание)
                if (progressSlider != null)
                    progressSlider.value = progress * 0.5f; // первые 50%
                if (progressText != null)
                    progressText.text = $"Скачивание... {Mathf.RoundToInt(progress * 100)}%";
            },
            (success) => bundleSuccess = success
        ));

        if (!bundleSuccess)
        {
            Debug.LogError("Не удалось загрузить бандл");
            yield break;
        }

        // ---------- 3. Асинхронно загружаем все префабы по списку ----------
        bool prefabsLoaded = false;
        yield return StartCoroutine(BundleManager.Instance.LoadPopupsData(
            manifest.prefabNames,
            (progress) =>
            {
                // Обновляем прогресс-бар (этап 2 - загрузка префабов)
                if (progressSlider != null)
                    progressSlider.value = 0.5f + progress * 0.5f; // вторые 50%
                if (progressText != null)
                    progressText.text = $"Загрузка элементов... {Mathf.RoundToInt(progress * 100)}%";
            },
            () => prefabsLoaded = true
        ));

        if (!prefabsLoaded)
        {
            Debug.LogError("Ошибка при загрузке префабов");
            yield break;
        }

        // ---------- 4. Загрузка завершена ----------
        if (loadingPanel != null) loadingPanel.SetActive(false);
        if (mainMenu != null) mainMenu.SetActive(true);

        // Запускаем таймер показа
        StartCoroutine(ShowRandomPrefabTimer());
    }

    // ----- Таймер показа каждые 3 минуты -----
    IEnumerator ShowRandomPrefabTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(180f); // 3 минуты
            ShowRandomPrefab();
        }
    }

    void ShowRandomPrefab()
    {
        PopupData prefab = BundleManager.Instance.GetRandomPrefab();

        if (prefab == null)
        {
            Debug.LogWarning("Нет префабов для показа");
            return;
        }

        // Удаляем старый префаб, если есть
        if (popupContainer != null)
        {
            foreach (Transform child in popupContainer)
                Destroy(child.gameObject);

            PopupData instance = Instantiate(prefab, popupContainer);
            /*RectTransform rect = instance.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchoredPosition = Vector2.zero;
                rect.sizeDelta = Vector2.zero;
            }*/
        }
        else
        {
            Debug.LogError("PopupContainer не назначен!");
        }
    }
}
