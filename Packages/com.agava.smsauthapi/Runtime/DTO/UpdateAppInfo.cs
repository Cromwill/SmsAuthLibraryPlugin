using System;
using UnityEngine.Scripting;

namespace SmsAuthAPI.DTO
{
    [Serializable, Preserve]
    public class UpdateAppInfo
    {
        public string app_id { get; set; }
        public string store_id { get; set; }
        public int update_bundle { get; set; }
        public string store_link { get; set; }
        public string platform { get; set; } = "";
    }
}
