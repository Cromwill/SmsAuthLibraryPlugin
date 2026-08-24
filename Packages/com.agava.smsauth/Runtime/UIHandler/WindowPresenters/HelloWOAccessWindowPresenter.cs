using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Scripting;
using System.Collections.Generic;
using KinDzaDzaGames.AdvertisementPlugin;
using System.Collections;

namespace Agava.Wink
{
    [Preserve]
    internal class HelloWOAccessWindowPresenter : WindowPresenter
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Button _subscribeButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _rewardButton;
        [SerializeField, Min(0)] private float _closeButtonDelay;
        [SerializeField] private List<XmlConfigText> _xmlConfigTexts;

        private Coroutine _waitCoroutine;
        private bool _closeButtonVisibility = false;

        public override void Enable()
        {
            _subscribeButton.onClick.AddListener(OnSubscribeButtonClick);
            _closeButton.onClick.AddListener(OnCloseButtonClick);
            EnableCanvasGroup(_canvasGroup);
            _waitCoroutine = StartCoroutine(WaitShowCloseButtons());
        }

        public override void Disable()
        {
            if (_waitCoroutine != null)
            {
                StopCoroutine(_waitCoroutine);
                _waitCoroutine = null;
            }

            DisableCanvasGroup(_canvasGroup);
            HideCloseButton();
            _subscribeButton.onClick.RemoveListener(OnSubscribeButtonClick);
            _closeButton.onClick.RemoveListener(OnCloseButtonClick);
        }

        public void FillRemoteTexts() => _xmlConfigTexts.ForEach(t => t.FillText());

        public void TryShowCloseButton(bool enabled)
        {
            _closeButtonVisibility = enabled;
        }

        public void HideCloseButton()
        {
            _closeButton.gameObject.SetActive(false);
            _rewardButton.gameObject.SetActive(false);
        }

        private void OnSubscribeButtonClick()
        {
            Disable();
        }

        private void OnCloseButtonClick()
        {
            Disable();
        }

        IEnumerator WaitShowCloseButtons()
        {
            yield return new WaitForSecondsRealtime(_closeButtonDelay);

            _closeButton.gameObject.SetActive(_closeButtonVisibility);

#if UNITY_EDITOR
            if (_closeButtonVisibility == false)
                _rewardButton.gameObject.SetActive(true);
#else
                if (_closeButtonVisibility == false && AdvertisementController.Instance != null)
                    _rewardButton.gameObject.SetActive(AdvertisementController.Instance.CanShowReward());
#endif
            _waitCoroutine = null;
        }
    }
}
