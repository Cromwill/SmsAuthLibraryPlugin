using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Scripting;
using System.Collections.Generic;

namespace Agava.Wink
{
    [Preserve]
    public class AppUpdatePresenter : WindowPresenter
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Image _openImage;
        [SerializeField] private Sprite _bears;
        [SerializeField] private Sprite _cats;
        [SerializeField] private Sprite _fairyPatrol;
        [SerializeField] private Sprite _leoAndTig;
        [SerializeField] private Sprite _kubokot;
        [SerializeField] private List<XmlConfigText> _xmlConfigTexts;
        [SerializeField] private Button _downloadButton;

        private string _appLink;

        public void Construct(AppAuthenticator AppAuthenticator)
        {
            _downloadButton.onClick.AddListener(OpenLink);

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

        public void Dispose() => _downloadButton.onClick.RemoveListener(OpenLink);

        public void FillRemoteTexts() => _xmlConfigTexts.ForEach(t => t.FillText());

        public void Open(string appLink)
        {
            AnalyticsWinkService.SendUpdateWindow();
            _appLink = appLink;
            Enable();
        }

        public override void Enable() => EnableCanvasGroup(_canvasGroup);
        public override void Disable() => DisableCanvasGroup(_canvasGroup);

        private void OpenLink()
        {
            AnalyticsWinkService.SendUpdateButtonClick();
            Application.OpenURL(_appLink);
        }
    }
}
