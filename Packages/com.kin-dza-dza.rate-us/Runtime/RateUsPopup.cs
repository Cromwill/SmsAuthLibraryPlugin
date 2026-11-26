using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace KinDzaDza.RateUs
{
    public class RateUsPopup : MonoBehaviour
    {
        [SerializeField] private RateUsPlaceType _rateUsPlaceType = RateUsPlaceType.Timer;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private List<Button> _refuseButtons;
        [SerializeField] private CanvasGroup _windowCanvasGroup;
        [SerializeField] private CanvasGroup _evaluationWindow;
        [SerializeField] private RateUsPopupTimer _rateUsPopupTimer;
        [SerializeField] private string _reserveStoreLink;

        private string _storeLink;
        private Store _store;

        public static RateUsPopup Instance { get; private set; }

        public static Action<RateUsPopup> LaunchGoogle;
        public static Action<RateUsPopup> LaunchRuStore;

        public static event Action Shown;
        public static event Action Closed;

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(this);

            Hide();
        }

        private void OnEnable()
        {
            _confirmButton.onClick.AddListener(OnConfirmButtonClicked);
            _refuseButtons.ForEach(b => b.onClick.AddListener(OnRefuseButtonClicked));
        }

        private void OnDisable()
        {
            _confirmButton.onClick.RemoveListener(OnConfirmButtonClicked);
            _refuseButtons.ForEach(b => b.onClick.RemoveListener(OnRefuseButtonClicked));

            _rateUsPopupTimer.TimerExpired -= OnRateUsTimerExpired;
        }

        public void Construct(string storeLink, Store store, float triggerTimeSeconds)
        {
            _storeLink = storeLink;
            _store = store;

            if (_rateUsPlaceType == RateUsPlaceType.Timer)
            {
                _rateUsPopupTimer.Construct(triggerTimeSeconds);
                _rateUsPopupTimer.TimerExpired += OnRateUsTimerExpired;
            }
        }

        public void OpenStorePage()
        {
            TryOpenStorePage();
        }

        public bool TryShow()
        {
            bool IsRateUsShown = PlayerPrefs.GetInt(Constants.RateUsShownKey, Constants.FalseValueInt) == Constants.TrueValueInt;

            if (_rateUsPlaceType != RateUsPlaceType.CustomPlace || IsRateUsShown)
            {
                return false;
            }

            return ShowIfHasLink();
        }

        public void Hide()
        {
            DisableCanvasGroup(_windowCanvasGroup);
            Closed?.Invoke();
        }

        private bool ShowIfHasLink()
        {
            bool isContainsStoreLink = !string.IsNullOrEmpty(_storeLink);

            if (isContainsStoreLink)
            {
                Show();
            }

            return isContainsStoreLink;
        }

        private void Show()
        {
            EnableCanvasGroup(_windowCanvasGroup);
            EnableCanvasGroup(_evaluationWindow);
            PlayerPrefs.SetInt(Constants.RateUsShownKey, Constants.TrueValueInt);

            if (_rateUsPopupTimer.IsExpired == false)
            {
                _rateUsPopupTimer.Stop();
            }

            Shown?.Invoke();
        }

        private void OnRateUsTimerExpired()
        {
            Debug.Log($"[{nameof(RateUsPopup)}]: rate us timer expired!");

            if (_rateUsPlaceType == RateUsPlaceType.Timer)
            {
                ShowIfHasLink();
            }
        }

        private void OnConfirmButtonClicked()
        {
            RateUsAnalytics.SendClicked(true, true);

            switch (_store)
            {
                case Store.Google when LaunchGoogle != null:
                    LaunchGoogle(this);
                    break;
                case Store.RuStore when LaunchRuStore != null:
                    LaunchRuStore(this);
                    break;
                default:
                    TryOpenStorePage();
                    break;
            }
        }

        private void OnRefuseButtonClicked()
        {
            RateUsAnalytics.SendClicked(false, false);
            Hide();
        }

        private bool TryOpenStorePage()
        {
            bool isStoreLinkEmpty = string.IsNullOrEmpty(_storeLink);
            bool isReserveLinkEmpty = string.IsNullOrEmpty(_reserveStoreLink);

            if (!isStoreLinkEmpty)
            {
                Application.OpenURL(_storeLink);
            }
            else if (!isReserveLinkEmpty)
            {
                Application.OpenURL(_reserveStoreLink);
            }

            Hide();

            return !isStoreLinkEmpty;
        }

        private void EnableCanvasGroup(CanvasGroup canvas)
        {
            canvas.alpha = 1;
            canvas.interactable = true;
            canvas.blocksRaycasts = true;
        }

        private void DisableCanvasGroup(CanvasGroup canvas)
        {
            canvas.alpha = 0;
            canvas.interactable = false;
            canvas.blocksRaycasts = false;
        }
    }
}
