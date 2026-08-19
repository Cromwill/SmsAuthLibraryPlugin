using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SmsAuthAPI.DTO;
using SmsAuthAPI.Program;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Scripting;
using UnityEngine.UI;

namespace Agava.Wink
{
    [Preserve]
    internal class InputWindowPresenter : WindowPresenter
    {
        private const string CodeExpirationDateKey = nameof(CodeExpirationDateKey);
        private const float CloseWindowDelay = 1f;

        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private CodeFormatter _codeFormatter;
        [SerializeField] private EnterCodeShaking _enterCodeShaking;
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private TextTimer _repeatCodeTimer;
        [SerializeField] private List<XmlConfigText> _xmlConfigTexts;
        [Header("Buttons")]
        [SerializeField] private CustomKeyboard _keyboard;
        [SerializeField] private Button _sendRepeatCodeButton;
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _continueButton;
        [Header("Wrong code texts")]
        [SerializeField] private GameObject _wrongCodeTextTop;
        [SerializeField] private GameObject _wrongCodeTextBottom;

        private SmsRetrieverManager _smsRetrieverManager;
        private Action<string> _onInputDone;
        private Action _onBackClicked;
        private string _phone;
        private string _appHash = string.Empty;
        private bool _checkedInputDone = false;
        private bool _wrongCodeTextActive = false;
        private bool _repeatCodeButtonActive = false;
        private bool _canSetCode = true;

        public bool ZeroSeconds => _repeatCodeTimer.ZeroSeconds;
        public bool Initialized => _repeatCodeTimer.Initialized;

        private void Awake()
        {
            _continueButton.onClick.AddListener(OnContinue);
            _sendRepeatCodeButton.onClick.AddListener(OnRepeatClicked);
            _backButton.onClick.AddListener(OnBackClicked);

            if (_keyboard == null)
                _keyboard = FindObjectOfType<CustomKeyboard>();
        }

        private void OnDestroy()
        {
            _continueButton.onClick.RemoveListener(OnContinue);
            _sendRepeatCodeButton.onClick.RemoveListener(OnRepeatClicked);
            _backButton.onClick.RemoveListener(OnBackClicked);
        }

        public void Construct(SmsRetrieverManager smsRetrieverManager)
        {
            _smsRetrieverManager = smsRetrieverManager ?? throw new ArgumentNullException(nameof(smsRetrieverManager));

            _smsRetrieverManager.SmsReceived += OnSmsReceived;
        }

        public void FillRemoteTexts() => _xmlConfigTexts.ForEach(t => t.FillText());

        public void Dispose()
        {
            _smsRetrieverManager.SmsReceived -= OnSmsReceived;
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

            if (_wrongCodeTextActive)
            {
                if (_codeFormatter.InputText.Length > 0)
                {
                    SetWrongTextActive(false);
                    _wrongCodeTextActive = false;
                }
            }

            if (_repeatCodeButtonActive)
            {
                if (CodeExpired())
                {
                    Clear();
                    _codeFormatter.SetInteractable(false);
                }
            }
        }

        public void Enable(string phone, string appHash, Action<string> onInputDone, Action onBackClicked)
        {
            _phone = phone;
            _appHash = appHash;
            _keyboard.Enable();
            _keyboard.Clicked += OnClicked;

            if (onInputDone != null)
                _onInputDone = onInputDone;

            if (onBackClicked != null)
                _onBackClicked = onBackClicked;

            _backButton.gameObject.SetActive(true);
            _repeatCodeTimer.TimerExpired += OnRepeatCodeTimerExpired;
            _repeatCodeTimer.StartTimer();
            _codeFormatter.SetInteractable(true);

            EnableCanvasGroup(_canvasGroup);
        }

        public override void Enable() { }

        public override void Disable()
        {
            _keyboard.Clicked -= OnClicked;
            _keyboard.Disable();

            DisableCanvasGroup(_canvasGroup);
            Clear();
            SetRepeatButtonActive(false);
            _repeatCodeTimer.TimerExpired -= OnRepeatCodeTimerExpired;
        }

        public void OnInputDone()
        {
            string code = _codeFormatter.InputText;

            if (string.IsNullOrEmpty(code))
                return;

            if (uint.TryParse(code, NumberStyles.Integer, CultureInfo.InvariantCulture, out uint _))
            {
                _onInputDone?.Invoke(code);
                _codeFormatter.SetInteractable(false);
            }
        }

