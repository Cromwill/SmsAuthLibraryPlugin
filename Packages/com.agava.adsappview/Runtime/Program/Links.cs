using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using AdsAppView.Utility;
using System.Collections.Generic;
using KinDzaDzaGames.AdvertisementPlugin.Utility;
using Store = KinDzaDzaGames.AdvertisementPlugin.Utility.Store;

namespace AdsAppView.Program
{
    public class Links : MonoBehaviour
    {
        private const string SupportRmtKey = "smart_support_bot";
        private const string AgreementRmtKey = "agreement";
        private const string PrivacyRmtKey = "privacy";
        private const string SubscriptionRmtKey = "subscription";

        private string _appNameParameter = string.Empty;
        private string _storeParameter = string.Empty;

        private Dictionary<AppName, string> _appAuthenticators = new()
        {
            { AppName.None, "" },
            { AppName.Kubokot, "?start=kubokot" },
            { AppName.LogicLike, "" },
            { AppName.LeoAndTigTaiga, "?start=leotigforest" },
            { AppName.MishkiBigConcert, "?start=mimimiconcert" },
            { AppName.FairytalePatrolAdventure, "?start=faitypatradv" },
            { AppName.MusicalPatrol, "?start=musicpatr" },
            { AppName.Multiknowledge, "?start=multiznayka" },
            { AppName.MishkiAdventure, "?start=mimimiadv" },
            { AppName.LeoAndTig, "?start=leotig" },
            { AppName.MishkiTrueFriend, "?start=mimimifriend" },
            { AppName.FairytalePatrolCafe, "?start=faitypatrcafe" },
            { AppName.MishkiPlanetOfCreativity, "?start=mimimicreate" },
            { AppName.MishkiInSpace, "?start=mimimispace" },
            { AppName.FairytalePatrol, "?start=faitypatr" },
            { AppName.ThreeCatsAdventure, "?start=3kotaadv" },
            { AppName.ThreeCatsRacing, "?start=3kotaskate" },
            { AppName.ThreeCatsPuzzles, "?start=3kotapuzzle" },
            { AppName.Papers, "?start=pappers" },
            { AppName.FourACube, "?start=4incube" },
            { AppName.HeroesOfEnvell, "?start=envelheroes" },
        };

        private Dictionary<Store, string> _storeAuthenticators = new()
        {
            { Store.AppStore, "_apple" },
            { Store.Google, "_google" },
            { Store.Huawei, "_appgal" },
            { Store.RuStore, "_rustore" },
            { Store.test, "" },
        };

        private AdsAppAPI _api;

        public string Support { get; private set; } = "https://t.me/MTgames_support_bot";
        public string Agreement { get; private set; } = "https://mt.media/agreement/";
        public string Privacy { get; private set; } = "https://mt.media/privacy/";
        public string Subscription { get; private set; } = "https://wink.ru/services/winkkids";
        public static Links Instance { get; private set; }

        public IEnumerator Initialize(AdsAppAPI api, AppName appName, Store store)
        {
            if (Instance == null)
                Instance = this;

            _api = api;
            var waitWeb = new WaitUntil(() => Application.internetReachability == NetworkReachability.NotReachable);
            var waitInit = new WaitUntil(() => _api.Initialized);

            if (Application.internetReachability == NetworkReachability.NotReachable)
                yield return waitWeb;

            yield return waitInit;
            yield return new WaitForSecondsRealtime(1f);

            SetAppInfo(appName, store);
            SetLinks();
        }

        private void SetAppInfo(AppName appAuthenticator, Store store)
        {
            if (_appAuthenticators.TryGetValue(appAuthenticator, out string appName))
                _appNameParameter = appName;
            else
                Debug.Log($"SUPPORT BOT: couldn't collect the app name support link parameter");

            if (_storeAuthenticators.TryGetValue(store, out string storeName))
                _storeParameter = storeName;
            else
                Debug.Log($"SUPPORT BOT: couldn't collect the store support link parameter");

            Support = Support + _appNameParameter;
        }

        private async void SetLinks()
        {
            var linkSupport = await GetLink(key: SupportRmtKey);
            var linkAgreement = await GetLink(key: AgreementRmtKey);
            var linkPrivacy = await GetLink(key: PrivacyRmtKey);
            var linkSubscription = await GetLink(key: SubscriptionRmtKey);

            if (string.IsNullOrEmpty(linkSupport) == false)
                Support = linkSupport + _appNameParameter + _storeParameter;

            Debug.Log($"SUPPORT BOT: remote support link with parameters: {Support}");

            if (string.IsNullOrEmpty(linkAgreement) == false)
                Agreement = linkAgreement;

            if (string.IsNullOrEmpty(linkPrivacy) == false)
                Privacy = linkPrivacy;

            if (string.IsNullOrEmpty(linkSubscription) == false)
                Subscription = linkSubscription;
        }

        private async Task<string> GetLink(string key)
        {
            var response = await _api.GetRemoteConfig(key);

            if (response.statusCode == UnityWebRequest.Result.Success)
            {
#if UNITY_EDITOR || TEST
                Debug.Log($"#Links# Remote config '{key}': " + response.body);
#endif
                return response.body;
            }
            else
            {
                Debug.LogWarning($"#Links# Fail to recieve remote config '{key}': " + response.statusCode);
                return string.Empty;
            }
        }
    }
}
