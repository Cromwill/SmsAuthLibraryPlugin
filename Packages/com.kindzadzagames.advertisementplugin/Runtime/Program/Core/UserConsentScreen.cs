using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
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

#if UNITY_EDITOR
        [ReadOnly]
#endif
        [SerializeField] private Button _nextButton;

        public bool AgreementClosed { get; private set; } = false;

#if YABBI_AD
        public bool AgreementAccepted => PlayerPrefs.HasKey(YabbyAgreement);
#elif UNITY_IOS && YANDEX_AD
        public bool AgreementAccepted => ATTrackingStatusBinding.GetAuthorizationTrackingStatus() != ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED;
#else
        public bool AgreementAccepted => PlayerPrefs.HasKey(YandexAgreement);
#endif

        private ICoroutine _coroutine;

#if YABBI_AD
        public void Construct(ConsentManager consentManager)
        {
            _consentManager = consentManager ?? throw new ArgumentNullException(nameof(consentManager));
        }

        public void OnConsentManagerLoaded() => _consentManager.ShowConsentWindow();
        public void OnConsentManagerLoadFailed(string error) { }
        public void OnConsentManagerShownFailed(string error) { }
#else
        public void Construct(string appPrivacyPolicy, ICoroutine coroutine)
        {
            _coroutine = coroutine ?? throw new ArgumentNullException(nameof(coroutine));

#if YANDEX_AD
            MobileAds.SetAgeRestrictedUser(true);
#endif
            if (AgreementAccepted)
            {
#if UNITY_IOS && YANDEX_AD
                bool approval = DetermineTrackingStatus();
#else
                bool approval = PlayerPrefs.GetInt(YandexAgreement) == HasAccepted;
#endif

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

#if UNITY_IOS && YANDEX_AD
                _agreeButton.gameObject.SetActive(false);

                for (int i = 0; i < _disagreeButtons.Count; i++)
                    _disagreeButtons[i].gameObject.SetActive(false);

                _nextButton.gameObject.SetActive(true);
                _nextButton.onClick.AddListener(OpenIosConsentScreen);
#else
                _nextButton.gameObject.SetActive(false);
                _agreeButton.onClick.AddListener(ApprovYandexConcern);

                for (int i = 0; i < _disagreeButtons.Count; i++)
                    _disagreeButtons[i].onClick.AddListener(CloseYandexConcern);
#endif

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

#if UNITY_IOS && YANDEX_AD
            _nextButton.onClick.RemoveListener(OpenIosConsentScreen);
#else
            _agreeButton.onClick.RemoveListener(ApprovYandexConcern);

            for (int i = 0; i < _disagreeButtons.Count; i++)
                _disagreeButtons[i].onClick.RemoveListener(CloseYandexConcern);
#endif
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
#if UNITY_IOS && YANDEX_AD
        private void OpenIosConsentScreen()
        {
            ATTrackingStatusBinding.RequestAuthorizationTracking();

            _coroutine.StartCoroutine(WaitCloseTracking());

            IEnumerator WaitCloseTracking()
            {
                while (ATTrackingStatusBinding.GetAuthorizationTrackingStatus() == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
                {
                    yield return null;
                }

                bool approval = DetermineTrackingStatus();
                MobileAds.SetUserConsent(approval);
                CloseConcern();
            }
        }

        private bool DetermineTrackingStatus() => ATTrackingStatusBinding.GetAuthorizationTrackingStatus() == ATTrackingStatusBinding.AuthorizationTrackingStatus.RESTRICTED;
#endif
    }
}