        public void Response(bool codeAccepted)
        {
            if (codeAccepted)
            {
                ResetCodeTimer();
                _backButton.gameObject.SetActive(false);
                //_continueButton.gameObject.SetActive(true);
                AnalyticsWinkService.SendOnEnteredCorrecOtpCodeWindow();
                StartCoroutine(CloseWithDelay());

                IEnumerator CloseWithDelay()
                {
                    yield return new WaitForSeconds(CloseWindowDelay);

                    Disable();
                }
            }
            else
            {
                Clear();
                SetWrongTextActive(true);
                AnalyticsWinkService.SendOnEnteredWrongOtpCodeWindow();
                _wrongCodeTextActive = true;
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
            SetWrongTextActive(false);
            _continueButton.gameObject.SetActive(false);
            _codeFormatter.Clear();
        }

        public void ResetCodeTimer()
        {
            _repeatCodeTimer.StopTimer();
            _repeatCodeTimer.ResetTimer();
            _repeatCodeTimer.ResetSeconds();
        }

        public void ActivateOtpCodeSetter() => _canSetCode = true;

        private void OnClicked(KeyCode code)
        {
            if (code == KeyCode.Backspace && _inputField.text.Length > 0)
            {
                _inputField.text = _inputField.text.Substring(0, _inputField.text.Length - 1);
            }
            else
            {
                if (string.IsNullOrEmpty(CustomKeyMapping.GetKey(code)) == false)
                {
                    string added = _inputField.text + CustomKeyMapping.GetKey(code);
                    _inputField.text = added;
                }
            }
        }

        private void OnBackClicked()
        {
            _onBackClicked?.Invoke();
        }

        private void OnRepeatCodeTimerExpired()
        {
            SetRepeatButtonActive(true);
            _repeatCodeTimer.ResetTimer();
        }

        private void OnRepeatClicked()
        {
            SetRepeatButtonActive(false);
            _canSetCode = true;

            AnalyticsWinkService.SendRepeatOtpCodeRequestButtonClick();
            _smsRetrieverManager.ReloadRetriever();

            StartCoroutine(WaitForResponse());

            IEnumerator WaitForResponse()
            {
                Task<Response> task;

                if (string.IsNullOrEmpty(_appHash))
                {
                    task = SmsAuthApi.Regist(_phone);
                }
                else
                {
                    RequestHashOtpData otpData = new() { phone = _phone, hashText = _appHash };
                    Debug.Log($"SMS Retriever: repeat send code, phone = {otpData.phone}, hash = {otpData.hashText}");

                    task = SmsAuthApi.Regist(otpData);
                }

                yield return new WaitUntil(() => task.IsCompleted);

                var statusCode = task.Result.statusCode;

                if (statusCode != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Repeat send sms Error : " + statusCode);
                    SetRepeatButtonActive(true);
                }
                else
                {
                    _repeatCodeTimer.StartTimer();
                    _codeFormatter.SetInteractable(true);
                }
            }
        }

        private void OnContinue()
        {
            Disable();
        }

        private void SetWrongTextActive(bool active)
        {
            _wrongCodeTextTop.gameObject.SetActive(active && !_repeatCodeButtonActive);
            _wrongCodeTextBottom.gameObject.SetActive(active && _repeatCodeButtonActive);
        }

        private bool CodeExpired()
        {
            if (UnityEngine.PlayerPrefs.HasKey(CodeExpirationDateKey))
            {
                if (DateTime.TryParse(UnityEngine.PlayerPrefs.GetString(CodeExpirationDateKey), out DateTime expirationDate))
                {
                    return expirationDate.Subtract(DateTime.Now).Seconds < 0;
                }
            }

            return false;
        }

        private void SetRepeatButtonActive(bool active)
        {
            _sendRepeatCodeButton.gameObject.SetActive(active);
            _repeatCodeButtonActive = active;
        }

        private void OnSmsReceived(string message)
        {
            if (_canSetCode == false)
                return;

            _canSetCode = false;
            _codeFormatter.SetCode(ExtractOtp(message));
        }

        private string ExtractOtp(string message)
        {
            Debug.Log($"SMS Retriever: catch sms. Message: {message}");

            Match match = Regex.Match(message, @"\b(\d{4}|\d{6})\b");
            return match.Success ? match.Value : "";
        }
    }
}
