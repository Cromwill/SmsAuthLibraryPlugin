using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Scripting;
using System.Collections.Generic;
using KinDzaDzaGames.AdvertisementPlugin;

namespace Agava.Wink
{
    [Preserve]
    internal class RedirectWindowPresenter : WindowPresenter
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private ImagesCarousel _imagesCarousel;
        [SerializeField] private Button _yesButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _rewardButton;
        [SerializeField] private Button _signInButton;
        [SerializeField] private bool _closeOnYesClicked = true;
        [SerializeField] private XmlCarouselConfigTexts _xmlCarouselConfigTexts;

        public bool TryFreeWink { get; private set; } = false;

        private void Awake()
        {
            _closeButton?.onClick.AddListener(OnCloseButtonClick);
            _yesButton.onClick.AddListener(OnYesClicked);
            _signInButton.onClick.AddListener(ResetFreeChoise);
        }

        private void OnDestroy()
        {
            _closeButton?.onClick.RemoveListener(OnCloseButtonClick);
            _yesButton.onClick.RemoveListener(OnYesClicked);
            _signInButton.onClick.RemoveListener(ResetFreeChoise);
        }

        public void Enable(bool closeButton)
        {
            TryFreeWink = false;
            _imagesCarousel.PositionChanged += OnPositionChanged;
            _imagesCarousel.Enable();
            EnableCanvasGroup(_canvasGroup);
        }

        public override void Enable() => Enable(true);

        public override void Disable()
        {
            DisableCanvasGroup(_canvasGroup);
            _imagesCarousel.Disable();
            _imagesCarousel.PositionChanged -= OnPositionChanged;
        }

        public void FillRemoteTexts() => _xmlCarouselConfigTexts.FillTexts();

        public void TryShowCloseButton(bool enabled)
        {
            _closeButton.gameObject.SetActive(enabled);
        }

        public void TryShowRewardButton(bool enabled)
        {
#if UNITY_EDITOR && YABBI_AD == false && YANDEX_AD == false
            if (enabled == false)
                _rewardButton.gameObject.SetActive(true);
#else
            if (enabled == false && AdvertisementController.Instance != null)
                _rewardButton.gameObject.SetActive(AdvertisementController.Instance.CanShowReward());
#endif
        }

        public void ResetFreeChoise() => TryFreeWink = false;
        public void EnableFreeChoise() => TryFreeWink = true;

        private void OnYesClicked()
        {
            TryFreeWink = true;

            if (_closeOnYesClicked)
                Disable();
        }

        private void OnCloseButtonClick()
        {
            TryFreeWink = false;
            Disable();
        }

        private void OnPositionChanged(CarouselID carouselID)
        {
            _xmlCarouselConfigTexts.SetText(carouselID);
        }
    }
}
