using UnityEngine;

namespace Agava.Wink
{
    public class LoadingWindowPresenter : WindowPresenter
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private ImagesCarousel _imagesCarousel;

        public void ConstructCorousel(CarouselSettings carouselSettings) => _imagesCarousel.Construct(carouselSettings);

        public override void Enable()
        {
            _imagesCarousel.Enable();
            EnableCanvasGroup(_canvasGroup);
        }

        public override void Disable()
        {
            DisableCanvasGroup(_canvasGroup);
            _imagesCarousel.Disable();
        }
    }
}
