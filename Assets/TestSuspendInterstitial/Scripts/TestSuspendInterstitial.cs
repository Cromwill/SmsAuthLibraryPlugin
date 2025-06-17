using System.Collections;
using System.Collections.Generic;
using KinDzaDzaGames.AdvertisementPlugin;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestSuspendInterstitial : MonoBehaviour, IAdBlocker
{
    [SerializeField] private Button _interButton;
    [SerializeField] private TMP_Text _interButtonText;

    private AdvertisementController _advertisementController;
    private bool _interEnabled = true;

    public bool DisplayBlocked { get; private set; }

    private void Awake()
    {
        _interButtonText.text = "Inter enabled";
        DisplayBlocked = false;
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
            DisplayBlocked = true;
            _advertisementController?.AddInterstitialBlocker(this);
        }
    }

    public void RemoveRestriction() => DisplayBlocked = false;
}
