using UnityEngine;
using UnityEngine.UI;
using AdsAppView.Utility;

namespace AdsAppView.Program
{
    public class LandscapeUpdatePopup : MonoBehaviour
    {
        [SerializeField] private Button _downloadButton;

        private string _appLink;

        public void Construct(string appLink)
        {
            _appLink = appLink;
            _downloadButton.onClick.AddListener(OpenLink);
        }

        public void Dispose() => _downloadButton.onClick.RemoveListener(OpenLink);

        private void OpenLink()
        {
            AnalyticsService.SendUpdateButtonClick();
            Application.OpenURL(_appLink);
        }
    }
}
