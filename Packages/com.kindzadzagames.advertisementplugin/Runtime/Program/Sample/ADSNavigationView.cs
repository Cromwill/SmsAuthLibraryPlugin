using UnityEngine;
using UnityEngine.UI;

namespace KinDzaDzaGames.AdvertisementPlugin
{
    public class ADSNavigationView : MonoBehaviour
    {
        [SerializeField] private InterstitialView _interstitialView;
        [SerializeField] private Button _interstitialButton;
        [SerializeField] private RewardView _rewardView;
        [SerializeField] private Button _rewardButton;
        [SerializeField] private BannerView _bannerView;
        [SerializeField] private Button _bannerButton;

        public void Construct()
        {
            _interstitialView.Construct();
            _rewardView.Construct();
            _bannerView.Construct();
        }

        private void OnEnable()
        {
            _interstitialButton.onClick.AddListener(_interstitialView.Show);
            _rewardButton.onClick.AddListener(_rewardView.Show);
            _bannerButton.onClick.AddListener(_bannerView.Show);
        }

        private void OnDisable()
        {
            _interstitialButton.onClick.RemoveListener(_interstitialView.Show);
            _rewardButton.onClick.RemoveListener(_rewardView.Show);
            _bannerButton.onClick.RemoveListener(_bannerView.Show);
        }
    }
}
