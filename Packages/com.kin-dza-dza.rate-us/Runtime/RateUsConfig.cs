using System;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace KinDzaDza.RateUs
{
    [Preserve, Serializable]
    public class RateUsConfig
    {
        [JsonProperty("app_id")] public string AppId;
        [JsonProperty("store_id")] public string StoreId;
        [JsonProperty("trigger_time_sec")] public int TriggerTimeSeconds;
        [JsonProperty("store_link")] public string StoreLink;
    }
}
