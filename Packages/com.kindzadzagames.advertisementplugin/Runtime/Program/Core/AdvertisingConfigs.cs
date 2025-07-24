using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace KinDzaDzaGames.AdvertisementPlugin
{
    [Preserve, Serializable]
    public class AdvertisingConfigs
    {
        [field: SerializeField] public string PublisherID = "65057899-a16a-4877-989b-38c432a7fa15";
        [field: SerializeField] public string InterstitialUnitID = "b8359c60-9bde-47c9-85ff-3c7afd2bd982";
        [field: SerializeField] public string RewardedUnitID = "eaac7a7f-b0b0-46d2-ac95-bd58578e9e29";
        [field: SerializeField] public string BannerUnitID = "27668678-d138-4af4-84f4-891252086125";
        [field: SerializeField] public string AppPrivacyPolicyURL = "https://yabbi.me/policy";
    }
}
