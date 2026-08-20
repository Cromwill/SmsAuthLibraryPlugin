using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Scripting;

namespace Agava.Wink
{
    [Preserve]
    internal class WinkProfileWindow : WindowPresenter
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private ImagesCarousel _imagesCarousel;
        [SerializeField] private Button _profileButton;
        [SerializeField] private Button _closeButton;

        public void ConstructCorousel(CarouselSettings carouselSettings) => _imagesCarousel.Construct(carouselSettings);

        private void OnDestroy()
        {
            _closeButton?.onClick.RemoveListener(Disable);
            _profileButton.onClick.RemoveListener(OnProfileButtonClicked);
        }

        private void Awake()
        {
            _closeButton?.onClick.AddListener(Disable);
            _profileButton.onClick.AddListener(OnProfileButtonClicked);
        }

        public override void Enable()
        {
            _imagesCarousel.Enable();
            EnableCanvasGroup(_canvasGroup);
            AnalyticsWinkService.SendSubscriptionManagementWindow();
        }

        public override void Disable()
        {
            DisableCanvasGroup(_canvasGroup);
            _imagesCarousel.Disable();
        }

        private void OnProfileButtonClicked()
        {
            Application.OpenURL(Links.Instance.Subscription);
            Disable();
        }
    }
}
