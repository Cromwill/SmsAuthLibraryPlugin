using Agava.Wink;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadingSample : MonoBehaviour
{
    [SerializeField] private bool _usePortraitCarousel;
    [SerializeField] private BundleManager _bundleManager;
    [SerializeField] private ImagesCarousel _portraitImagesCarousel;
    [SerializeField] private ImagesCarousel _landscapeImagesCarousel;
    [SerializeField] private CarouselSettings _carouselSettings;
    [SerializeField] private Image _loadingBar;
    [SerializeField] private float _waitDelay = 3;

    private ManifestData _manifestData;
    private bool _loadResult = false;

    private IEnumerator Start()
    {
        if(_usePortraitCarousel)
        {
            _portraitImagesCarousel.Construct(_carouselSettings);
            _portraitImagesCarousel.Enable();
        }
        else
        {
            _landscapeImagesCarousel.Construct(_carouselSettings);
            _landscapeImagesCarousel.Enable();
        }

        yield return new WaitForSeconds(_waitDelay);

        yield return _bundleManager.LoadManifest(HandleManifest);
        yield return _bundleManager.LoadBundle(_manifestData.bundleVersion, StartFillLoadingBar, LoadPopupData);

        if(_loadResult)
            yield return _bundleManager.LoadPopupsData(_manifestData.prefabNames, FinishFillLoadingBar, () => Debug.Log("COMPLETE!"));
    }

    private void HandleManifest(ManifestData manifestData)
    {
        _manifestData = manifestData;
        Debug.Log($"The manifest was successfully downloaded, version = {_manifestData.bundleVersion}, data count = {_manifestData.prefabNames.Length}");
    }

    private void StartFillLoadingBar(float value) => _loadingBar.fillAmount = value * 0.5f;
    private void FinishFillLoadingBar(float value) => _loadingBar.fillAmount = 0.5f + value * 0.5f;
    private void LoadPopupData(bool result) => _loadResult = result;
}
