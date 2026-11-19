using System;
using Io.AppMetrica;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Scripting;

namespace KinDzaDza.RateUs
{
    [Preserve]
    public static class RateUsAnalytics
    {
        private const string EventName = "Rate Us Clicked";

        public static void SendClicked(bool agreed, bool subscription)
        {
            string json = JsonConvert.SerializeObject(new RateUsClickData
            {
                Result = agreed ? "Yes" : "No",
                Subscription = subscription,
            });

            try
            {
                Debug.Log($"ANALYTICS: event - {EventName}, json - {json}");
                AppMetrica.ReportEvent(EventName, json);
            }
            catch (Exception exception)
            {
                Debug.LogError(exception.Message);
            }
        }
    }
}
