using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;

namespace Agava.Wink
{
    [Preserve]
    internal class SubmitInputButton : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour _inputFieldFormatting;
        [SerializeField] private Button _button;

        private IInputFieldFormatting InputFieldFormatting => (IInputFieldFormatting)_inputFieldFormatting;

        private void Update()
        {
            _button.interactable = InputFieldFormatting.InputDone;
        }

        private void OnValidate()
        {
            if (_inputFieldFormatting && !(_inputFieldFormatting is IInputFieldFormatting))
            {
                Debug.LogError(nameof(_inputFieldFormatting) + " needs to implement " + nameof(IInputFieldFormatting));
                _inputFieldFormatting = null;
            }
        }
    }
}
