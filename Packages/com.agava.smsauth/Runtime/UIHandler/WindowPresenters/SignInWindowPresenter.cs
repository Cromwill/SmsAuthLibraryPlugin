using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Scripting;

namespace Agava.Wink
{
    [Preserve]
    internal class SignInWindowPresenter : WindowPresenter
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Button _closeButton;
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private CustomKeyboard _keyboard;
        [SerializeField] private float _displayTime = .6f;
        [SerializeField] private Image _cursorIcon;

        private float _lastDisplayTime = 0;
        private Action Closed;

        private void OnDestroy() => _closeButton.onClick.RemoveListener(Disable);

        private void Awake()
        {
            _closeButton.onClick.AddListener(Disable);

            if (_keyboard == null)
                _keyboard = FindObjectOfType<CustomKeyboard>();
        }

        public override void Enable() => EnableCanvasGroup(_canvasGroup);

        public void Clear()
        {
            _inputField.text = string.Empty;
        }

        private void Update()
        {
            if (Enabled == false)
                return;

            if(_inputField.text.Length == 0)
            {
                _lastDisplayTime += Time.deltaTime;

                if(_lastDisplayTime > _displayTime)
                {
                    _cursorIcon.enabled = !_cursorIcon.enabled;
                    _lastDisplayTime = 0;
                }
            }
            else if(_cursorIcon.enabled)
            {
                _cursorIcon.enabled = false;
            }
        }

        public void Enable(Action closeCallback)
        {
            _keyboard.Clicked += OnClicked;

            Enable();
            Closed = closeCallback;

            _keyboard.Enable();
        }

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
                    if (string.IsNullOrEmpty(_inputField.text))
                    {
                        _inputField.text = "7";
                    }
                    else
                    {
                        string added = _inputField.text + CustomKeyMapping.GetKey(code);
                        _inputField.text = added;
                    }
                }
            }
        }

        public override void Disable()
        {
            _keyboard.Clicked -= OnClicked;
            _keyboard.Disable();

            DisableCanvasGroup(_canvasGroup);
            Closed?.Invoke();
            Clear();
        }
    }
}
