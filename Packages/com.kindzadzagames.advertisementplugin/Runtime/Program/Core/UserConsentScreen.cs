using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Scripting;
using System.Collections.Generic;
using KinDzaDzaGames.AdvertisementPlugin.ClickableTexts;
#if UNITY_EDITOR
using KinDzaDzaGames.AdvertisementPlugin.EditorScripts;
#endif

#if YABBI_AD
using YabbiSDK.ConsentManagerSDK.Api;
using SspnetSDK.ConsentManagerSDK.Unfiled;
#elif YANDEX_AD
using YandexMobileAds;
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
        private const string YandexAgreement = nameof(YandexAgreement);
        private const int HasAccepted = 1;
        private const int NotAccepted = 0;
        private const string YandexPP = "https://yandex.ru/legal/confidential/ru/";
#if YABBI_AD
        private ConsentManager _consentManager;
#endif
        [field: SerializeField] public bool NeedShowConsentScreen { get; private set; } = false;

#if UNITY_EDITOR
            [ReadOnly]
#endif
        [SerializeField] private CanvasGroup _canvasGroup;

#if UNITY_EDITOR
        [ReadOnly]
#endif
        [SerializeField] private Image _appImage;

#if UNITY_EDITOR && YANDEX_AD == false
        [ReadOnly]
#endif
        [SerializeField] private Sprite _appIcon;

#if UNITY_EDITOR
        [ReadOnly]
#endif
        [SerializeField] private TMP_Text _appLabelText;

#if UNITY_EDITOR
        [ReadOnly]
#endif
        [SerializeField] private ClickableText _appPrivacyPolicy;

#if UNITY_EDITOR
        [ReadOnly]
#endif
        [SerializeField] private ClickableText _yandexPrivacyPolicy;

#if UNITY_EDITOR
        [ReadOnly]
#endif
        [SerializeField] private List<Button> _disagreeButtons;

#if UNITY_EDITOR
        [ReadOnly]
#endif
        [SerializeField] private Button _agreeButton;

        public bool AgreementClosed { get; private set; } = false;

#if YABBI_AD
        public bool AgreementAccepted => PlayerPrefs.HasKey(YabbyAgreement);
#else
        public bool AgreementAccepted => PlayerPrefs.HasKey(YandexAgreement);
#endif

#if YABBI_AD
        public void Construct(ConsentManager consentManager)
        {
            _consentManager = consentManager ?? throw new ArgumentNullException(nameof(consentManager));
        }

        public void OnConsentManagerLoaded() => _consentManager.ShowConsentWindow();
        public void OnConsentManagerLoadFailed(string error) { }
        public void OnConsentManagerShownFailed(string error) { }
#else
        public void Construct(string appPrivacyPolicy)
        {
#if YANDEX_AD
            MobileAds.SetAgeRestrictedUser(true);
#endif
            if (AgreementAccepted)
            {
                bool approval = PlayerPrefs.GetInt(YandexAgreement) == HasAccepted;
                Debug.Log($"Advertisement Plugin: YANDEX pp = {approval}.");
#if YANDEX_AD
                MobileAds.SetUserConsent(approval);
#endif
            }
            else
            {
                if(_appIcon == null)
                    _appImage.gameObject.SetActive(false);
                else
                    _appImage.sprite = _appIcon;

                _appLabelText.text = Application.productName;
                _appPrivacyPolicy.Initialize("appPrivacyPolicy", appPrivacyPolicy);
                _yandexPrivacyPolicy.Initialize("yandexPrivacyPolicy", YandexPP);

                _agreeButton.onClick.AddListener(ApprovYandexConcern);

                for (int i = 0; i < _disagreeButtons.Count; i++)
                    _disagreeButtons[i].onClick.AddListener(CloseYandexConcern);

                _canvasGroup.alpha = 1;
                _canvasGroup.interactable = true;
                _canvasGroup.blocksRaycasts = true;
            }
        }
#endif

        public void Dispose()
        {
#if YANDEX_AD || UNITY_EDITOR
            if (AgreementAccepted)
                return;

            _agreeButton.onClick.RemoveListener(ApprovYandexConcern);

            for (int i = 0; i < _disagreeButtons.Count; i++)
                _disagreeButtons[i].onClick.RemoveListener(CloseYandexConcern);
#endif
        }

        public void OnConsentWindowClosed(bool hasConsent)
        {
            PlayerPrefs.SetInt(YabbyAgreement, hasConsent ? HasAccepted : NotAccepted);
            AgreementClosed = true;
            PlayerPrefs.Save();
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

        private void CloseYandexConcern()
        {
            PlayerPrefs.SetInt(YandexAgreement, NotAccepted);
#if YANDEX_AD
                MobileAds.SetUserConsent(false);
#endif
            CloseConcern();
        }

        private void ApprovYandexConcern()
        {
            PlayerPrefs.SetInt(YandexAgreement, HasAccepted);
#if YANDEX_AD
                MobileAds.SetUserConsent(true);
#endif
            CloseConcern();
        }

        private void CloseConcern()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            AgreementClosed = true;
            PlayerPrefs.Save();
        }
    }
}
