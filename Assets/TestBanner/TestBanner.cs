using TMPro;
using UnityEngine;
using UnityEngine.UI;
using KinDzaDzaGames.AdvertisementPlugin;

public class TestBanner : MonoBehaviour, IAdBlocker
{
    [SerializeField] private Button _bannerShowButton;
    [SerializeField] private Button _bannerHideButton;
    [SerializeField] private Button _bannerSuspendButton;
    [SerializeField] private TMP_Text _bannerSuspendButtonText;
    [SerializeField] private Button _bannerChangePositionButton;
    [SerializeField] private TMP_Text _bannerChangePositionButtonText;
    [SerializeField] private Image _bannerIndicator;
    [SerializeField] private Button _bannerRestartButton;
    [SerializeField] private TMP_Text _bannerRestartButtonText;

    private AdvertisementController _advertisementController;
    private bool _bannerEnabled = true;
    private bool _bannerPositionBottom = true;
    private bool _showWithChangePosition = false;

    public bool DisplayBlocked { get; private set; }

    private void Awake()
    {
        _bannerSuspendButtonText.text = "Banner enabled";
        _bannerChangePositionButtonText.text = "Bottom position";
        _bannerRestartButtonText.text = "Restart banner: off";
        DisplayBlocked = false;
        _advertisementController = AdvertisementController.Instance;
    }

    private void OnEnable()
    {
        _bannerShowButton.onClick.AddListener(ShowBanner);
        _bannerHideButton.onClick.AddListener(HideBanner);
        _bannerSuspendButton.onClick.AddListener(SuspendBanner);
        _bannerChangePositionButton.onClick.AddListener(ChangePosition);
        _bannerRestartButton.onClick.AddListener(RestartPosition);

        if(_advertisementController != null)
        {
            Debug.Log($"Advertisement Plugin: try subscribe on bunner");
            _advertisementController.BannerDisplayed += OnBannerDisplayed;
            _advertisementController.BannerHided += OnBannerHided;
        }
    }

    private void OnDisable()
    {
        _bannerShowButton.onClick.RemoveListener (ShowBanner);
        _bannerHideButton.onClick.RemoveListener(HideBanner);
        _bannerSuspendButton.onClick.RemoveListener(SuspendBanner);
        _bannerChangePositionButton.onClick.RemoveListener(ChangePosition);
        _bannerRestartButton.onClick.RemoveListener(RestartPosition);

        if (_advertisementController != null)
        {
            _advertisementController.BannerDisplayed -= OnBannerDisplayed;
            _advertisementController.BannerHided -= OnBannerHided;
        }
    }

    public void RemoveRestriction() => DisplayBlocked = false;

    private void SuspendBanner()
    {
        _bannerEnabled = !_bannerEnabled;

        if (_bannerEnabled)
        {
            _bannerSuspendButtonText.text = "Banner enabled";
            RemoveRestriction();
        }
        else
        {
            _bannerSuspendButtonText.text = "Banner suspended";
            DisplayBlocked = true;
            _advertisementController?.SuspendDisplayBanner(this);
        }
    }

    private void ShowBanner() => _advertisementController?.ShowBanner();
    private void HideBanner() => _advertisementController?.HideBanner();

    private void ChangePosition()
    {
        _bannerPositionBottom = !_bannerPositionBottom;

        if (_bannerPositionBottom)
        {
            _bannerChangePositionButtonText.text = "Bottom position";
            _advertisementController?.ChangeBannerPosition(PlaceOnScreen.BottomCenter, _showWithChangePosition);
        }
        else
        {
            _bannerChangePositionButtonText.text = "Top position";
            _advertisementController?.ChangeBannerPosition(PlaceOnScreen.TopCenter, _showWithChangePosition);
        }
    }

    private void RestartPosition()
    {
        _showWithChangePosition = !_showWithChangePosition;

        if (_showWithChangePosition)
        {
            _bannerRestartButtonText.text = "Restart banner: off";
        }
        else
        {
            _bannerRestartButtonText.text = "Restart banner: on";
        }
    }

    private void OnBannerDisplayed()
    {
        Debug.Log("Advertisement Plugin: OnBannerDisplayed");
        _bannerIndicator.color = Color.blue;
    }

    private void OnBannerHided()
    {
        Debug.Log("Advertisement Plugin: OnBannerHided");
        _bannerIndicator.color = Color.green;
    }
}
