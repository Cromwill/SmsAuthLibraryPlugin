using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Scripting;

namespace Agava.Wink
{
    [Serializable, Preserve]
    public class AnalyticsSender
    {
        [SerializeField] private Button _checkSubscriptionButton;
        [SerializeField] private Button _closeOfferWinkKidsButton;
        [SerializeField] private Button _closeSubscribeProfileWindowButton;
        //[SerializeField] private Button[] _haveSubscriptionButtons;
        //[SerializeField] private Button[] _tryFreeTrialButtons;
        [SerializeField] private Button[] _offerWinkKidsButtons;
        //[SerializeField] private Button _closeStartWindowButton;
        [SerializeField] private Button _subscribeWinkButton;
        [SerializeField] private Button _deleteAccountButton;
        [SerializeField] private Button _subscriptionManagementButton;
        [SerializeField] private Button _subscribeButtonRewardButton;
        [SerializeField] private Button _rewardVideoButton;
        [SerializeField] private Button[] _turnOffAdButtons;
        [SerializeField] private Button[] _learnMoreAboutSubscriptionButtons;
        [SerializeField] private Button[] _turnOffAdOnInfoButtons;
        [SerializeField] private Button[] _closeTurnOffAdWindowButtons;
        [SerializeField] private Button[] _closeTurnOffAdInfoWindowButtons;

        private bool _adEvents = false;

        public void Construct()
        {
            /*foreach (var button in _haveSubscriptionButtons)
                button.onClick.AddListener(SendHaveSubscriptionButtonClick);

            foreach (var button in _tryFreeTrialButtons)
                button.onClick.AddListener(SendTryFreeTrialButtonClick);*/

            _checkSubscriptionButton.onClick.AddListener(CheckSubscriptionButtonClick);
            _closeOfferWinkKidsButton.onClick.AddListener(CloseOfferWinkKidsButtonClick);
            _closeSubscribeProfileWindowButton.onClick.AddListener(CloseSubscribeProfileWindowButtonClick);

            foreach (var button in _offerWinkKidsButtons)
                button.onClick.AddListener(SendOfferWinkKidsButtonClick);

            //_closeStartWindowButton.onClick.AddListener(SendCloseStartWindowButtonClick);
            _subscribeWinkButton.onClick.AddListener(SendSubscribeWinkButtonClick);
            _deleteAccountButton.onClick.AddListener(SendDeleteAccountButtonClick);
            _subscriptionManagementButton.onClick.AddListener(SendSubscriptionManagementButtonClick);
            _subscribeButtonRewardButton.onClick.AddListener(SendSubscriptionManagementOnRewardWindowButtonClick);
            _rewardVideoButton.onClick.AddListener(SendViewingRewardAdButtonClick);

            foreach (var button in _turnOffAdButtons)
                button.onClick.AddListener(SendTurnOffAdOnDisableWindowButtonClick);

            foreach (var button in _learnMoreAboutSubscriptionButtons)
                button.onClick.AddListener(SendLearnMoreSubsButtonClick);

            foreach (var button in _turnOffAdOnInfoButtons)
                button.onClick.AddListener(SendTurnOffAdOnInfoWindowButtonClick);

            foreach (var button in _closeTurnOffAdWindowButtons)
                button.onClick.AddListener(SendCloseTurnOffAdWindowButtonClick);

            foreach (var button in _closeTurnOffAdInfoWindowButtons)
                button.onClick.AddListener(SendCloseTurnOffAdInfoWindowButtonClick);
        }

