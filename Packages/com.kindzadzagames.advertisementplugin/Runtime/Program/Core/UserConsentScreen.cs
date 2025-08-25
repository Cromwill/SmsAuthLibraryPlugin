using System;
using UnityEngine;
using UnityEngine.Scripting;

#if YABBI_AD
using YabbiSDK.ConsentManagerSDK.Api;
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
#if YABBI_AD
        private ConsentManager _consentManager;
#endif
        [field: SerializeField] public bool NeedShowConsentScreen { get; private set; } = false;

        public bool AgreementClosed { get; private set; } = false;

        public bool AgreementAccepted => PlayerPrefs.HasKey(YabbyAgreement);
#if YABBI_AD
        public void Construct(ConsentManager consentManager)
        {
            _consentManager = consentManager ?? throw new ArgumentNullException(nameof(consentManager));
        }

        public void OnConsentManagerLoaded() => _consentManager.ShowConsentWindow();
#endif
        public void OnConsentManagerLoadFailed(string error) { }
        public void OnConsentManagerShownFailed(string error) { }

        public void OnConsentWindowClosed(bool hasConsent)
        {
            PlayerPrefs.SetInt(YabbyAgreement, hasConsent ? HasAccepted : NotAccepted);
            AgreementClosed = true;
        }

        public void OnConsentWindowShown()
        {
            PlayerPrefs.SetInt(YabbyAgreement, NotAccepted);
            PlayerPrefs.Save();
        }

        public void CloseConcernScreen()
        {
            AgreementClosed = true;
        }
    }
}
