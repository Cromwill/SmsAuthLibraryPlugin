using System;
using UnityEngine;
using UnityEngine.Scripting;
using System.Collections.Generic;

namespace Agava.Wink
{
    [Serializable, Preserve]
    public class WinkWebViewURLHandler
    {
        private const string Kubokot = "https://wink.ru/embed/buy?type=service&id=winkkids&auth_phone={AUTH_PHONE}&auth_app=Kubokot";
        private const string LogicLike = "https://wink.ru/embed/buy?type=service&id=winkkids&auth_phone={AUTH_PHONE}&auth_app=LogicLike";
        private const string LeoAndTigTaiga = "https://wink.ru/embed/buy?type=service&id=winkkids&auth_phone={AUTH_PHONE}&auth_app=LeoAndTigTaiga";
        private const string MishkiBigConcert = "https://wink.ru/embed/buy?type=service&id=winkkids&auth_phone={AUTH_PHONE}&auth_app=MishkiBigConcert";
        private const string FairytalePatrolAdventure = "https://wink.ru/embed/buy?type=service&id=winkkids&auth_phone={AUTH_PHONE}&auth_app=FairytalePatrolAdventure";
        private const string MusicalPatrol = "https://wink.ru/embed/buy?type=service&id=winkkids&auth_phone={AUTH_PHONE}&auth_app=MusicalPatrol";
        private const string Multiknowledge = "https://wink.ru/embed/buy?type=service&id=winkkids&auth_phone={AUTH_PHONE}&auth_app=Multiknowledge";
        private const string MishkiAdventure = "https://wink.ru/embed/buy?type=service&id=winkkids&auth_phone={AUTH_PHONE}&auth_app=MishkiAdventure";
        private const string LeoAndTig = "https://wink.ru/embed/buy?type=service&id=winkkids&auth_phone={AUTH_PHONE}&auth_app=LeoAndTig";
        private const string MishkiTrueFriend = "https://wink.ru/embed/buy?type=service&id=winkkids&auth_phone={AUTH_PHONE}&auth_app=MishkiTrueFriend";
        private const string FairytalePatrolCafe = "https://wink.ru/embed/buy?type=service&id=winkkids&auth_phone={AUTH_PHONE}&auth_app=FairytalePatrolCafe";
        private const string MishkiPlanetOfCreativity = "https://wink.ru/embed/buy?type=service&id=winkkids&auth_phone={AUTH_PHONE}&auth_app=MishkiPlanetOfCreativity";
        private const string MishkiInSpace = "https://wink.ru/embed/buy?type=service&id=winkkids&auth_phone={AUTH_PHONE}&auth_app=MishkiInSpace";
        private const string FairytalePatrol = "https://wink.ru/embed/buy?type=service&id=winkkids&auth_phone={AUTH_PHONE}&auth_app=FairytalePatrol";
        private const string PlayerPhonePattern = "AUTH_PHONE";

        [SerializeField] private AppAuthenticator _appAuthenticator;

        private string _phoneNumber = string.Empty;
        private Dictionary<AppAuthenticator, string> _webViewURLs = new Dictionary<AppAuthenticator, string>()
        {
            { AppAuthenticator.None, string.Empty},
            { AppAuthenticator.Kubokot, Kubokot},
            { AppAuthenticator.LogicLike, LogicLike},
            { AppAuthenticator.LeoAndTigTaiga, LeoAndTigTaiga},
            { AppAuthenticator.MishkiBigConcert, MishkiBigConcert},
            { AppAuthenticator.FairytalePatrolAdventure, FairytalePatrolAdventure},
            { AppAuthenticator.MusicalPatrol, MusicalPatrol},
            { AppAuthenticator.Multiknowledge, Multiknowledge},
            { AppAuthenticator.MishkiAdventure, MishkiAdventure},
            { AppAuthenticator.LeoAndTig, LeoAndTig},
            { AppAuthenticator.MishkiTrueFriend, MishkiTrueFriend},
            { AppAuthenticator.FairytalePatrolCafe, FairytalePatrolCafe},
            { AppAuthenticator.MishkiPlanetOfCreativity, MishkiPlanetOfCreativity},
            { AppAuthenticator.MishkiInSpace, MishkiInSpace},
            { AppAuthenticator.FairytalePatrol, FairytalePatrol},
        };

        public void SetPhone(string phoneNumber) => _phoneNumber = phoneNumber;

        public string CheckAvailabilityURL()
        {
            _webViewURLs.TryGetValue(_appAuthenticator, out string url);

            return url;
        }

        public string GetURL()
        {
            string url = _webViewURLs[_appAuthenticator].Replace($"{{{PlayerPhonePattern}}}", _phoneNumber);

            return url;
        }
    }
}
