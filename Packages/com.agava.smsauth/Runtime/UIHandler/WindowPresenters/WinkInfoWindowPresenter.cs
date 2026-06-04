using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Scripting;
using System.Collections.Generic;

namespace Agava.Wink
{
    [Preserve]
    internal class WinkInfoWindowPresenter : WindowPresenter
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private List<Button> _freeTrialButtons;
        [SerializeField] private Button _closeButton;
        [SerializeField] private List<XmlConfigText> _xmlConfigTexts;
        [SerializeField] private ScrollRect _scroll;
        [Header("AD variant")]
        [SerializeField] private List<XmlConfigSelectableText> _xmlConfigSelectableText;

        private bool _isAdVariant = false;

        public Button CloseButton => _closeButton;

        public event Action CloseButtonClicked;
        public event Action FreeTrialButtonClicked;

        private void Awake()
        {
            _closeButton.onClick.AddListener(CloseButtonClick);
            _freeTrialButtons.ForEach(b => b.onClick.AddListener(FreeTrialPlay));
        }

        private void OnDestroy()
        {
            _closeButton.onClick.RemoveListener(CloseButtonClick);
            _freeTrialButtons.ForEach(b => b.onClick.RemoveListener(FreeTrialPlay));
        }

        public void FillRemoteTexts() => _xmlConfigTexts.ForEach(t => t.FillText());

        public override void Enable()
        {
            _isAdVariant = false;
            _scroll.verticalNormalizedPosition = 1f;
            EnableCanvasGroup(_canvasGroup);
            AnalyticsWinkService.SendShowOfferWinkKidsWindow();
        }

        public override void Disable() => DisableCanvasGroup(_canvasGroup);

        public virtual void EnableAdVariant()
        {
            _closeButton.gameObject.SetActive(true);
            _isAdVariant = true;
            _scroll.verticalNormalizedPosition = 1f;
            InstallAdTexts();
            EnableCanvasGroup(_canvasGroup);
            AnalyticsWinkService.SendShowTurnOffAdInfoWindow();
        }

        public void InstallAdTexts()
        {
            if (_xmlConfigSelectableText.Count > 0)
                _xmlConfigSelectableText.ForEach(t => t.UseAdVariantText());
        }

        public void InstallOriginalTexts()
        {
            if (_xmlConfigSelectableText.Count > 0)
                _xmlConfigSelectableText.ForEach(t => t.SetOriginalText());
        }

        private void CloseButtonClick()
        {
            CloseButtonClicked?.Invoke();
        }

        private void FreeTrialPlay()
        {
            FreeTrialButtonClicked?.Invoke();
        }
    }
}
