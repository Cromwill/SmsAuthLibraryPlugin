using UnityEngine;
using AdsAppView.Utility;
using System.Collections.Generic;

namespace AdsAppView.Program
{
    public class AppUpdatePresenter : MonoBehaviour
    {
        [SerializeField] private PortraitUpdatePopup _portraitUpdatePopup;
        [SerializeField] private LandscapeUpdatePopup _landscapeUpdatePopup;
        [SerializeField] private List<XmlConfigText> _xmlConfigTexts;
        [SerializeField] private List<ClickableSupportText> _clickableSupportTexts;

        private AppOrientation _appOrientation;
        private bool _activated = false;

        public void Construct(AppOrientation appOrientation)
        {
            _appOrientation = appOrientation;
        }

        public void Dispose()
        {
            if (_activated == false)
                return;

            _portraitUpdatePopup.Dispose();
            _landscapeUpdatePopup.Dispose();
        }

        public void FiiRemoteTexts(string supportLink)
        {
            _xmlConfigTexts.ForEach(t => t.FillText());
            _clickableSupportTexts.ForEach(t => t.Construct(supportLink));
        }

        public void Activate(string appLink)
        {
            _activated = true;
            _portraitUpdatePopup.Construct(appLink);
            _landscapeUpdatePopup.Construct(appLink);

            if (_appOrientation == AppOrientation.Portrait)
                _portraitUpdatePopup.gameObject.SetActive(true);
            else
                _landscapeUpdatePopup.gameObject.SetActive(true);

            AnalyticsService.SendUpdateWindow();
        }
    }
}
