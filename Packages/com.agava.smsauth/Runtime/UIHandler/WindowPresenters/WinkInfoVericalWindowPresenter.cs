using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Scripting;
using KinDzaDzaGames.AdvertisementPlugin;

namespace Agava.Wink
{
    [Preserve]
    internal class WinkInfoVericalWindowPresenter : WinkInfoWindowPresenter
    {
        [SerializeField] private Button _rewardButton;

        public void HideCloseButton()
        {
            CloseButton.gameObject.SetActive(false);
            _rewardButton.gameObject.SetActive(false);
        }

        public void TryShowCloseButton(bool enabled)
        {
            CloseButton.gameObject.SetActive(enabled);

#if UNITY_EDITOR
            if (enabled == false)
                _rewardButton.gameObject.SetActive(true);
#else
            if (enabled == false && AdvertisementController.Instance != null)
                _rewardButton.gameObject.SetActive(AdvertisementController.Instance.CanShowReward());
#endif
        }

        public override void EnableAdVariant()
        {
            _rewardButton.gameObject.SetActive(false);
            base.EnableAdVariant();
        }
    }
}
