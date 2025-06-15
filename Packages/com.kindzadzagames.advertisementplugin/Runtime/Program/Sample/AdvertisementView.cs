using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if YABBI_AD
using YabbiSDK.Api;
#endif

namespace KinDzaDzaGames.AdvertisementPlugin
{
    public abstract class AdvertisementView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Button _loadAdButton;
        [SerializeField] private Button _showAdButton;
        [SerializeField] private Button _destroyAdButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private TMP_Text _logger;

        protected virtual void OnEnable()
        {
            _loadAdButton.onClick.AddListener(LoadButtonClick);
            _showAdButton.onClick.AddListener(ShowButtonClick);
            _destroyAdButton.onClick.AddListener(DestroyButtonClick);
            _closeButton.onClick.AddListener(Hide);
        }

        protected virtual void OnDisable()
        {
            _loadAdButton.onClick.RemoveListener(LoadButtonClick);
            _showAdButton.onClick.RemoveListener(ShowButtonClick);
            _destroyAdButton.onClick.RemoveListener(DestroyButtonClick);
            _closeButton.onClick.RemoveListener(Hide);
        }

        public virtual void Show()
        {
            _logger.text = string.Empty;
            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;
        }


        protected virtual void Hide()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;

            DestroyButtonClick();
            _logger.text = string.Empty;
        }

        protected virtual void ShowButtonClick()
        {
#if YABBI_AD
            if (Yabbi.IsAdLoaded(GetAdType(), GetPlacementName()))
                Yabbi.ShowAd(GetAdType(), GetPlacementName());
            else
#endif
                ShowNotLoadedADWarning();
        }

        protected virtual void ShowNotLoadedADWarning()
        {
            AddLog("Ad is not loaded yet");
        }

        protected void AddLog(string message)
        {
            var current = _logger.text;
            _logger.text = $"{current}\n{message}";
        }

        protected abstract string GetPlacementName();
        protected abstract int GetAdType();

        private void LoadButtonClick()
        {
#if YABBI_AD
            if (Yabbi.CanLoadAd(GetAdType(), GetPlacementName()))
            {
                AddLog("Ad start to load.");
                Yabbi.LoadAd(GetAdType(), GetPlacementName());
            }
            else
            {
                AddLog("SDK can't start load ad.");
            }
#endif
        }

        private void DestroyButtonClick()
        {
#if YABBI_AD
            Yabbi.DestroyAd(GetAdType(), GetPlacementName());
#endif
            AddLog("Ad was destroyed.");
        }
    }
}
