using System;
using Io.AppMetrica;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Scripting;

namespace KinDzaDzaGames.AdvertisementPlugin
{
    [Preserve]
    public static class AdvertisementAnalyticsService
    {
        public static void SendAdsLoadSuccess(AdsType advertismentType) => SendEvent($"Ad Loaded", GetAdsDataJson(advertismentType));
        public static void SendAdsLoadFailed(AdsType advertismentType) => SendEvent($"Ad Failed", GetActionAdsDataJson(advertismentType, Action.Load));
        public static void SendAdsShowRequested(AdsType advertismentType) => SendEvent($"Ad Show Requested", GetAdsDataJson(advertismentType));
        public static void SendAdsShowSuccess(AdsType advertismentType) => SendEvent($"Ad Shown", GetAdsDataJson(advertismentType));
        public static void SendAdsShowFailed(AdsType advertismentType) => SendEvent($"Ad Failed", GetActionAdsDataJson(advertismentType, Action.Show));
        public static void SendAdsClosed(AdsType advertismentType) => SendEvent($"Ad Closed", GetCloseAdsDataJson(advertismentType, ClosingCause.Closed));
        public static void SendAdsVideoCompleted(AdsType advertismentType) => SendEvent($"Ad Closed", GetCloseAdsDataJson(advertismentType, ClosingCause.Completed));
        public static void SendUserRewarded(AdsType advertismentType) => SendEvent($"Ad Closed", GetCloseAdsDataJson(advertismentType, ClosingCause.UserRewarded));

        private static string GetAdsDataJson(AdsType adsType)
        {
            AdsData data = new()
            {
                AdsType = adsType.ToString(),
            };

            return JsonConvert.SerializeObject(data);
        }

        private static string GetActionAdsDataJson(AdsType adsType, Action action)
        {
            ActionAdsData data = new()
            {
                AdsType = adsType.ToString(),
                Action = action.ToString(),
            };

            return JsonConvert.SerializeObject(data);
        }

        private static string GetCloseAdsDataJson(AdsType adsType, ClosingCause cause)
        {
            CloseAdsData data = new()
            {
                AdsType = adsType.ToString(),
                Cause = cause.ToString(),
            };

            return JsonConvert.SerializeObject(data);
        }

        [Serializable]
        internal class AdsData
        {
            public string AdsType;
        }

        [Serializable]
        internal class ActionAdsData : AdsData
        {
            public string Action;
        }

        [Serializable]
        internal class CloseAdsData : AdsData
        {
            public string Cause;
        }

        private static void SendEvent(string eventName)
        {
            Debug.Log($"ANALYTICS: event - {eventName}");
            AppMetrica.ReportEvent(eventName);
        }

        private static void SendEvent(string eventName, string json)
        {
            try
            {
                Debug.Log($"ANALYTICS: event - {eventName}, params: {json}");
                AppMetrica.ReportEvent(eventName, json);
            }
            catch (Exception ex)
            {
                Debug.Log("AppMetrica error:");
                Debug.Log(ex.Message);
            }
        }

        public enum ClosingCause
        {
            UserRewarded,
            Closed,
            Completed
        }

        public enum Action
        {
            Show,
            Load
        }

        public enum AdsType
        {
            Interstitial,
            Banner,
            Reward
        }
    }
}
