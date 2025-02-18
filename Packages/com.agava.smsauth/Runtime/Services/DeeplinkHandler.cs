using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Scripting;
using SmsAuthAPI.DTO;
using SmsAuthAPI.Program;

namespace Agava.Wink
{
    [Preserve]
    public class DeeplinkHandler
    {
        private const string RemoteName = "adv-comp-name";

        public void Init()
        {
            Application.deepLinkActivated += OnDeepLinkActivated;

            if (!string.IsNullOrEmpty(Application.absoluteURL))
                OnDeepLinkActivated(Application.absoluteURL);
        }

        private async void OnDeepLinkActivated(string url)
        {
            string pattern = @"appmetrica_tracking_id=(\d+)";
            Match matchTracking = Regex.Match(url, pattern);
            string trackingId;

            if (matchTracking.Success)
                trackingId = matchTracking.Groups[1].Value;
            else
                trackingId = "no id";

            string partner = @"campaign=(\d+)";
            Match matchPartner = Regex.Match(url, partner);
            string partnerId;

            if (matchPartner.Success)
            {
                string name = await GetAdvName();
                partnerId = matchPartner.Groups[1].Value;

                if (string.IsNullOrEmpty(name) == false && name == partnerId)
                {
                    Application.deepLinkActivated -= OnDeepLinkActivated;
                    AnalyticsWinkService.SendDeeplinkRedirected(Application.identifier, partnerId, trackingId);
                    Debug.LogWarning($"Deeplink detected '{url}' (campaing: {partnerId}), analytics send:, with campaing {name}");
                }
            }
            else
            {
                partnerId = "no partner name";
            }

            string onbordingType = @"content=(\d+)";
            Match matchOnbording = Regex.Match(url, onbordingType);
            string onbordingId;

            if (matchOnbording.Success)
                onbordingId = matchOnbording.Groups[1].Value;
            else
                onbordingId = "default";

            Debug.LogWarning($"appmetrica_tracking_id={trackingId}");
            Debug.LogWarning($"campaign={partnerId}");
            Debug.LogWarning($"content={onbordingId}");
            Debug.LogWarning($"Deeplink detected: {url}");
        }

        private async Task<string> GetAdvName()
        {
            Response response = await SmsAuthApi.GetRemoteConfig(RemoteName);

            if (response.statusCode == UnityWebRequest.Result.Success)
            {
                if (string.IsNullOrEmpty(response.body) == false)
                {
                    return response.body;
                }
                else
                {
                    Debug.LogError($"#{GetType()}# Fail to recieve remote config '{RemoteName}': value is NULL");
                    return null;
                }
            }
            else
            {
                Debug.LogError($"#{GetType()}# Get remote fail");
                return null;
            }
        }
    }
}
