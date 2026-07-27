using System;
using UnityEngine.Scripting;

namespace SmsAuthAPI.DTO
{
    [Serializable, Preserve]
    public class BackendAnalyticsData
    {
        public string event_name { get; set; }
        public string phone { get; set; }
        public string device_id { get; set; }
        public string san { get; set; }
        public DateTime event_time { get; set; }
        public string platform { get; set; }
        public string version { get; set; }
        public string appmetrica_device_id { get; set; }
        public string event_json { get; set; }
    }
}