        public void Dispose()
        {
            /*foreach (var button in _haveSubscriptionButtons)
                button.onClick.RemoveListener(SendHaveSubscriptionButtonClick);

            foreach (var button in _tryFreeTrialButtons)
                button.onClick.RemoveListener(SendTryFreeTrialButtonClick);*/

            _checkSubscriptionButton.onClick.RemoveListener(CheckSubscriptionButtonClick);
            _closeOfferWinkKidsButton.onClick.RemoveListener(CloseOfferWinkKidsButtonClick);
            _closeSubscribeProfileWindowButton.onClick.RemoveListener(CloseSubscribeProfileWindowButtonClick);

            foreach (var button in _offerWinkKidsButtons)
                button.onClick.RemoveListener(SendOfferWinkKidsButtonClick);

            //_closeStartWindowButton.onClick.RemoveListener(SendCloseStartWindowButtonClick);
            _subscribeWinkButton.onClick.RemoveListener(SendSubscribeWinkButtonClick);
            _deleteAccountButton.onClick.RemoveListener(SendDeleteAccountButtonClick);
            _subscriptionManagementButton.onClick.RemoveListener(SendSubscriptionManagementButtonClick);
            _subscribeButtonRewardButton.onClick.RemoveListener(SendSubscriptionManagementOnRewardWindowButtonClick);
            _rewardVideoButton.onClick.RemoveListener(SendViewingRewardAdButtonClick);

            foreach (var button in _turnOffAdButtons)
                button.onClick.RemoveListener(SendTurnOffAdOnDisableWindowButtonClick);

            foreach (var button in _learnMoreAboutSubscriptionButtons)
                button.onClick.RemoveListener(SendLearnMoreSubsButtonClick);

            foreach (var button in _turnOffAdOnInfoButtons)
                button.onClick.RemoveListener(SendTurnOffAdOnInfoWindowButtonClick);

            foreach (var button in _closeTurnOffAdWindowButtons)
                button.onClick.RemoveListener(SendCloseTurnOffAdWindowButtonClick);

            foreach (var button in _closeTurnOffAdInfoWindowButtons)
                button.onClick.RemoveListener(SendCloseTurnOffAdInfoWindowButtonClick);
        }

        public void SetAdInfo(bool adEvents) => _adEvents = adEvents;

        private void CheckSubscriptionButtonClick() => AnalyticsWinkService.SendCheckSubscriptionButtonClick();
        private void CloseSubscribeProfileWindowButtonClick() => AnalyticsWinkService.SendCloseSubscribeProfileWindow();
        //private void SendHaveSubscriptionButtonClick() => AnalyticsWinkService.SendHaveWinkButtonClick();
        //private void SendTryFreeTrialButtonClick() => AnalyticsWinkService.SendPayWallRedirect();
        //private void SendCloseStartWindowButtonClick() => AnalyticsWinkService.SendCloseStartWindow();
        private void SendSubscribeWinkButtonClick() => AnalyticsWinkService.SendSubscribeWinkButtonClick();
        private void SendDeleteAccountButtonClick() => AnalyticsWinkService.SendDeleteAccountButtonClick();
        private void SendSubscriptionManagementButtonClick() => AnalyticsWinkService.SendSubscriptionManagementButtonClick();
        private void SendSubscriptionManagementOnRewardWindowButtonClick() => AnalyticsWinkService.SendSubscriptionManagementOnRewardWindowButtonClick();
        private void SendViewingRewardAdButtonClick() => AnalyticsWinkService.SendViewingRewardAdButtonClick();
        private void SendTurnOffAdOnDisableWindowButtonClick() => AnalyticsWinkService.SendTurnOffAdOnDisableWindowButtonClick();
        private void SendLearnMoreSubsButtonClick() => AnalyticsWinkService.SendLearnMoreSubsButtonClick();
        private void SendCloseTurnOffAdWindowButtonClick() => AnalyticsWinkService.SendCloseTurnOffAdWindowButtonClick();

        private void CloseOfferWinkKidsButtonClick()
        {
            if (_adEvents == false)
                AnalyticsWinkService.SendCloseOfferWinkKids();
        }

        private void SendOfferWinkKidsButtonClick()
        {
            if(_adEvents == false)
                AnalyticsWinkService.SendOfferWinkKidsButtonClick();
        }

        private void SendTurnOffAdOnInfoWindowButtonClick()
        {
            if (_adEvents)
                AnalyticsWinkService.SendTurnOffAdOnInfoWindowButtonClick();
        }

        private void SendCloseTurnOffAdInfoWindowButtonClick()
        {
            if (_adEvents)
                AnalyticsWinkService.SendCloseTurnOffAdInfoWindowButtonClick();
        }
    }
}
