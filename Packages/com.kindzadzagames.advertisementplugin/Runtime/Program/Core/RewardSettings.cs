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
        [field: SerializeField, Space(10)] public string WinkPriceKey = "wink-price-text";
        [field: SerializeField] public string DefaultWinkPrice = "ƒалее 199 р/мес€ц";
        [field: SerializeField, Space(10)] public string TrialPeriodDaysKey = "trial-period-days-text";
        [field: SerializeField] public string DefaultTrialPeriodDays = "30 дней за 0 руб";

        public int RewardCount { get; private set; }
        public bool RewardAvailable { get; private set; }
        public string WinkPrice { get; private set; }
        public string TrialPeriodDays { get; private set; }

        public void Contruct(int rewardCount, bool rewardAvailable, string winkPrice, string trialPeriodDays)
        {
            RewardCount = rewardCount;
            RewardAvailable = rewardAvailable;
            WinkPrice = winkPrice;
            TrialPeriodDays = trialPeriodDays;

            Debug.Log($"Reward Remote: reward count = {RewardCount}, reward available = {RewardAvailable},\n wink price = {WinkPrice}, trial periodDays = {TrialPeriodDays}");
        }
    }
}
