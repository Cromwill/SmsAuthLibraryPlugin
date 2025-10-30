using KinDzaDzaGames.AdvertisementPlugin;
using UnityEngine;
using UnityEngine.UI;

namespace Agava.Wink
{
    public class TurnOffAdOfferButton : MonoBehaviour
    {
        [SerializeField] private Button _button;

        private WinkSignInHandlerUI _winkSignInHandlerUI;
        private WinkAccessManager _winkAccessManager;

        private void Start()
        {
            _winkSignInHandlerUI = WinkSignInHandlerUI.Instance;
            _winkAccessManager = WinkAccessManager.Instance;
            UpdateButton();
        }

        private void FixedUpdate()
        {
            UpdateButton();
        }

        private void OnEnable() => _button.onClick.AddListener(OnButtonClick);

        private void OnDisable() => _button.onClick.RemoveListener(OnButtonClick);

        private void OnButtonClick() => _winkSignInHandlerUI.OpenTurnOffAdPanel();

        private void UpdateButton()
        {
            if (PreloadService.Instance == null || PreloadService.Instance.IsPluginAwailable == false)
            {
                _button.gameObject.SetActive(false);
                return;
            }

            if(WinkAccessManager.Instance.HasAccess || WinkAccessManager.Instance.HasTempAccess)
            {
                _button.gameObject.SetActive(false);
                return;
            }

            if(AdvertisementController.Instance == null)
            {
                _button.gameObject.SetActive(false);
                return;
            }

            _button.gameObject.SetActive(true);
        }
    }
}
