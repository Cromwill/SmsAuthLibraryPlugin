using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SmsAuthAPI.Program;
using SmsAuthAPI.DTO;
using UnityEngine.Networking;
using UnityEngine.Scripting;
using System.Collections;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;

namespace Agava.Wink
{
    [Preserve]
    internal class InputWindowPresenter : WindowPresenter
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private CodeFormatter _codeFormatter;
        [SerializeField] private EnterCodeShaking _enterCodeShaking;
        [SerializeField] private TextTimer _repeatCodeTimer;
        [SerializeField] private GameObject _wrongCodeText;
        [Header("Buttons")]
        [SerializeField] private Button _sendRepeatCodeButton;
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _continueButton;

        private Action<string> _onInputDone;
        private Action _onBackClicked;
        private string _phone;
        private bool _checkedInputDone = false;

        public bool CodeExpired => _repeatCodeTimer.Expired;
        public bool Initialized => _repeatCodeTimer.Initialized;

        private void Awake()
        {
            _continueButton.onClick.AddListener(OnContinue);
            _sendRepeatCodeButton.onClick.AddListener(OnRepeatClicked);
            _backButton.onClick.AddListener(OnBackClicked);
        }

        private void OnDestroy()
        {
            _continueButton.onClick.RemoveListener(OnContinue);
            _sendRepeatCodeButton.onClick.RemoveListener(OnRepeatClicked);
            _backButton.onClick.RemoveListener(OnBackClicked);
        }

        private void Update()
        {
            if (Enabled == false)
                return;

            if (_codeFormatter.InputDone)
            {
                if (_checkedInputDone == false)
                {
                    _checkedInputDone = true;
                    OnInputDone();
                }
            }
            else
            {
                _checkedInputDone = false;
            }
        }

        public void Enable(string phone, Action<string> onInputDone, Action onBackClicked)
        {
            _phone = phone;

            if (onInputDone != null)
                _onInputDone = onInputDone;

            if (onBackClicked != null)
                _onBackClicked = onBackClicked;

            _repeatCodeTimer.TimerExpired += OnNewCodeTimerExpired;
            _repeatCodeTimer.StartTimer();

            EnableCanvasGroup(_canvasGroup);
        }

        public override void Enable() { }

        public override void Disable()
        {
            DisableCanvasGroup(_canvasGroup);
            Clear();
            _repeatCodeTimer.TimerExpired -= OnNewCodeTimerExpired;
        }

        public void OnInputDone()
        {
            string code = _codeFormatter.InputText;

            if (string.IsNullOrEmpty(code))
                return;

            bool isCorrectCode = uint.TryParse(code, System.Globalization.NumberStyles.Integer, CultureInfo.InvariantCulture, out uint _);

            if (isCorrectCode)
            {
                _onInputDone?.Invoke(code);
                _codeFormatter.SetInteractable(false);
            }
        }

        public void Response(bool codeAccepted)
        {
            if (codeAccepted)
            {
                _continueButton.gameObject.SetActive(true);
            }
            else
            {
                Clear();
                _wrongCodeText.SetActive(true);
                _enterCodeShaking.StartAnimation();

                StartCoroutine(WaitForAnimation());

                IEnumerator WaitForAnimation()
                {
                    yield return new WaitWhile(() => _enterCodeShaking.Shaking);

                    _codeFormatter.SetInteractable(true);
                }
            }
        }

        public void Clear()
        {
            _sendRepeatCodeButton.gameObject.SetActive(false);
            _wrongCodeText.gameObject.SetActive(false);
            _continueButton.gameObject.SetActive(false);
            _codeFormatter.Clear();
        }

        private void OnBackClicked()
        {
            _onBackClicked?.Invoke();
        }

        private void OnNewCodeTimerExpired()
        {
            Clear();
            _codeFormatter.SetInteractable(false);
            _sendRepeatCodeButton.gameObject.SetActive(true);
            _repeatCodeTimer.ResetTimer();
        }

        private void OnRepeatClicked()
        {
            _sendRepeatCodeButton.gameObject.SetActive(false);

            StartCoroutine(WaitForResponse());

            IEnumerator WaitForResponse()
            {
                Task<Response> task = SmsAuthApi.Regist(_phone);

                yield return new WaitUntil(() => task.IsCompleted);

                var statusCode = task.Result.statusCode;

                if (statusCode != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Repeat send sms Error : " + statusCode);
                    _sendRepeatCodeButton.gameObject.SetActive(true);
                }
                else
                {
                    _codeFormatter.SetInteractable(true);
                    _repeatCodeTimer.StartTimer();
                }
            }
        }

        private void OnContinue()
        {
            Disable();
        }
    }
}
