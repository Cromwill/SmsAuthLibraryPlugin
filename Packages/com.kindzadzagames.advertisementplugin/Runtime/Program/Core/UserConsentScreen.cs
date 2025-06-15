using System;
using UnityEngine;
using UnityEngine.Scripting;
#if YABBI_AD
using SspnetSDK.ConsentManagerSDK.Unfiled;
#endif

namespace KinDzaDzaGames.AdvertisementPlugin
{
    [Preserve, Serializable]
    public class UserConsentScreen
#if YABBI_AD
        : IConsentListener
#endif
    {
        private const string YabbyAgreement = nameof(YabbyAgreement);
        private const int HasAccepted = 1;
        private const int NotAccepted = 0;

        [field: SerializeField] public bool NeedShowConsentScreen { get; private set; } = false;

        public bool AgreementAccepted => PlayerPrefs.HasKey(YabbyAgreement);

        public void OnConsentManagerLoaded() { }
        public void OnConsentManagerLoadFailed(string error) { }
        public void OnConsentManagerShownFailed(string error) { }

        public void OnConsentWindowClosed(bool hasConsent) => PlayerPrefs.SetInt(YabbyAgreement, hasConsent ? HasAccepted : NotAccepted);
        public void OnConsentWindowShown() => PlayerPrefs.SetInt(YabbyAgreement, NotAccepted);
    }
}
