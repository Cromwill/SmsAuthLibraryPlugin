using TMPro;
using System;
using UnityEngine;
using System.Collections;
using AdsAppView.Utility;
using UnityEngine.EventSystems;

namespace AdsAppView.Program
{
    public class ClickableSupportText : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TMP_Text _tmpText;

        private string _linkId;
        private string _link;
        private Action _onClick;

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => Links.Instance != null);
            yield return new WaitUntil(() => Links.Instance.Initialized);

            _linkId = "support";
            _link = Links.Instance.Support;
            Debug.Log($"_link = {_link}");
            _onClick = AnalyticsService.SendSupportLink;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (string.IsNullOrEmpty(_linkId) || string.IsNullOrEmpty(_link))
                return;

            int linkIndex = TMP_TextUtilities.FindIntersectingLink(_tmpText, eventData.position, eventData.pressEventCamera);

            if (linkIndex == -1)
                return;

            TMP_LinkInfo linkInfo = _tmpText.textInfo.linkInfo[linkIndex];
            string selectedLink = linkInfo.GetLinkID();

            if (selectedLink == _linkId)
            {
                Application.OpenURL(_link);
                _onClick?.Invoke();
            }
        }
    }
}
