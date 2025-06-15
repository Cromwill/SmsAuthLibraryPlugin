using System;
using UnityEngine.Scripting;

namespace KinDzaDzaGames.AdvertisementPlugin.DTO
{
    [Preserve, Serializable]
    public class AppData
    {
        public string app_id { get; set; }
        public string store_id { get; set; }
        public string platform { get; set; }
    }
}
