using UnityEngine;
using TMPro;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace Agava.Wink
{
    [Preserve]
    internal class CodeFormatter : MonoBehaviour, IInputFieldFormatting
    {
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private TextCell[] _textCells;

        private int _codeLength;
        private int _length = 0;

        public bool InputDone { get; private set; } = false;

        private void Start()
        {
            _codeLength = _textCells.Length;
            _inputField.resetOnDeActivation = false;
            _inputField.restoreOriginalTextOnEscape = false;
        }

        private void Update()
        {
            _inputField.caretPosition = _length;
        }

        private void OnEnable() => _inputField.onValueChanged.AddListener(OnValueChanged);

        private void OnDisable() => _inputField.onValueChanged.RemoveListener(OnValueChanged);

        public void Clear()
        {
            foreach (TextCell cell in _textCells)
            {
                cell.SetActive(false);
                cell.SetText(string.Empty);
            }
        }

        private void OnValueChanged(string newValue)
        {
            if (newValue.Length > _codeLength)
            {
                _inputField.text = newValue.Substring(0, _codeLength);
            }
            else
            {
                _length = _inputField.text.Length;

                for (int i = 0; i < _codeLength; i++)
                {
                    _textCells[i].SetText(i >= newValue.Length ? string.Empty : newValue[i].ToString());
                    _textCells[i].SetActive(_textCells[i].Empty == false);
                }
            }

            InputDone = _textCells[_textCells.Length - 1].Empty == false;
        }
    }
}
