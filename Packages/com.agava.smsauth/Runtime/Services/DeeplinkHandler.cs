using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Scripting;

namespace Agava.Wink
{
    [Preserve]
    public static class DeeplinkHandler
    {
        public static void Init()
        {
            Application.deepLinkActivated += OnDeepLinkActivated;

            if (!string.IsNullOrEmpty(Application.absoluteURL))
                OnDeepLinkActivated(Application.absoluteURL);
        }

        private static void OnDeepLinkActivated(string url)
        {
            Application.deepLinkActivated -= OnDeepLinkActivated;

            string pattern = @"appmetrica_tracking_id=(\d+)";
            Match match = Regex.Match(url, pattern);
            string trackingId;

            if (match.Success)
                trackingId = match.Groups[1].Value;
            else
                trackingId = "no id";

            AnalyticsWinkService.SendDeeplinkRedirected(Application.identifier, trackingId);
            Debug.LogWarning($"Deeplink detected: {url}");
            Debug.LogWarning($"Absolute URL: {Application.absoluteURL}");
        }
    }
}
