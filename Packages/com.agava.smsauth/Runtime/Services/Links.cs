using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdsAppView.Utility;
using Agava.Wink;
using SmsAuthAPI.Program;
using UnityEngine;
using UnityEngine.Networking;

public class Links : MonoBehaviour
{
    private string _appNameParameter = string.Empty;
    private string _storeParameter = string.Empty;
    private bool _supportParamsSetted = false;

    public const string SupportRmtKey = "smart_support_bot";
    public const string AgreementRmtKey = "agreement";
    public const string PrivacyRmtKey = "privacy";
    public const string SubscriptionRmtKey = "subscription";

    private Dictionary<AppAuthenticator, string> _appAuthenticators = new()
        {
            { AppAuthenticator.None, "" },
            { AppAuthenticator.Kubokot, "?start=kubokot" },
            { AppAuthenticator.LogicLike, "" },
            { AppAuthenticator.LeoAndTigTaiga, "?start=leotigforest" },
            { AppAuthenticator.MishkiBigConcert, "?start=mimimiconcert" },
            { AppAuthenticator.FairytalePatrolAdventure, "?start=faitypatradv" },
            { AppAuthenticator.MusicalPatrol, "?start=musicpatr" },
            { AppAuthenticator.Multiknowledge, "?start=multiznayka" },
            { AppAuthenticator.MishkiAdventure, "?start=mimimiadv" },
            { AppAuthenticator.LeoAndTig, "?start=leotig" },
            { AppAuthenticator.MishkiTrueFriend, "?start=mimimifriend" },
            { AppAuthenticator.FairytalePatrolCafe, "?start=faitypatrcafe" },
            { AppAuthenticator.MishkiPlanetOfCreativity, "?start=mimimicreate" },
            { AppAuthenticator.MishkiInSpace, "?start=mimimispace" },
            { AppAuthenticator.FairytalePatrol, "?start=faitypatr" },
            { AppAuthenticator.ThreeCatsAdventure, "?start=3kotaadv" },
            { AppAuthenticator.ThreeCatsRacing, "?start=3kotaskate" },
            { AppAuthenticator.ThreeCatsPuzzles, "?start=3kotapuzzle" },
            { AppAuthenticator.Pappers, "?start=pappers" },
            { AppAuthenticator.FourInCube, "?start=4incube" },
            { AppAuthenticator.EnvelHeroes, "?start=envelheroes" },
        };

    private Dictionary<Store, string> _storeAuthenticators = new()
        {
            { Store.AppStore, "_apple" },
            { Store.Google, "_google" },
            { Store.Huawei, "_appgal" },
            { Store.RuStore, "_rustore" },
            { Store.test, "" },
        };

    public string Support { get; private set; } = "https://t.me/MTgames_support_bot";
    public string Agreement { get; private set; } = "https://mt.media/agreement/";
    public string Privacy { get; private set; } = "https://mt.media/privacy/";
    public string Subscription { get; private set; } = "https://wink.ru/services/winkkids";
    public bool Initialized { get; private set; } = false;

    public static Links Instance { get; private set; }

    public void Construct()
    {
        if (Instance == null)
            Instance = this;
    }

    private IEnumerator Start()
    {
        var waitWeb = new WaitUntil(() => Application.internetReachability == NetworkReachability.NotReachable);
        var waitInit = new WaitUntil(() => SmsAuthApi.Initialized);
        var waitSupport = new WaitUntil(() => _supportParamsSetted);

        if (Application.internetReachability == NetworkReachability.NotReachable)
            yield return waitWeb;

        yield return waitInit;
        yield return new WaitForSecondsRealtime(1f);
        yield return waitSupport;

        SetLinks();
    }

    public void SetAppInfo(BuildVersionHolder buildVersionHolder, AppAuthenticator appAuthenticator)
    {
        _supportParamsSetted = false;

        if (_appAuthenticators.TryGetValue(appAuthenticator, out string appName))
            _appNameParameter = appName;
        else
            Debug.Log($"SUPPORT BOT: couldn't collect the app name support link parameter");

        if(_storeAuthenticators.TryGetValue(buildVersionHolder.StoreName, out string storeName))
            _storeParameter = storeName;
        else
            Debug.Log($"SUPPORT BOT: couldn't collect the store support link parameter");

        Support = Support + _appNameParameter;
        _supportParamsSetted = true;
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

        Initialized = true;
    }

    private async Task<string> GetLink(string key)
    {
        var response = await SmsAuthApi.GetRemoteConfig(key);

        if (response.statusCode == UnityWebRequest.Result.Success)
        {
#if UNITY_EDITOR || TEST
            Debug.Log($"Remote config '{key}': " + response.body);
#endif
            return response.body;
        }
        else
        {
            Debug.LogWarning($"Fail to recieve remote config '{key}': " + response.statusCode);
            return string.Empty;
        }
    }
}
