using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Scripting;

namespace Agava.Wink
{
    [Preserve]
    internal class HelloWOAccessWindowPresenter : WindowPresenter
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private ImagesCarousel _imagesCarousel;
        [SerializeField] private Button _subscribeButton;
        [SerializeField] private Button _closeButton;

        public override void Enable()
        {
            _imagesCarousel.Enable();
            _subscribeButton.onClick.AddListener(OnSubscribeButtonClick);
            _closeButton.onClick.AddListener(OnCloseButtonClick);
            EnableCanvasGroup(_canvasGroup);
        }

        public override void Disable()
        {
            DisableCanvasGroup(_canvasGroup);
            _imagesCarousel.Disable();
            _subscribeButton.onClick.RemoveListener(OnSubscribeButtonClick);
            _closeButton.onClick.RemoveListener(OnCloseButtonClick);
        }

        public void TryShowCloseButton(bool enabled) => _closeButton.gameObject.SetActive(enabled);

        private void OnSubscribeButtonClick()
        {
            Disable();
        }

        private void OnCloseButtonClick()
        {
            Disable();
        }
    }
}
