using System;
using UnityEngine;
using KinDzaDzaGames.AdvertisementPlugin.DTO;

namespace KinDzaDzaGames.AdvertisementPlugin
{
    public class InterstitialPlayer : MonoBehaviour
    {
        private InterstitialHandler _interstitialHandler;
        private AdsSdkSettingsData _settingsData;

        public void Construct(InterstitialHandler interstitialHandler, AdsSdkSettingsData settingsData)
        {
            _interstitialHandler = interstitialHandler ?? throw new ArgumentNullException(nameof(interstitialHandler));
            _settingsData = settingsData ?? throw new ArgumentNullException(nameof(settingsData));

            _interstitialHandler.InterstitialClosed += OnInterstitialClosed;
        }

        public void Dispose()
        {
            _interstitialHandler.InterstitialClosed -= OnInterstitialClosed;
        }

        private void OnInterstitialClosed()
        {
            throw new NotImplementedException();
        }
    }
}
