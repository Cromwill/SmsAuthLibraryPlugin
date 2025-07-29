using UnityEngine;
using System.Collections;
using UnityEngine.Scripting;
using KinDzaDzaGames.AdvertisementPlugin.DTO;

namespace KinDzaDzaGames.AdvertisementPlugin
{
    [Preserve]
    public class AdvertisementSelfStart : MonoBehaviour
    {
        [SerializeField] private SceneLoader _sceneLoader;
        [SerializeField] private LoadingCanvas _loadingProgressBar;
        [Header("Advertising configs")]
        [SerializeField] private AdvertisingConfigs _advertisingConfigs;
        [Header("Remote reward data")]
        [SerializeField] private RewardSettings _rewardSettings;
        [Header("Interstitial data")]
        [SerializeField] private int _firstTimer = 120;
        [SerializeField] private int _regularTimer = 60;
        [Header("Advertisement controller")]
        [SerializeField] private AdvertisementController _advertisementController;
        [Header("Internet check")]
        [SerializeField] private InternetChecker _internetChecker;

        private AdsSdkSettingsData _settings;

        private IEnumerator Start()
        {
            StartCoroutine(_internetChecker.EnternetChecking());

            if (Application.internetReachability == NetworkReachability.NotReachable)
                yield return new WaitWhile(() => Application.internetReachability == NetworkReachability.NotReachable);

            Debug.Log("#Boot Advertisement# : Self start advertisement plugin initialize");

            _settings = new AdsSdkSettingsData() { first_timer = _firstTimer, regular_timer = _regularTimer };
            _advertisementController.Construct(false, _rewardSettings, _settings, _advertisingConfigs);

            yield return new WaitUntil(() => _advertisementController.Initialized || _advertisementController.Breaked);

            if(_advertisementController.Breaked)
                Destroy(_advertisementController.gameObject);
            else if(_advertisementController.WaitConcernPolicy && _advertisementController.PolicyAccepted == false)
                yield return new WaitUntil(() => _advertisementController.AgreementClosed);

            AdvertisementController.Instance?.StartInterstitialTimer();
            _sceneLoader.LoadGameScene();
        }
    }
}
