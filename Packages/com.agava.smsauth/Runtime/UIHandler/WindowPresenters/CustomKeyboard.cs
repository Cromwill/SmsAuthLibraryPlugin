using System;
using UnityEngine;

namespace Agava.Wink
{
    internal class CustomKeyboard : WindowPresenter
    {
        [SerializeField] private KeyboardButton[] _buttons;
        [SerializeField] private CanvasGroup _groupHorizontal;
        [SerializeField] private CanvasGroup _groupVertical;

        public event Action<KeyCode> Clicked;

        private void OnDestroy()
        {
            for (int i = 0; i < _buttons.Length; i++)
            {
                KeyboardButton btn = _buttons[i];
                btn.Clicked -= OnClicked;
            }
        }

        private void Awake()
        {
            DisableCanvasGroup(_groupHorizontal);

            for (int i = 0; i < _buttons.Length; i++)
            {
                KeyboardButton btn = _buttons[i];
                btn.Clicked += OnClicked;
            }
        }

        public override void Enable()
        {
            CanvasGroup group = (Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown)
                ? _groupVertical : _groupHorizontal;

            EnableCanvasGroup(group);
        }

        public override void Disable()
        {
            DisableCanvasGroup(_groupVertical);
            DisableCanvasGroup(_groupHorizontal);
        }

        private void OnClicked(KeyCode code) => Clicked?.Invoke(code);
    }
}
