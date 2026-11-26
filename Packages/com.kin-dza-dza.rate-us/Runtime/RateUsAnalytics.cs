using System;
using UnityEngine;
using Io.AppMetrica;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace KinDzaDza.RateUs
{
    [Preserve]
    public static class RateUsAnalytics
    {
        private const string RateUsClicked = "Rate Us Clicked";
        private const string RateUsWindow = "Rate Us Window";

        public static Func<bool> HasSubscription;

        public static void SendClicked(bool agreed)
        {
            bool hasSubscription = HasSubscription != null && HasSubscription.Invoke();

            string json = JsonConvert.SerializeObject(new RateUsClickData
            {
                Result = agreed ? "Yes" : "No",
                Subscription = hasSubscription,
            });

            try
            {
                Debug.Log($"ANALYTICS: event - {RateUsClicked}, json - {json}");
                AppMetrica.ReportEvent(RateUsClicked, json);
            }
            catch (Exception exception)
            {
                Debug.LogError(exception.Message);
            }
        }

        public static void SendOpenWindow()
        {
            try
            {
                Debug.Log($"ANALYTICS: event - {RateUsWindow}");
                AppMetrica.ReportEvent(RateUsWindow);
            }
            catch (Exception exception)
            {
                Debug.LogError(exception.Message);
            }
        }
    }
}
