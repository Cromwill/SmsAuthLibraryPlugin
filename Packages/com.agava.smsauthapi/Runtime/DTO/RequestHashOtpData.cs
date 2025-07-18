using System;
using UnityEngine.Scripting;

namespace SmsAuthAPI.DTO
{
    [Serializable, Preserve]
    public class RequestHashOtpData
    {
        public string phone { get; set; }
        public string hashText { get; set; }
    }
}
