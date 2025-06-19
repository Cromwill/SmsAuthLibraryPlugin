using TMPro;
using UnityEngine;
using UnityEngine.UI;
using KinDzaDzaGames.AdvertisementPlugin;

public class TestBanner : MonoBehaviour, IBannerBlocker
{
    [SerializeField] private Button _bannerShowButton;
    [SerializeField] private Button _bannerHideButton;
    [SerializeField] private Button _bannerSuspendButton;
    [SerializeField] private TMP_Text _bannerSuspendButtonText;
    [SerializeField] private Image _bannerIndicator;
    [SerializeField] private Button _bannerChangePositionButton;
    [SerializeField] private Button _bannerChoosePositionButton;
    [SerializeField] private TMP_Text _bannerChoosePositionButtonText;

    private AdvertisementController _advertisementController;
    private bool _bannerEnabled = true;
    private bool _isBottom = true;
    private PlaceOnScreen _placeOnScreen = PlaceOnScreen.BottomCenter;

    public bool BannerDisplayBlocked { get; private set; }

    private void Awake()
    {
        _bannerSuspendButtonText.text = "Banner enabled";
        BannerDisplayBlocked = false;

        if (_isBottom)
        {
            _bannerChoosePositionButtonText.text = "Bottom position";
            _placeOnScreen = PlaceOnScreen.BottomCenter;
        }
        else
        {
            _bannerChoosePositionButtonText.text = "Top position";
            _placeOnScreen = PlaceOnScreen.TopCenter;
        }

        _advertisementController = AdvertisementController.Instance;

        if(_advertisementController != null)
        {
            if (_advertisementController.BannerShown)
                OnBannerDisplayed();
            else
                OnBannerHided();
        }  
    }

    private void OnEnable()
    {
        _bannerShowButton.onClick.AddListener(ShowBanner);
        _bannerHideButton.onClick.AddListener(HideBanner);
        _bannerSuspendButton.onClick.AddListener(SuspendBanner);
        _bannerChangePositionButton.onClick.AddListener(ChangePosition);
        _bannerChoosePositionButton.onClick.AddListener(ChoosePosition);

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
        _bannerChoosePositionButton.onClick.RemoveListener(ChoosePosition);

        if (_advertisementController != null)
        {
            _advertisementController.BannerDisplayed -= OnBannerDisplayed;
            _advertisementController.BannerHided -= OnBannerHided;
        }
    }

    public void RemoveRestriction() => BannerDisplayBlocked = false;

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
            BannerDisplayBlocked = true;
            _advertisementController?.SuspendDisplayBanner(this);
        }
    }

    private void ShowBanner() => _advertisementController?.ShowBanner();
    private void HideBanner() => _advertisementController?.HideBanner();
    private void ChangePosition() => _advertisementController?.ShowBanner(_placeOnScreen);

    private void ChoosePosition()
    {
        _isBottom = !_isBottom;

        if (_isBottom)
        {
            _bannerChoosePositionButtonText.text = "Bottom position";
            _placeOnScreen = PlaceOnScreen.BottomCenter;
        }
        else
        {
            _bannerChoosePositionButtonText.text = "Top position";
            _placeOnScreen = PlaceOnScreen.TopCenter;
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
