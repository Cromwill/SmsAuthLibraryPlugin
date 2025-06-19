using TMPro;
using UnityEngine;
using UnityEngine.UI;
using KinDzaDzaGames.AdvertisementPlugin;

public class TestSuspendInterstitial : MonoBehaviour, IInterstitialBlocker
{
    [SerializeField] private Button _interButton;
    [SerializeField] private TMP_Text _interButtonText;

    private AdvertisementController _advertisementController;
    private bool _interEnabled = true;

    public bool InterstitialDisplayBlocked { get; private set; }

    private void Awake()
    {
        _interButtonText.text = "Inter enabled";
        InterstitialDisplayBlocked = false;
        _advertisementController = AdvertisementController.Instance;
    }

    private void OnEnable()
    {
        _interButton.onClick.AddListener(SuspendInter);
    }

    private void OnDisable()
    {
        _interButton.onClick.RemoveListener(SuspendInter);
    }

    private void SuspendInter()
    {
        _interEnabled = !_interEnabled;

        if (_interEnabled)
        {
            _interButtonText.text = "Inter enabled";
            RemoveRestriction();
        }
        else
        {
            _interButtonText.text = "Inter suspended";
            InterstitialDisplayBlocked = true;
            _advertisementController?.AddInterstitialBlocker(this);
        }
    }

    public void RemoveRestriction() => InterstitialDisplayBlocked = false;
}
