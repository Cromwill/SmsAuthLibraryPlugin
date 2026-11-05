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
        [SerializeField] private Image _openImage;
        [SerializeField] private Sprite _bears;
        [SerializeField] private Sprite _cats;
        [SerializeField] private Sprite _fairyPatrol;
        [SerializeField] private Sprite _leoAndTig;
        [SerializeField] private Sprite _kubokot;
        [SerializeField] private List<XmlConfigText> _xmlConfigTexts;

        public void Construct(AppAuthenticator AppAuthenticator)
        {
            switch (AppAuthenticator)
            {
                default:
                    _openImage.sprite = _bears;
                    break;

                case AppAuthenticator.Kubokot:
                case AppAuthenticator.LogicLike:
                    _openImage.sprite = _kubokot;
                    break;

                case AppAuthenticator.LeoAndTig:
                case AppAuthenticator.LeoAndTigTaiga:
                    _openImage.sprite = _leoAndTig;
                    break;

                case AppAuthenticator.FairytalePatrolAdventure:
                case AppAuthenticator.MusicalPatrol:
                case AppAuthenticator.FairytalePatrolCafe:
                case AppAuthenticator.FairytalePatrol:
                    _openImage.sprite = _fairyPatrol;
                    break;

                case AppAuthenticator.ThreeCatsAdventure:
                case AppAuthenticator.ThreeCatsRacing:
                case AppAuthenticator.ThreeCatsPuzzles:
                    _openImage.sprite = _cats;
                    break;
            }
        }

        public void FillRemoteTexts() => _xmlConfigTexts.ForEach(t => t.FillText());

        public override void Enable() => EnableCanvasGroup(_canvasGroup);

        public override void Disable() => DisableCanvasGroup(_canvasGroup);
    }
}
