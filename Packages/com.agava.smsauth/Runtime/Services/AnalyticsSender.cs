using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Scripting;

namespace Agava.Wink
{
    [Serializable, Preserve]
    public class AnalyticsSender
    {
        [SerializeField] private Button[] _haveSubscriptionButtons;
        [SerializeField] private Button[] _tryFreeTrialButtons;
        [SerializeField] private Button[] _offerWinkKidsButtons;
        [SerializeField] private Button _subscribeWinkButton;
        [SerializeField] private Button _deleteAccountButton;

        public void Construct()
        {
            foreach (var button in _haveSubscriptionButtons)
                button.onClick.AddListener(SendHaveSubscriptionButtonClick);

            foreach (var button in _tryFreeTrialButtons)
                button.onClick.AddListener(SendTryFreeTrialButtonClick);

            foreach (var button in _offerWinkKidsButtons)
                button.onClick.AddListener(SendOfferWinkKidsButtonClick);

            _subscribeWinkButton.onClick.AddListener(SendSubscribeWinkButtonClick);
            _deleteAccountButton.onClick.AddListener(SendDeleteAccountButtonClick);
        }

        public void Dispose()
        {
            foreach (var button in _haveSubscriptionButtons)
                button.onClick.RemoveListener(SendHaveSubscriptionButtonClick);

            foreach (var button in _tryFreeTrialButtons)
                button.onClick.RemoveListener(SendTryFreeTrialButtonClick);

            foreach (var button in _offerWinkKidsButtons)
                button.onClick.RemoveListener(SendOfferWinkKidsButtonClick);

            _subscribeWinkButton.onClick.RemoveListener(SendSubscribeWinkButtonClick);
            _deleteAccountButton.onClick.RemoveListener(SendDeleteAccountButtonClick);
        }

        private void SendHaveSubscriptionButtonClick() => AnalyticsWinkService.SendHaveWinkButtonClick();
        private void SendTryFreeTrialButtonClick() => AnalyticsWinkService.SendPayWallRedirect();
        private void SendOfferWinkKidsButtonClick() => AnalyticsWinkService.SendOfferWinkKidsButtonClick();
        private void SendSubscribeWinkButtonClick() => AnalyticsWinkService.SendSubscribeWinkButtonClick();
        private void SendDeleteAccountButtonClick() => AnalyticsWinkService.SendDeleteAccountButtonClick();
    }
}
