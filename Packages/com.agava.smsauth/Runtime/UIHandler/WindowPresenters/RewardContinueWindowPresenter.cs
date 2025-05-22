using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Scripting;

namespace Agava.Wink
{
    [Preserve]
    internal class RewardContinueWindowPresenter : WindowPresenter
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private ImagesCarousel _imagesCarousel;
        [SerializeField] private Button _subscriptionButton;
        [SerializeField] private Button _rewardDemoTimeButton;

        private SubscriptionCheckWindowPresenter _subscriptionCheckWindow;
        private DemoTimer _demoTimer;

        public void Construct(SubscriptionCheckWindowPresenter subscriptionCheck, DemoTimer demoTimer)
        {
            _subscriptionCheckWindow = subscriptionCheck ?? throw new ArgumentNullException(nameof(subscriptionCheck));
            _demoTimer = demoTimer ?? throw new ArgumentNullException(nameof(demoTimer));
        }

        public override void Enable()
        {
            _imagesCarousel.Enable();
            _subscriptionButton.onClick.AddListener(OpenSubscriptionWindow);
            _rewardDemoTimeButton.onClick.AddListener(AddDemoTime);
            EnableCanvasGroup(_canvasGroup);
        }

        public override void Disable()
        {
            DisableCanvasGroup(_canvasGroup);
            _subscriptionButton.onClick.RemoveListener(OpenSubscriptionWindow);
            _rewardDemoTimeButton.onClick.RemoveListener(AddDemoTime);
            _imagesCarousel.Disable();
        }

        private void OpenSubscriptionWindow()
        {
            _subscriptionCheckWindow.Enable();
            Disable();
        }

        private void AddDemoTime()
        {
            _demoTimer.AddDemoTime(600);     // TODO: install remote time, 10 minuts
        }
    }
}
