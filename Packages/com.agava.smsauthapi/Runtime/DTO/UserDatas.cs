using System;
using UnityEngine.Scripting;

namespace SmsAuthAPI.DTO
{
    [Serializable, Preserve]
    public class UserDatas
    {
        public string phone { get; set; }
        public string device_id { get; set; }
        public string appmetrica_id { get; set; }
    }
}
