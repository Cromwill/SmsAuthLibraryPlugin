using UnityEngine;
using UnityEngine.Scripting;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace KinDzaDzaGames.AdvertisementPlugin.Utility
{
    [Preserve]
    public static class RemoteConfig
    {
        public static async Task<int> IntRemoteConfig(string configName, int defaultValue)
        {
            var response = await AdvertisementAPI.Instance.GetRemoteConfig(configName);

            if (response.statusCode == UnityWebRequest.Result.Success)
            {
                if (string.IsNullOrEmpty(response.body) == false)
                    return ParseIntConfig(response.body, defaultValue);
                else
                    Debug.LogError($"Fail to recieve remote config '{configName}': value is NULL");
            }
            else
            {
                Debug.LogError($"Fail to recieve remote config '{configName}': BAD REQUEST");
            }

            return defaultValue;
        }

        public static async Task<bool> BoolRemoteConfig(string configName, bool defaultValue)
        {
            var response = await AdvertisementAPI.Instance.GetRemoteConfig(configName);

            if (response.statusCode == UnityWebRequest.Result.Success)
            {
                if (string.IsNullOrEmpty(response.body) == false)
                    return ParseBoolConfig(response.body, defaultValue);
                else
                    Debug.LogError($"Fail to recieve remote config '{configName}': value is NULL");
            }
            else
            {
                Debug.LogError($"Fail to recieve remote config '{configName}': BAD REQUEST");
            }

            return defaultValue;
        }

        private static int ParseIntConfig(string countStr, int defaultValue)
        {
            bool success = int.TryParse(countStr, out int count);

            return success ? count : defaultValue;
        }
        
        private static bool ParseBoolConfig(string boolStr, bool defaultValue)
        {
            bool success = bool.TryParse(boolStr, out bool available);

            return success ? available : defaultValue;
        }

        public static async Task<string> StringRemoteConfig(string configName, string defaultValue)
        {
            var response = await AdvertisementAPI.Instance.GetRemoteConfig(configName);

            if (response.statusCode == UnityWebRequest.Result.Success)
            {
                if (string.IsNullOrEmpty(response.body) == false)
                    return response.body;
                else
                    Debug.LogError($"Fail to recieve remote config '{configName}': value is NULL");
            }
            else
            {
                Debug.LogError($"Fail to recieve remote config '{configName}': BAD REQUEST");
            }

            return defaultValue;
        }
    }
}
