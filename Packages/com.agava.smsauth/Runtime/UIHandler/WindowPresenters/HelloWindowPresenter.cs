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
        [SerializeField] private ImagesCarousel _imagesCarousel;
        [SerializeField] private Button _startButton;

        private Action _onEnd;

        public void Enable(Action onEnd = null)
        {
            _imagesCarousel.Enable();
            EnableCanvasGroup(_canvasGroup);
            _onEnd = onEnd;
            _startButton.onClick.AddListener(OnStartButtonClick);
        }

        public override void Enable() { }

        public override void Disable()
        {
            DisableCanvasGroup(_canvasGroup);
            _imagesCarousel.Disable();
            _startButton.onClick.RemoveListener(OnStartButtonClick);
        }

        private void OnStartButtonClick()
        {
            _onEnd?.Invoke();
            Disable();
        }
    }
}
