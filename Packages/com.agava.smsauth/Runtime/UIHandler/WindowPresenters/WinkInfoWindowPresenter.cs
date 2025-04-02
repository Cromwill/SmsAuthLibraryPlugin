using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Scripting;
using System.Collections.Generic;
using SmsAuthAPI.Utility;

namespace Agava.Wink
{
    [Preserve]
    internal class WinkInfoWindowPresenter : WindowPresenter
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private List<Button> _freeTrialButtons;
        [SerializeField] private Button _closeButton;

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

        public override void Enable()
        {
            EnableCanvasGroup(_canvasGroup);
            AnalyticsWinkService.SendShowOfferWinkKidsWindow();

            if (SheetRemoteConfigs.Texts != null)
            {
                Dictionary<string, string> data = SheetRemoteConfigs.Texts.Data["Hiking"]; //Key raw
                string text = data["Value"]; //Key column
                Debug.LogError("Test text - " + text);
            }
        }

        public override void Disable() => DisableCanvasGroup(_canvasGroup);

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
