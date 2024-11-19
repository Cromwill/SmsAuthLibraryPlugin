using System.Collections;
using UnityEngine;
using System;
using UnityEngine.Scripting;
using UnityEngine.UI;

namespace Agava.Wink
{
    [Preserve]
    internal class HelloWindowPresenter : WindowPresenter
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Button _startButton;

        Action _onEnd;

        public void Enable(Action onEnd = null)
        {
            EnableCanvasGroup(_canvasGroup);
            _onEnd = onEnd;
            _startButton.onClick.AddListener(OnStartButtonClick);
        }

        public override void Enable() { }

        public override void Disable()
        {
            DisableCanvasGroup(_canvasGroup);
            _startButton.onClick.RemoveListener(OnStartButtonClick);
        }

        private void OnStartButtonClick()
        {
            Disable();
            _onEnd?.Invoke();
        }
    }
}
