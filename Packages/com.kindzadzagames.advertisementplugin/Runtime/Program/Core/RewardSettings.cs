using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace KinDzaDzaGames.AdvertisementPlugin
{
    [Serializable, Preserve]
    public class RewardSettings
    {
        [field: SerializeField] public string RewardCountKey = "reward-count";
        [field: SerializeField] public int DefaultRewardCount = 0;
        [field: SerializeField, Space(10)] public string RewardAvailableKey = "reward-available";
        [field: SerializeField] public bool DefaultRewardAvailable = false;

        public int RewardCount { get; private set; }
        public bool RewardAvailable { get; private set; }

        public void Contruct(int rewardCount, bool rewardAvailable)
        {
            RewardCount = rewardCount;
            RewardAvailable = rewardAvailable;

            Debug.Log($"Advertisement Plugin: remote config, reward count = {RewardCount}, reward available = {RewardAvailable}.");
        }
    }
}
