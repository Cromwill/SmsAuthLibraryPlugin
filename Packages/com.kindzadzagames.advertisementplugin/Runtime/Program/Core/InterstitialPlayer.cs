using System;
using UnityEngine;

namespace KinDzaDzaGames.AdvertisementPlugin
{
    public class InterstitialPlayer : MonoBehaviour
    {
        InterstitialHandler _interstitialHandler;

        public void Construct(InterstitialHandler interstitialHandler)
        {
            _interstitialHandler = interstitialHandler ?? throw new ArgumentNullException(nameof(interstitialHandler));
        }

        public void Dispose()
        {

        }
    }
}
