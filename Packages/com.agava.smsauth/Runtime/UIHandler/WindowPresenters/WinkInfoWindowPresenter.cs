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
        [Header("AD variant")]
        [SerializeField] private List<XmlConfigSelectableText> _xmlConfigSelectableText;

        private bool _isAdVariant = false;

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
            EnableCanvasGroup(_canvasGroup);
            AnalyticsWinkService.SendShowOfferWinkKidsWindow();
        }

        public override void Disable() => DisableCanvasGroup(_canvasGroup);

        public void EnableAdVariant()
        {
            _isAdVariant = true;

            if(_xmlConfigSelectableText.Count > 0)
                _xmlConfigSelectableText.ForEach(t => t.UseAdVariantText());

            EnableCanvasGroup(_canvasGroup);
        }

        private void CloseButtonClick()
        {
            CloseButtonClicked?.Invoke();
            Disable();
        }

        private void FreeTrialPlay()
        {
            FreeTrialButtonClicked?.Invoke();
            Disable();
        }
    }
}
