using System;
using Io.AppMetrica;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Scripting;

namespace Agava.Wink
{
    [Preserve]
    public static class AnalyticsWinkService
    {
        /// <summary>
        /// Auditory from deeplinks
        /// </summary>
        public static void SendDeeplinkRedirected(string appId, string partnerId, string trackingId)
            => AppMetrica.ReportEvent("Adv company event", GetDataTrackingJson("Deeplink open", appId, partnerId, trackingId));

        /// <summary>
        /// Auditory from user events
        /// </summary>
        public static void SendStartApp(string appId) => SendEvent("App run", GetJson("App run", appId));
        public static void SendSanId(string sanId)
        {
            SendEvent("SanId", GetJson("SanId", sanId));
            Debug.LogWarning("Analytics: SanId: " + sanId);
        }

        public static void SendSex(string sex) => SendEvent($"Sex {sex}");//N/A
        public static void SendAge(string age) => SendEvent($"Age {age}");//N/A

        /// <summary>
        /// Retention
        /// </summary>
        public static void SendAverageSessionLength(int time) => SendEvent("Average Session Length", GetJson("New Account", time.ToString()));

        /// <summary>
        /// First time events
        /// </summary>
        public static void SendSubscribeOfferWindow() => SendEvent("Subscribe Offer Window (Unsigned user)");
        public static void SendHelloWindow() => SendEvent("Subscribe Profile Window", GetJson(true));
        public static void SendHelloWOAccessWindow() => SendEvent("Subscribe Profile Window", GetJson(false));
        public static void SendEnterPhoneWindow() => SendEvent("Enter Phone Window");
        public static void SendOnEnteredPhoneWindow() => SendEvent("On Entered Phone");
        public static void SendEnterOtpCodeWindow() => SendEvent("Enter Otp Code Window");
        public static void SendOnEnteredOtpCodeWindow() => SendEvent("On Entered Otp Code");
        public static void SendPayWallWindow() => SendEvent("PayWall Window");
        public static void SendPayWallRedirect() => SendEvent("PayWall Redirect");
        public static void SendFirstOpen() => SendEvent("First Open Game");
        public static void SendSupportLink() => SendEvent("Support Link");
        public static void SendSubscriptionLink() => SendEvent("About Subscription Link");
        public static void SendDeleteWindow() => SendEvent("Delete Window");
        public static void SendCloseStartWindow() => SendEvent("Close Start Window");
        public static void SendHaveWinkButtonClick() => SendEvent("Click Have Wink Button");
        public static void SendOfferWinkKidsButtonClick() => SendEvent("Clicked Offer Wink Kids");
        public static void SendSubscribeWinkButtonClick() => SendEvent("Subscribe Wink");
        public static void SendDeleteAccountButtonClick() => SendEvent("Delete Account Button");
        public static void SendShowOfferWinkKidsWindow() => SendEvent("Show Offer Wink Kids");
        public static void SendShowRedirectWindow() => SendEvent("Show Redirect Screen");
        public static void SendChangeOrientationWindow() => SendEvent("Change Orientation Window");
        public static void SendPlayerRotateDevice() => SendEvent("Player Rotate Device");
        public static void SendAccountDeletionWindow() => SendEvent("Account Deletion Window");
        public static void SendSubscriptionManagementWindow() => SendEvent("Subscription Management");
        public static void SendSubscriptionManagementButtonClick() => SendEvent("Click Subscription Management Button");
        public static void SendSubscribeButtonClickOnSettings() => SendEvent("Subscribe Button On Settings");
        public static void SendDeleteAccountButtonClickOnSetting() => SendEvent("Delete Account Button On Settings");
        public static void SendSupportButtonClickOnSetting() => SendEvent("Support Button On Settings");
        public static void SendSubscriptionPurchaseWasSuccessful() => SendEvent("Subscription Purchase Was Successful");
        public static void SendCancelSubscriptionPurchase() => SendEvent("Cancel Subscription Purchase");

        private static string GetJson(string name, string value)
        {
            Data data = new Data()
            {
                Name = name,
                Value = value
            };

            return JsonConvert.SerializeObject(data);
        }

        private static string GetJson(bool value)
        {
            SubscribeData data = new SubscribeData()
            {
                Subscribe = value
            };

            return JsonConvert.SerializeObject(data);
        }

        private static string GetDataTrackingJson(string name, string app, string partnerId, string trackingId)
        {
            DataTracking data = new()
            {
                event_name = name,
                app_id = app,
                from = trackingId,
                partner = partnerId,
            };

            return JsonConvert.SerializeObject(data);
        }

        internal class Data
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        internal class SubscribeData
        {
            public bool Subscribe { get; set; }
        }

        internal class DataTracking
        {
            public string event_name { get; set; }
            public string app_id { get; set; }
            public string from { get; set; }
            public string partner { get; set; }
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
                AppMetrica.ReportEvent(eventName, json);
            }
            catch (Exception ex)
            {
                Debug.Log("AppMetrica error:");
                Debug.Log(ex.Message);
            }
        }
    }
}
