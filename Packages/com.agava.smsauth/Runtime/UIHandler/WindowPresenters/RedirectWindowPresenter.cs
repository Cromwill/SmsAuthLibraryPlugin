using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Scripting;
using System.Collections.Generic;

namespace Agava.Wink
{
    [Preserve]
    internal class RedirectWindowPresenter : WindowPresenter
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private ImagesCarousel _imagesCarousel;
        [SerializeField] private Button _yesButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _signInButton;
        [SerializeField] private bool _closeOnYesClicked = true;
        [SerializeField] private List<XmlConfigText> _xmlConfigTexts;

        public bool TryFreeWink { get; private set; } = false;

        private void Awake()
        {
            _closeButton?.onClick.AddListener(Disable);
            _yesButton.onClick.AddListener(OnYesClicked);
        }

        private void OnDestroy()
        {
            _closeButton?.onClick.RemoveAllListeners();
            _yesButton.onClick.RemoveAllListeners();
        }

        public void Enable(bool closeButton)
        {
            TryFreeWink = false;
            _imagesCarousel.Enable();
            EnableCanvasGroup(_canvasGroup);
        }

        public override void Enable() => Enable(true);

        public override void Disable()
        {
            DisableCanvasGroup(_canvasGroup);
            _imagesCarousel.Disable();
        }

        public void FillRemoteTexts() => _xmlConfigTexts.ForEach(t => t.FillText());

        public void TryShowCloseButton(bool enabled) => _closeButton.gameObject.SetActive(enabled);

        private void OnYesClicked()
        {
            TryFreeWink = true;

            if (_closeOnYesClicked)
                Disable();
        }
    }
}
