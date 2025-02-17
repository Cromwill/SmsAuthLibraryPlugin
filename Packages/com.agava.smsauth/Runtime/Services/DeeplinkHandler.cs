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
            AnalyticsWinkService.SendDeeplinkRedirected(Application.identifier);
            Debug.LogWarning($"Deeplink detected: {url}");
            Debug.LogWarning($"Absolute URL: {Application.absoluteURL}");
        }
    }
}
