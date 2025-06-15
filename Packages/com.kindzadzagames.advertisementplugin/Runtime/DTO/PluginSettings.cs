using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace KinDzaDzaGames.AdvertisementPlugin.DTO
{
    [Preserve, Serializable]
    public class PluginSettings : MonoBehaviour
    {
        public string app_name;
        public string platform;
        public int released_version;
        public int review_version;
        public string plugin_state;
        public string test_review;
        public string common;
    }
}
