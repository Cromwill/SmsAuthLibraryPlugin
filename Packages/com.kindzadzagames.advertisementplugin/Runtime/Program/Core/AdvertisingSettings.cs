namespace KinDzaDzaGames.AdvertisementPlugin
{
    public static class AdvertisingSettings
    {
        public class EditorTest
        {
            public const string Test = "9999";
        }

        public class YabbiAds
        {
            public const string publisherID = "65057899-a16a-4877-989b-38c432a7fa15";
            public const string yabbiInterstitialUnitID = "b8359c60-9bde-47c9-85ff-3c7afd2bd982";
            public const string yabbiRewardedUnitID = "eaac7a7f-b0b0-46d2-ac95-bd58578e9e29";
            public const string yabbiBannerUnitID = "27668678-d138-4af4-84f4-891252086125";
            public const string yandexInterstitialUnitID = "cd1dff91-76d0-44c2-a6ca-fd3f446ef9b5";
            public const string yandexRewardedlUnitID = "ded961d2-9790-11ee-9c72-89e4aa1699d3";
            public const string ironsourceInterstitialUnitID = "ec9decde-58c8-4d1b-885f-479b05f39dcb";
            public const string ironsourceRewardedlUnitID = "9b45ac1a-ca72-4d57-9f0f-7bab924ad1b4";
            public const string mintegralInterstitialUnitID = "";
            public const string mintegralRewardedlUnitID = "";
            public const string applovinInterstitialUnitID = "c5922651-61e7-471c-9562-c1b3ec7ec754";
            public const string applovinRewardedlUnitID = "66684cac-546e-465a-8795-0ac6097bc088";
            public const string appnextInterstitialUnitID = "";
            public const string appnextRewardedlUnitID = "";
            public const string PrivacyPolicyURL = "https://yabbi.me/privacy-policies";
        }

        public class YandexAds
        {
            public class Demo
            {
                public static string InterstitialUnitId { get; private set; } = "demo-interstitial-yandex";
                public static string RewardUnitId { get; private set; } = "demo-rewarded-yandex";
                public static string BannerUnitId { get; private set; } = "demo-banner-yandex";
            }

            public class Release
            {
                public static string InterstitialUnitId { get; private set; } = "R-M-15576881-2";
                public static string RewardUnitId { get; private set; } = "R-M-15576881-3";
                public static string BannerUnitId { get; private set; } = "R-M-15576881-1";
            }
        }
    }
}
