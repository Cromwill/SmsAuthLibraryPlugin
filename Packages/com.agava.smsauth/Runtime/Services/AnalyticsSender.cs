using System;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;

namespace Agava.Wink
{
    [Serializable, Preserve]
    public class AnalyticsSender
    {
        [SerializeField] private Button _checkSubscriptionButton;
        [SerializeField] private Button _closeOfferWinkKidsButton;
        [SerializeField] private Button _closeSubscribeProfileWindowButton;
        [SerializeField] private Button[] _offerWinkKidsButtons;
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
            _checkSubscriptionButton.onClick.AddListener(CheckSubscriptionButtonClick);
            _closeOfferWinkKidsButton.onClick.AddListener(CloseOfferWinkKidsButtonClick);
            _closeSubscribeProfileWindowButton.onClick.AddListener(CloseSubscribeProfileWindowButtonClick);

            foreach (Button button in _offerWinkKidsButtons)
            {
                button.onClick.AddListener(SendOfferWinkKidsButtonClick);
            }

            _subscribeWinkButton.onClick.AddListener(SendSubscribeWinkButtonClick);
            _deleteAccountButton.onClick.AddListener(SendDeleteAccountButtonClick);
            _subscriptionManagementButton.onClick.AddListener(SendSubscriptionManagementButtonClick);
            _subscribeButtonRewardButton.onClick.AddListener(SendSubscriptionManagementOnRewardWindowButtonClick);
            _rewardVideoButton.onClick.AddListener(SendViewingRewardAdButtonClick);

            foreach (Button button in _turnOffAdButtons)
            {
                button.onClick.AddListener(SendTurnOffAdOnDisableWindowButtonClick);
            }

            foreach (Button button in _learnMoreAboutSubscriptionButtons)
            {
                button.onClick.AddListener(SendLearnMoreSubsButtonClick);
            }

            foreach (Button button in _turnOffAdOnInfoButtons)
            {
                button.onClick.AddListener(SendTurnOffAdOnInfoWindowButtonClick);
            }

            foreach (Button button in _closeTurnOffAdWindowButtons)
            {
                button.onClick.AddListener(SendCloseTurnOffAdWindowButtonClick);
            }

            foreach (Button button in _closeTurnOffAdInfoWindowButtons)
            {
                button.onClick.AddListener(SendCloseTurnOffAdInfoWindowButtonClick);
            }
        }

        public void Dispose()
        {
            _checkSubscriptionButton.onClick.RemoveListener(CheckSubscriptionButtonClick);
            _closeOfferWinkKidsButton.onClick.RemoveListener(CloseOfferWinkKidsButtonClick);
            _closeSubscribeProfileWindowButton.onClick.RemoveListener(CloseSubscribeProfileWindowButtonClick);

            foreach (Button button in _offerWinkKidsButtons)
            {
                button.onClick.RemoveListener(SendOfferWinkKidsButtonClick);
            }

            _subscribeWinkButton.onClick.RemoveListener(SendSubscribeWinkButtonClick);
            _deleteAccountButton.onClick.RemoveListener(SendDeleteAccountButtonClick);
            _subscriptionManagementButton.onClick.RemoveListener(SendSubscriptionManagementButtonClick);
            _subscribeButtonRewardButton.onClick.RemoveListener(SendSubscriptionManagementOnRewardWindowButtonClick);
            _rewardVideoButton.onClick.RemoveListener(SendViewingRewardAdButtonClick);

            foreach (Button button in _turnOffAdButtons)
            {
                button.onClick.RemoveListener(SendTurnOffAdOnDisableWindowButtonClick);
            }

            foreach (Button button in _learnMoreAboutSubscriptionButtons)
            {
                button.onClick.RemoveListener(SendLearnMoreSubsButtonClick);
            }

            foreach (Button button in _turnOffAdOnInfoButtons)
            {
                button.onClick.RemoveListener(SendTurnOffAdOnInfoWindowButtonClick);
            }

            foreach (Button button in _closeTurnOffAdWindowButtons)
            {
                button.onClick.RemoveListener(SendCloseTurnOffAdWindowButtonClick);
            }

            foreach (Button button in _closeTurnOffAdInfoWindowButtons)
            {
                button.onClick.RemoveListener(SendCloseTurnOffAdInfoWindowButtonClick);
            }
        }

        public void SetAdInfo(bool adEvents)
        {
            _adEvents = adEvents;
        }

        private void CheckSubscriptionButtonClick()
        {
            AnalyticsWinkService.SendCheckSubscriptionButtonClick();
        }

        private void CloseSubscribeProfileWindowButtonClick()
        {
            AnalyticsWinkService.SendCloseSubscribeProfileWindow();
        }

        private void SendSubscribeWinkButtonClick()
        {
            AnalyticsWinkService.SendSubscribeWinkButtonClick();
        }

        private void SendDeleteAccountButtonClick()
        {
            AnalyticsWinkService.SendDeleteAccountButtonClick();
        }

        private void SendSubscriptionManagementButtonClick()
        {
            AnalyticsWinkService.SendSubscriptionManagementButtonClick();
        }

        private void SendSubscriptionManagementOnRewardWindowButtonClick()
        {
            AnalyticsWinkService.SendSubscriptionManagementOnRewardWindowButtonClick();
        }

        private void SendViewingRewardAdButtonClick()
        {
            AnalyticsWinkService.SendViewingRewardAdButtonClick();
        }

        private void SendTurnOffAdOnDisableWindowButtonClick()
        {
            AnalyticsWinkService.SendTurnOffAdOnDisableWindowButtonClick();
        }

        private void SendLearnMoreSubsButtonClick()
        {
            AnalyticsWinkService.SendLearnMoreSubsButtonClick();
        }

        private void SendCloseTurnOffAdWindowButtonClick()
        {
            AnalyticsWinkService.SendCloseTurnOffAdWindowButtonClick();
        }

        private void CloseOfferWinkKidsButtonClick()
        {
            if (_adEvents == false)
            {
                AnalyticsWinkService.SendCloseOfferWinkKids();
            }
        }

        private void SendOfferWinkKidsButtonClick()
        {
            if (_adEvents == false)
            {
                AnalyticsWinkService.SendOfferWinkKidsButtonClick();
            }
        }

        private void SendTurnOffAdOnInfoWindowButtonClick()
        {
            if (_adEvents)
            {
                AnalyticsWinkService.SendTurnOffAdOnInfoWindowButtonClick();
            }
        }

        private void SendCloseTurnOffAdInfoWindowButtonClick()
        {
            if (_adEvents)
            {
                AnalyticsWinkService.SendCloseTurnOffAdInfoWindowButtonClick();
            }
        }
    }
}
