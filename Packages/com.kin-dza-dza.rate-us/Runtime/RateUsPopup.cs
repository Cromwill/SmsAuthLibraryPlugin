using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace KinDzaDza.RateUs
{
    public class RateUsPopup : MonoBehaviour
    {
        private const float CloseWindowDelay = 3f;

        [SerializeField] private RateUsPlaceType _rateUsPlaceType = RateUsPlaceType.Timer;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private List<Button> _refuseButtons;
        [SerializeField] private Button _closeByeWindowButton;
        [SerializeField] private CanvasGroup _windowCanvasGroup;
        [SerializeField] private CanvasGroup _evaluationWindow;
        [SerializeField] private CanvasGroup _byeWindow;
        [SerializeField] private RateUsPopupTimer _rateUsPopupTimer;
        [SerializeField] private string _reserveStoreLink;
        [SerializeField] private List<XmlConfigText> _xmlConfigTexts;

        private string _storeLink;
        private Store _store;
        private Coroutine _closeCoroutine;

        public static RateUsPopup Instance { get; private set; }

        public static Action<RateUsPopup> LaunchGoogle;
        public static Action<RateUsPopup> LaunchRuStore;

        public static event Action Shown;
        public static event Action Closed;

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(this);

            DisableCanvasGroup(_windowCanvasGroup);
            DisableCanvasGroup(_evaluationWindow);
            DisableCanvasGroup(_byeWindow);
        }

        private void OnEnable()
        {
            _confirmButton.onClick.AddListener(OnConfirmButtonClicked);
            _refuseButtons.ForEach(b => b.onClick.AddListener(OnRefuseButtonClicked));
            _closeByeWindowButton.onClick.AddListener(CloseImmediately);
        }

        private void OnDisable()
        {
            _confirmButton.onClick.RemoveListener(OnConfirmButtonClicked);
            _refuseButtons.ForEach(b => b.onClick.RemoveListener(OnRefuseButtonClicked));
            _closeByeWindowButton.onClick.RemoveListener(CloseImmediately);

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

            _xmlConfigTexts.ForEach(t => t.FillText());
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
            DisableCanvasGroup(_evaluationWindow);
            DisableCanvasGroup(_byeWindow);
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
            RateUsAnalytics.SendOpenWindow();
            EnableCanvasGroup(_windowCanvasGroup);
            EnableCanvasGroup(_evaluationWindow);
            PlayerPrefs.SetInt(Constants.RateUsShownKey, Constants.TrueValueInt);
            PlayerPrefs.Save();

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
            RateUsAnalytics.SendClicked(true);

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
            RateUsAnalytics.SendClicked(false);

            DisableCanvasGroup(_evaluationWindow);
            EnableCanvasGroup(_byeWindow);

            _closeCoroutine = StartCoroutine(DelayClose());

            IEnumerator DelayClose()
            {
                yield return new WaitForSeconds(CloseWindowDelay);

                Hide();
                _closeCoroutine = null;
            }
        }

        private void CloseImmediately()
        {
            if(_closeCoroutine != null)
            {
                StopCoroutine(_closeCoroutine);
                _closeCoroutine = null;
            }

            DisableCanvasGroup(_byeWindow);
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
