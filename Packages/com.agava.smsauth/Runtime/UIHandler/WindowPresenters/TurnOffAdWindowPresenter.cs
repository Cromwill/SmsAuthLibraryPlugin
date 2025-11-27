using TMPro;
using UnityEngine;
using System.Collections.Generic;

namespace Agava.Wink
{
    public class TurnOffAdWindowPresenter : WindowPresenter
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private ImagesCarousel _imagesCarousel;
        [SerializeField] private TMP_Text _label;
        [SerializeField] private List<XmlConfigText> _xmlConfigTexts;

        public void Construct(RewardSettings rewardSettings) => _label.text = rewardSettings.turn_off_ad_offer_wink_text;

        public override void Disable()
        {
            DisableCanvasGroup(_canvasGroup);
            _imagesCarousel.Disable();
        }

        public override void Enable()
        {
            _imagesCarousel.Enable();
            EnableCanvasGroup(_canvasGroup);

            AnalyticsWinkService.SendShowTurnOffAdWindow();
        }

        public void FillRemoteTexts() => _xmlConfigTexts.ForEach(t => t.FillText());
    }
}
