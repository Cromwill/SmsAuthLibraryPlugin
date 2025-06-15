using System;
using UnityEngine.Scripting;

namespace KinDzaDzaGames.AdvertisementPlugin.DTO
{
    [Preserve, Serializable]
    public class AdsSdkSettingsData
    {
        public string app_id { get; set; }
        public string store_id { get; set; }
        public string platform { get; set; }
        public int released_version { get; set; }
        public bool released_state { get; set; }
        public int review_version { get; set; }
        public bool review_state { get; set; }
        public bool vip_state { get; set; }
        public int first_timer { get; set; }
        public int regular_timer { get; set; }
        public string ads_lk_id { get; set; }
        public string common { get; set; }
    }
}
