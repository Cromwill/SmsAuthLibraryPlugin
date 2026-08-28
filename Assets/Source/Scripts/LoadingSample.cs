using Agava.Wink;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;

public class LoadingSample : MonoBehaviour
{
    [SerializeField] private bool _usePortraitCarousel;
    [SerializeField] private BundleManager _bundleManager;
    [SerializeField] private float _waitDownloadDelay = 3;
    [Header("Loading Canvas")]
    [SerializeField] private CanvasGroup _loadingCanvas;
    [SerializeField] private GameObject _portraitPopup;
    [SerializeField] private ImagesCarousel _portraitImagesCarousel;
    [SerializeField] private Image _portraitLoadingBar;
    [SerializeField] private GameObject _landscapePopup;
    [SerializeField] private ImagesCarousel _landscapeImagesCarousel;
    [SerializeField] private Image _landscapetLoadingBar;
    [SerializeField] private CarouselSettings _carouselSettings;
    [Header("Popup Canvas")]
    [SerializeField] private CanvasGroup _popupCanvas;
    [SerializeField] private PopupPresenter _popupPresenter;

    private ManifestData _manifestData;
    private Image _loadingBar;
    private bool _loadResult = false;

    private IEnumerator Start()
    {
        _bundleManager.Construct();

        if (_usePortraitCarousel)
        {
            _loadingBar = _portraitLoadingBar;
            _landscapePopup.SetActive(false);
            _portraitPopup.SetActive(true);
            _portraitImagesCarousel.Construct(_carouselSettings);
            _portraitImagesCarousel.Enable();
        }
        else
        {
            _loadingBar = _landscapetLoadingBar;
            _portraitPopup.SetActive(false);
            _landscapePopup.SetActive(true);
            _landscapeImagesCarousel.Construct(_carouselSettings);
            _landscapeImagesCarousel.Enable();
        }

        yield return new WaitForSeconds(_waitDownloadDelay);
        yield return _bundleManager.LoadManifest(HandleManifest);
        yield return _bundleManager.LoadBundle(_manifestData.bundleVersion, StartFillLoadingBar, LoadPopupData);

        if (_loadResult)
            yield return _bundleManager.LoadPopupsData(_manifestData.prefabNames, FinishFillLoadingBar, () => StartPopupTimer(_loadingCanvas));

        _popupPresenter.CloseButtonClicked += OnCloseButtonClicked;
    }

    private void OnDisable()
    {
        _popupPresenter.CloseButtonClicked -= OnCloseButtonClicked;
    }

    private void HandleManifest(ManifestData manifestData)
    {
        _manifestData = manifestData;
        Debug.Log($"The manifest was successfully downloaded, version = {_manifestData.bundleVersion}, data count = {_manifestData.prefabNames.Length}");
    }

    private void StartFillLoadingBar(float value) => _loadingBar.fillAmount = value * 0.5f;
    private void FinishFillLoadingBar(float value) => _loadingBar.fillAmount = 0.5f + value * 0.5f;
    private void LoadPopupData(bool result) => _loadResult = result;

    private void StartPopupTimer(CanvasGroup canvas)
    {
        if (_usePortraitCarousel)
            _portraitImagesCarousel.Disable();
        else
            _landscapeImagesCarousel.Disable();

        HideCanvas(canvas);
        StartCoroutine(ShowRandomPrefabTimer());
    }

    private void HideCanvas(CanvasGroup canvas)
    {
        canvas.interactable = false;
        canvas.DOFade(0, 2).OnComplete(() => canvas.blocksRaycasts = false);
    }

    private void OnCloseButtonClicked()
    {
        StartPopupTimer(_popupCanvas);
    }

    private IEnumerator ShowRandomPrefabTimer()
    {
        yield return new WaitForSeconds(30f);

        _popupPresenter.Show(_bundleManager.GetRandomPrefab());
        _popupCanvas.blocksRaycasts = true;
        _popupCanvas.DOFade(1, 2).OnComplete(() => _popupCanvas.interactable = true);
    }
}
