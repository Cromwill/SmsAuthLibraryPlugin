using System;
using System.Collections.Generic;

namespace KinDzaDzaGames.AdvertisementPlugin.Utility
{
    public static class AdvertisementID
    {
        private static string _appPrivacyPolicyURL = "https://mt.media/privacy/";
        private static string _yabbiTestPublisherID = "65057899-a16a-4877-989b-38c432a7fa15";
        private static string _publisherID = "09146b04-16d4-11f0-beaa-076395a5c120";
        private static string _yandexPublisherID = "not used";

        public static AdvertisingConfigs GetConfig(AppName appName, Store store, AdvertisingProvider advertisingProvider)
        {
            if(GetStoreADSList(store, advertisingProvider).TryGetValue(appName, out AdvertisingConfigs advertisingConfigs))
            {
                return advertisingConfigs;
            }
            else
            {
                throw new ArgumentException("The application data is not included in the required dictionary.");
            }
        }

        private static Dictionary<AppName, AdvertisingConfigs> GetStoreADSList(Store store, AdvertisingProvider advertisingProvider)
        {
            switch (store)
            {
                case Store.test:
                    return advertisingProvider == AdvertisingProvider.YabbiAdvertisement ? _testAdvertisementID : _testYandexAdvertisementID;
                case Store.Google:
                    return advertisingProvider == AdvertisingProvider.YabbiAdvertisement ? _googleAdvertisementID : _googleYandexAdvertisementID;
                case Store.AppStore:
                    return advertisingProvider == AdvertisingProvider.YabbiAdvertisement ? _appStoreAdvertisementID : _appStoreYandexAdvertisementID;
                case Store.RuStore:
                    return advertisingProvider == AdvertisingProvider.YabbiAdvertisement ? _ruStoreAdvertisementID : _ruStoreYandexAdvertisementID;
                case Store.Huawei:
                    return advertisingProvider == AdvertisingProvider.YabbiAdvertisement ? _huaweiStoreAdvertisementID : _huaweiStoreYandexAdvertisementID;
                default:
                    throw new ArgumentException("An unregistered store has been selected.");
            }
        }

#region Yabbi AD IDs
        private static Dictionary<AppName, AdvertisingConfigs> _testAdvertisementID = new Dictionary<AppName, AdvertisingConfigs>()
        {
            {
                AppName.TestYabbiAD, new AdvertisingConfigs()
                {
                    PublisherID = _yabbiTestPublisherID,
                    BannerUnitID = "27668678-d138-4af4-84f4-891252086125",
                    InterstitialUnitID = "b8359c60-9bde-47c9-85ff-3c7afd2bd982",
                    RewardedUnitID = "eaac7a7f-b0b0-46d2-ac95-bd58578e9e29",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            }
        };
#endregion

#region GooglePlay IDs
        private static Dictionary<AppName, AdvertisingConfigs> _googleAdvertisementID = new Dictionary<AppName, AdvertisingConfigs>()
        {
            {AppName.LeoAndTig, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "b5d3b6b2-4da0-11f0-8208-757ad425fb4a",
                    InterstitialUnitID = "bed4b630-4da0-11f0-af35-0d53a6e6e8ec",
                    RewardedUnitID = "c87b5f90-4da0-11f0-9344-c7c51d823f31",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MishkiAdventure, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "62952bd8-4da1-11f0-8b80-635d71024f68",
                    InterstitialUnitID = "6b0c5426-4da1-11f0-95f8-ffd8f86f0ca0",
                    RewardedUnitID = "7244fe28-4da1-11f0-a55d-b3e69cbfa03f",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MishkiInSpace, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "894f155e-4da1-11f0-866d-21e0d89800a7",
                    InterstitialUnitID = "9287f17c-4da1-11f0-ba0b-bd1d5545b085",
                    RewardedUnitID = "97dbf524-4da1-11f0-8560-f79f0f4432c9",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MishkiTrueFriend, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "67883ee4-4c5e-11f0-bb36-3bcfe96e5e90",
                    InterstitialUnitID = "77c28760-4c5e-11f0-8d4a-b5c69bc5015e",
                    RewardedUnitID = "811a8e84-4c5e-11f0-8f26-53db3e4d26ba",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.FairytalePatrolCafe, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "b8d05fe0-4da1-11f0-ad56-6f8d155dceaf",
                    InterstitialUnitID = "ecf0fb9a-4da1-11f0-a7a5-bf6c2abfa56a",
                    RewardedUnitID = "1ee30f94-4da2-11f0-8b17-3d66b5d3f3a0",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.LeoAndTigTaiga, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "be2b5ef4-4da1-11f0-981f-8d004fdf1d3b",
                    InterstitialUnitID = "f0dd0af0-4da1-11f0-8a0b-df38a3c0c134",
                    RewardedUnitID = "245276f4-4da2-11f0-a65c-e947496452d2",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MishkiPlanetOfCreativity, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "c2b58166-4da1-11f0-92d6-3fadd971504c",
                    InterstitialUnitID = "f4c20940-4da1-11f0-877a-17f10e7bb61e",
                    RewardedUnitID = "27b61058-4da2-11f0-999d-4501340e6c63",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MishkiBigConcert, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "c6ec3c8e-4da1-11f0-ada5-3732a52ff2e0",
                    InterstitialUnitID = "f931cc68-4da1-11f0-9740-ff179c97ec5c",
                    RewardedUnitID = "3158371c-4da2-11f0-83cb-e5bc3d8cf8e4",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.FairytalePatrol, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "cbb230b6-4da1-11f0-989f-c99ff866e043",
                    InterstitialUnitID = "fcfe85ca-4da1-11f0-ab43-5f57d7411e8b",
                    RewardedUnitID = "36e936b8-4da2-11f0-ab6e-f90161a8c1df",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.FairytalePatrolAdventure, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "d00f682c-4da1-11f0-8f9b-43bc964b70cd",
                    InterstitialUnitID = "0180478c-4da2-11f0-8b16-0543dc5060cf",
                    RewardedUnitID = "3a93938a-4da2-11f0-a56c-11978f9c6563",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MusicalPatrol, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "d3ef6384-4da1-11f0-886f-51c9768f61d5",
                    InterstitialUnitID = "05974a96-4da2-11f0-88e2-9505c08b56af",
                    RewardedUnitID = "4ac85a2e-4da2-11f0-af42-03ae5aede5f6",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.Multiknowledge, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "d7ac022a-4da1-11f0-9041-5185b003f8e6",
                    InterstitialUnitID = "0a8a8fd6-4da2-11f0-9d3e-83a99a70401a",
                    RewardedUnitID = "51790bd4-4da2-11f0-9132-83e4eab65dac",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.Papers, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "dba79f06-4da1-11f0-8a9f-25f08360ccf4",
                    InterstitialUnitID = "0dea2d9e-4da2-11f0-9240-a39af68694c7",
                    RewardedUnitID = "55844036-4da2-11f0-9b09-4f6342d49a9f",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.HeroesOfEnvell, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "df4560d0-4da1-11f0-b237-f580f2e0a96f",
                    InterstitialUnitID = "1176802a-4da2-11f0-9540-8f468b6fd0be",
                    RewardedUnitID = "5a9f3f6c-4da2-11f0-a27b-43c283411aea",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.FourACube, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "e3ab7542-4da1-11f0-aad9-4d5456149cbd",
                    InterstitialUnitID = "15662316-4da2-11f0-8881-279d7c51b840",
                    RewardedUnitID = "5ed9b404-4da2-11f0-af63-f13299712122",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.ThreeCatsAdventure, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "eef04f4a-be3a-11f0-8066-4749a982424f",
                    InterstitialUnitID = "dd58e166-be3a-11f0-8399-759ec4db61c1",
                    RewardedUnitID = "e990663e-be3a-11f0-99e8-a1b76605c2d6",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.ThreeCatsRacing, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "d8b7db2a-c5de-11f0-8a42-ed61f5fbcfc2",
                    InterstitialUnitID = "ca8c81b8-c5de-11f0-aee8-33502acce0ba",
                    RewardedUnitID = "d2126fba-c5de-11f0-b017-d5cf2255756b",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.ThreeCatsPuzzles, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "04363b08-cf52-11f0-a3c2-8ba275a750a8",
                    InterstitialUnitID = "fddc74e8-cf51-11f0-95e1-b3ad3ed53ef0",
                    RewardedUnitID = "0a185704-cf52-11f0-8c69-9f109c566dd9",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            }
        };
        #endregion

#region AppStore IDs
        private static Dictionary<AppName, AdvertisingConfigs> _appStoreAdvertisementID = new Dictionary<AppName, AdvertisingConfigs>()
        {
            {AppName.LeoAndTig, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "ab64de48-4db1-11f0-8f76-d525e73104e5",
                    InterstitialUnitID = "37a4bd38-4db2-11f0-841d-15c88f701e43",
                    RewardedUnitID = "814adf1c-4db2-11f0-8c3d-172f47161daa",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MishkiAdventure, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "afbc4f12-4db1-11f0-ab85-37ce8c35cb35",
                    InterstitialUnitID = "3b0a0938-4db2-11f0-a136-ed3a8ef322a9",
                    RewardedUnitID = "85622c68-4db2-11f0-a021-6d17ad5ba548",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MishkiInSpace, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "b3b6f644-4db1-11f0-90f3-6b306f51cc01",
                    InterstitialUnitID = "3e4a8ff0-4db2-11f0-960a-e17e001dc6d0",
                    RewardedUnitID = "88757be4-4db2-11f0-adef-6bfc99f8d2a2",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MishkiTrueFriend, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "b8af61f4-4db1-11f0-abcb-671abc4dcf00",
                    InterstitialUnitID = "41a554f0-4db2-11f0-9cb8-f5fd12181c09",
                    RewardedUnitID = "8bc3d336-4db2-11f0-9d62-bb1bffb6e660",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.FairytalePatrolCafe, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "fe765f62-4db1-11f0-8518-1ff432d55237",
                    InterstitialUnitID = "4517728a-4db2-11f0-9e2d-8bef51a8f635",
                    RewardedUnitID = "8f5c568a-4db2-11f0-8ed7-89533bdc329a",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.LeoAndTigTaiga, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "05383e88-4db2-11f0-9403-e35118e511a0",
                    InterstitialUnitID = "48308d30-4db2-11f0-b3b7-df79567adcfa",
                    RewardedUnitID = "9309e4d2-4db2-11f0-8d9f-e555e6eb4c26",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MishkiPlanetOfCreativity, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "093375e8-4db2-11f0-b4e3-db6d1dcf3bf7",
                    InterstitialUnitID = "4b67ffb0-4db2-11f0-9bf9-41cdd9464d6c",
                    RewardedUnitID = "a68018a6-4db2-11f0-8c17-211c2caaa0e0",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MishkiBigConcert, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "0cfd60ee-4db2-11f0-8790-21ee244452fc",
                    InterstitialUnitID = "4e91f7e0-4db2-11f0-8283-615fafcdf0d8",
                    RewardedUnitID = "ac21abbc-4db2-11f0-b568-0d82a3415e9b",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.FairytalePatrol, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "14e4c1da-4db2-11f0-b123-dd78a6b81537",
                    InterstitialUnitID = "517f845e-4db2-11f0-9275-fdfddbc43f9b",
                    RewardedUnitID = "b018cdae-4db2-11f0-af6c-ddd94df730a4",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.FairytalePatrolAdventure, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "186fe334-4db2-11f0-a21f-7711fd823508",
                    InterstitialUnitID = "548d9c58-4db2-11f0-9d15-390ccab40ad2",
                    RewardedUnitID = "b3be5456-4db2-11f0-a9a4-998195b8803c",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MusicalPatrol, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "1c4cc13e-4db2-11f0-9e2b-fb8a40b48f2f",
                    InterstitialUnitID = "5785578e-4db2-11f0-a30d-2f0cff47efed",
                    RewardedUnitID = "baff9ad6-4db2-11f0-900a-e1755346024d",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.Multiknowledge, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "1ff321c0-4db2-11f0-992a-45916f110e71",
                    InterstitialUnitID = "5a8a03da-4db2-11f0-a858-336c02e287d9",
                    RewardedUnitID = "be3ee4e0-4db2-11f0-8290-697071137736",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.Papers, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "2339596c-4db2-11f0-86a7-1f3ae49c4b32",
                    InterstitialUnitID = "5dc62c9a-4db2-11f0-a359-5990105f48f8",
                    RewardedUnitID = "c160f898-4db2-11f0-8a29-554016b4eb5e",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.HeroesOfEnvell, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "269e4f0e-4db2-11f0-a804-393655fa0067",
                    InterstitialUnitID = "60d6e168-4db2-11f0-b85c-7b35ba33f3dd",
                    RewardedUnitID = "c4ffc7c2-4db2-11f0-b765-51789fa20fc5",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.FourACube, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "29e17aec-4db2-11f0-87fe-0d80409d1841",
                    InterstitialUnitID = "63c92304-4db2-11f0-9ebc-3de257842752",
                    RewardedUnitID = "c800912c-4db2-11f0-9181-e5687a0a496f",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.ThreeCatsAdventure, new AdvertisingConfigs()
                {
                    PublisherID = "It is required to use manual id input",
                    BannerUnitID = "It is required to use manual id input",
                    InterstitialUnitID = "It is required to use manual id input",
                    RewardedUnitID = "It is required to use manual id input",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.ThreeCatsRacing, new AdvertisingConfigs()
                {
                    PublisherID = "It is required to use manual id input",
                    BannerUnitID = "It is required to use manual id input",
                    InterstitialUnitID = "It is required to use manual id input",
                    RewardedUnitID = "It is required to use manual id input",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.ThreeCatsPuzzles, new AdvertisingConfigs()
                {
                    PublisherID = "It is required to use manual id input",
                    BannerUnitID = "It is required to use manual id input",
                    InterstitialUnitID = "It is required to use manual id input",
                    RewardedUnitID = "It is required to use manual id input",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            }
        };
        #endregion

#region RuStore IDs
        private static Dictionary<AppName, AdvertisingConfigs> _ruStoreAdvertisementID = new Dictionary<AppName, AdvertisingConfigs>()
        {
            {AppName.LeoAndTig, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "5986f3b2-4da3-11f0-a3ca-17b6ce13390b",
                    InterstitialUnitID = "9d95696c-4da3-11f0-b5d5-ef2d622df427",
                    RewardedUnitID = "dddc82f8-4da3-11f0-ba99-c3f45114f930",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MishkiAdventure, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "5d76f990-4da3-11f0-88a3-878eba87c3e0",
                    InterstitialUnitID = "a267ab08-4da3-11f0-a6ec-5f14182ef499",
                    RewardedUnitID = "e48f1bec-4da3-11f0-aace-d7fdfbe07a61",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MishkiInSpace, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "62e5adc2-4da3-11f0-be9e-1513793aa1c7",
                    InterstitialUnitID = "a6cd5580-4da3-11f0-a096-07f15847b64a",
                    RewardedUnitID = "f5167492-4da3-11f0-b6df-ad68e7c8271c",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MishkiTrueFriend, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "68020300-4da3-11f0-8a98-db78889a8489",
                    InterstitialUnitID = "aa4aa5d2-4da3-11f0-8e39-874c2c274753",
                    RewardedUnitID = "f9bf4bae-4da3-11f0-baac-ef365283e94d",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.FairytalePatrolCafe, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "6bee44a6-4da3-11f0-8b83-49a79b9dbb40",
                    InterstitialUnitID = "adf5dc24-4da3-11f0-b540-07096b49d81a",
                    RewardedUnitID = "fcf807b6-4da3-11f0-b57c-876daad7972e",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.LeoAndTigTaiga, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "7009d6b8-4da3-11f0-847e-43b5a52b0652",
                    InterstitialUnitID = "b2cf6738-4da3-11f0-b4e5-31cf23d17b29",
                    RewardedUnitID = "00d7f1ca-4da4-11f0-b5e3-5bcb9ad60d34",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MishkiPlanetOfCreativity, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "755640f2-4da3-11f0-b8ba-25d32c892c0d",
                    InterstitialUnitID = "b7281596-4da3-11f0-8e09-195a41e9fba1",
                    RewardedUnitID = "04efe6f0-4da4-11f0-9ba4-e19462e2d821",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MishkiBigConcert, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "7979faa2-4da3-11f0-87b4-bde6d04059bd",
                    InterstitialUnitID = "bca79992-4da3-11f0-a8fa-f1e09df0efe0",
                    RewardedUnitID = "091ba836-4da4-11f0-9345-affe9c95f50b",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.FairytalePatrol, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "7d1e9258-4da3-11f0-8dfd-ff78e0ed075e",
                    InterstitialUnitID = "c01c1ea4-4da3-11f0-9c24-cb142d74199a",
                    RewardedUnitID = "0d0e26da-4da4-11f0-bcb8-cb11e93aa55c",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.FairytalePatrolAdventure, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "8090670e-4da3-11f0-a489-81ed7c3b96e5",
                    InterstitialUnitID = "c4c2e2d0-4da3-11f0-98b5-7b4de7eaf140",
                    RewardedUnitID = "10ac98c6-4da4-11f0-81dc-c73a01f4b6ed",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MusicalPatrol, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "8460754a-4da3-11f0-9646-f5e6df49ae5c",
                    InterstitialUnitID = "c82a31f8-4da3-11f0-a020-9fed85eff9c9",
                    RewardedUnitID = "151c13e6-4da4-11f0-a328-87bc648db661",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.Multiknowledge, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "882118e2-4da3-11f0-b426-8f60a32732cb",
                    InterstitialUnitID = "cc6578d6-4da3-11f0-8ebc-e7bde4ee011b",
                    RewardedUnitID = "1a314f36-4da4-11f0-805d-4f63f29985b1",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.Papers, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "8bbdf3bc-4da3-11f0-8f86-597b752dcfb2",
                    InterstitialUnitID = "cfde20ee-4da3-11f0-8497-95ccf149c511",
                    RewardedUnitID = "1fa1a682-4da4-11f0-97b8-7d4947295e81",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.HeroesOfEnvell, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "8f580e0e-4da3-11f0-9011-153ef5c54592",
                    InterstitialUnitID = "d39b85f0-4da3-11f0-8236-6f632f435c7f",
                    RewardedUnitID = "24870a66-4da4-11f0-b369-f987e86f7afb",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.FourACube, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "d618bca6-4db4-11f0-9be4-a9a4fd006812",
                    InterstitialUnitID = "dd63d75c-4db4-11f0-9e2d-5182d12566b0",
                    RewardedUnitID = "e4b80280-4db4-11f0-97be-db3800574528",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.ThreeCatsAdventure, new AdvertisingConfigs()
                {
                    PublisherID = "It is required to use manual id input",
                    BannerUnitID = "It is required to use manual id input",
                    InterstitialUnitID = "It is required to use manual id input",
                    RewardedUnitID = "It is required to use manual id input",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.ThreeCatsRacing, new AdvertisingConfigs()
                {
                    PublisherID = "It is required to use manual id input",
                    BannerUnitID = "It is required to use manual id input",
                    InterstitialUnitID = "It is required to use manual id input",
                    RewardedUnitID = "It is required to use manual id input",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.ThreeCatsPuzzles, new AdvertisingConfigs()
                {
                    PublisherID = "It is required to use manual id input",
                    BannerUnitID = "It is required to use manual id input",
                    InterstitialUnitID = "It is required to use manual id input",
                    RewardedUnitID = "It is required to use manual id input",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            }
        };
        #endregion

#region Huawei Store IDs
        private static Dictionary<AppName, AdvertisingConfigs> _huaweiStoreAdvertisementID = new Dictionary<AppName, AdvertisingConfigs>()
        {
            {AppName.LeoAndTig, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "7253c9fa-4db3-11f0-b179-eb8c36031643",
                    InterstitialUnitID = "c95c0a50-4db3-11f0-bd5c-fb2e2417b199",
                    RewardedUnitID = "0a328220-4db4-11f0-87a2-4d268583b649",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MishkiAdventure, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "778f8e22-4db3-11f0-bf42-5fca6bec8e4d",
                    InterstitialUnitID = "cdb15a88-4db3-11f0-ba8a-2b237060916d",
                    RewardedUnitID = "0e4d449e-4db4-11f0-a51f-3f427eed6cb9",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MishkiInSpace, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "7b4ee3e6-4db3-11f0-ba78-47ab44d077e7",
                    InterstitialUnitID = "d1b8754e-4db3-11f0-ab33-63c3b9c7cb56",
                    RewardedUnitID = "12a3fe5c-4db4-11f0-bb90-3f053c45181c",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MishkiTrueFriend, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "7ebccad4-4db3-11f0-ab96-d1de2091eaa8",
                    InterstitialUnitID = "d701ac46-4db3-11f0-a022-b9b446fe7a76",
                    RewardedUnitID = "1712455c-4db4-11f0-8e1c-d3e298ea4808",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.FairytalePatrolCafe, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "836fc4dc-4db3-11f0-b295-15876b83e2d7",
                    InterstitialUnitID = "d9f0f614-4db3-11f0-b7f3-37acb25ae60a",
                    RewardedUnitID = "1c0c5b60-4db4-11f0-8903-af316247af32",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.LeoAndTigTaiga, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "869f0de8-4db3-11f0-8109-637b9885a39b",
                    InterstitialUnitID = "dce530a6-4db3-11f0-8007-635f89f8b055",
                    RewardedUnitID = "1f611a4e-4db4-11f0-8aa8-5f1a3140d040",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MishkiPlanetOfCreativity, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "8a7e5bb2-4db3-11f0-a57f-71b771df2761",
                    InterstitialUnitID = "e027de6c-4db3-11f0-b7b8-556c6e475cbb",
                    RewardedUnitID = "231b6522-4db4-11f0-8c9b-9b1403d6e6d2",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MishkiBigConcert, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "8d22ed38-4db3-11f0-a0ca-fbcbf6ee2699",
                    InterstitialUnitID = "e39adebe-4db3-11f0-9308-e3736328abfb",
                    RewardedUnitID = "2636bf2c-4db4-11f0-abc8-cbfc9ffa66ef",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.FairytalePatrol, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "910133d8-4db3-11f0-878f-ed60d84a7a7a",
                    InterstitialUnitID = "e6c47b36-4db3-11f0-8dfc-a9576082c251",
                    RewardedUnitID = "290e9116-4db4-11f0-a091-2fb704d75ff7",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.FairytalePatrolAdventure, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "a8976abc-4db3-11f0-8c6a-4f4d057a0553",
                    InterstitialUnitID = "ea69750c-4db3-11f0-902d-5731954b6453",
                    RewardedUnitID = "2cb49bf8-4db4-11f0-86cc-1979d7dcba94",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MusicalPatrol, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "ac756b20-4db3-11f0-902f-d7ed4bb08a93",
                    InterstitialUnitID = "ed8821c0-4db3-11f0-a3e9-d95292177850",
                    RewardedUnitID = "2fd2c3dc-4db4-11f0-aaea-11267994398b",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.Multiknowledge, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "b0d14cde-4db3-11f0-b8e0-c3e98bfda683",
                    InterstitialUnitID = "f0e25610-4db3-11f0-b212-7d621f711cd8",
                    RewardedUnitID = "340a1ffe-4db4-11f0-b8bb-f93dc01fdc02",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.Papers, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "b505b510-4db3-11f0-b613-69e213277c02",
                    InterstitialUnitID = "f52466c8-4db3-11f0-ac03-ffc39700b7e3",
                    RewardedUnitID = "37e405cc-4db4-11f0-95a9-652ef22717ed",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.HeroesOfEnvell, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "b9bf744c-4db3-11f0-8e08-1f95f40b1c48",
                    InterstitialUnitID = "f7e81ca6-4db3-11f0-8c19-73d9eb4fdcb0",
                    RewardedUnitID = "3b3d25c8-4db4-11f0-bcee-ab57b7b4fa0e",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.FourACube, new AdvertisingConfigs()
                {
                    PublisherID = _publisherID,
                    BannerUnitID = "be6353b0-4db3-11f0-b644-e70ac6086740",
                    InterstitialUnitID = "fb30388a-4db3-11f0-8b75-65ba89cd4025",
                    RewardedUnitID = "3f56a7e2-4db4-11f0-a8ba-8d2241e3dd1d",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.ThreeCatsAdventure, new AdvertisingConfigs()
                {
                    PublisherID = "It is required to use manual id input",
                    BannerUnitID = "It is required to use manual id input",
                    InterstitialUnitID = "It is required to use manual id input",
                    RewardedUnitID = "It is required to use manual id input",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.ThreeCatsRacing, new AdvertisingConfigs()
                {
                    PublisherID = "It is required to use manual id input",
                    BannerUnitID = "It is required to use manual id input",
                    InterstitialUnitID = "It is required to use manual id input",
                    RewardedUnitID = "It is required to use manual id input",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.ThreeCatsPuzzles, new AdvertisingConfigs()
                {
                    PublisherID = "It is required to use manual id input",
                    BannerUnitID = "It is required to use manual id input",
                    InterstitialUnitID = "It is required to use manual id input",
                    RewardedUnitID = "It is required to use manual id input",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            }
        };
        #endregion

#region Yandex AD IDs
        private static Dictionary<AppName, AdvertisingConfigs> _testYandexAdvertisementID = new Dictionary<AppName, AdvertisingConfigs>()
        {
            {
                AppName.TestYandexAD, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "demo-banner-yandex",
                    InterstitialUnitID = "demo-interstitial-yandex",
                    RewardedUnitID = "demo-rewarded-yandex",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            }
        };
        #endregion

#region GooglePlay Yandex IDs
        private static Dictionary<AppName, AdvertisingConfigs> _googleYandexAdvertisementID = new Dictionary<AppName, AdvertisingConfigs>()
        {
            {AppName.LeoAndTig, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17448964-1",
                    InterstitialUnitID = "R-M-17448964-2",
                    RewardedUnitID = "R-M-17448964-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MishkiAdventure, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17449005-1",
                    InterstitialUnitID = "R-M-17449005-2",
                    RewardedUnitID = "R-M-17449005-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MishkiInSpace, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17449027-1",
                    InterstitialUnitID = "R-M-17449027-2",
                    RewardedUnitID = "R-M-17449027-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MishkiTrueFriend, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17083797-1",
                    InterstitialUnitID = "R-M-17083797-2",
                    RewardedUnitID = "R-M-17083797-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.FairytalePatrolCafe, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17449028-1",
                    InterstitialUnitID = "R-M-17449028-2",
                    RewardedUnitID = "R-M-17449028-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.LeoAndTigTaiga, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17449088-1",
                    InterstitialUnitID = "R-M-17449088-2",
                    RewardedUnitID = "R-M-17449088-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MishkiPlanetOfCreativity, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17449133-1",
                    InterstitialUnitID = "R-M-17449133-2",
                    RewardedUnitID = "R-M-17449133-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MishkiBigConcert, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17485277-1",
                    InterstitialUnitID = "R-M-17485277-2",
                    RewardedUnitID = "R-M-17485277-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.FairytalePatrol, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17485292-1",
                    InterstitialUnitID = "R-M-17485292-2",
                    RewardedUnitID = "R-M-17485292-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.FairytalePatrolAdventure, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17485295-1",
                    InterstitialUnitID = "R-M-17485295-2",
                    RewardedUnitID = "R-M-17485295-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MusicalPatrol, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17485346-1",
                    InterstitialUnitID = "R-M-17485346-2",
                    RewardedUnitID = "R-M-17485346-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.Multiknowledge, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17485374-1",
                    InterstitialUnitID = "R-M-17485374-2",
                    RewardedUnitID = "R-M-17485374-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.Papers, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17485406-1",
                    InterstitialUnitID = "R-M-17485406-2",
                    RewardedUnitID = "R-M-17485406-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.HeroesOfEnvell, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17485410-1",
                    InterstitialUnitID = "R-M-17485410-2",
                    RewardedUnitID = "R-M-17485410-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.FourACube, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17485454-1",
                    InterstitialUnitID = "R-M-17485454-2",
                    RewardedUnitID = "R-M-17485454-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.ThreeCatsAdventure, new AdvertisingConfigs()
                {
                    PublisherID = "It is required to use manual id input",
                    BannerUnitID = "It is required to use manual id input",
                    InterstitialUnitID = "It is required to use manual id input",
                    RewardedUnitID = "It is required to use manual id input",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.ThreeCatsRacing, new AdvertisingConfigs()
                {
                    PublisherID = "It is required to use manual id input",
                    BannerUnitID = "It is required to use manual id input",
                    InterstitialUnitID = "It is required to use manual id input",
                    RewardedUnitID = "It is required to use manual id input",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.ThreeCatsPuzzles, new AdvertisingConfigs()
                {
                    PublisherID = "It is required to use manual id input",
                    BannerUnitID = "It is required to use manual id input",
                    InterstitialUnitID = "It is required to use manual id input",
                    RewardedUnitID = "It is required to use manual id input",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            }
        };
        #endregion

#region AppStore Yandex IDs
        private static Dictionary<AppName, AdvertisingConfigs> _appStoreYandexAdvertisementID = new Dictionary<AppName, AdvertisingConfigs>()
        {
            {AppName.LeoAndTig, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17416046-1",
                    InterstitialUnitID = "R-M-17416046-2",
                    RewardedUnitID = "R-M-17416046-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MishkiAdventure, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17416162-1",
                    InterstitialUnitID = "R-M-17416162-2",
                    RewardedUnitID = "R-M-17416162-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MishkiInSpace, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17416125-1",
                    InterstitialUnitID = "R-M-17416125-2",
                    RewardedUnitID = "R-M-17416125-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MishkiTrueFriend, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17376046-1",
                    InterstitialUnitID = "R-M-17376046-2",
                    RewardedUnitID = "R-M-17376046-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.FairytalePatrolCafe, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17416204-1",
                    InterstitialUnitID = "R-M-17416204-2",
                    RewardedUnitID = "R-M-17416204-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.LeoAndTigTaiga, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17416069-1",
                    InterstitialUnitID = "R-M-17416069-2",
                    RewardedUnitID = "R-M-17416069-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MishkiPlanetOfCreativity, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17416144-1",
                    InterstitialUnitID = "R-M-17416144-2",
                    RewardedUnitID = "R-M-17416144-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MishkiBigConcert, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17416096-1",
                    InterstitialUnitID = "R-M-17416096-2",
                    RewardedUnitID = "R-M-17416096-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.FairytalePatrol, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17416188-1",
                    InterstitialUnitID = "R-M-17416188-2",
                    RewardedUnitID = "R-M-17416188-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.FairytalePatrolAdventure, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17416234-1",
                    InterstitialUnitID = "R-M-17416234-2",
                    RewardedUnitID = "R-M-17416234-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MusicalPatrol, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17416183-1",
                    InterstitialUnitID = "R-M-17416183-2",
                    RewardedUnitID = "R-M-17416183-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.Multiknowledge, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17416146-1",
                    InterstitialUnitID = "R-M-17416146-2",
                    RewardedUnitID = "R-M-17416146-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.Papers, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17416025-1",
                    InterstitialUnitID = "R-M-17416025-2",
                    RewardedUnitID = "R-M-17416025-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.HeroesOfEnvell, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17416042-1",
                    InterstitialUnitID = "R-M-17416042-2",
                    RewardedUnitID = "R-M-17416042-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.FourACube, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17416268-1",
                    InterstitialUnitID = "R-M-17416268-2",
                    RewardedUnitID = "R-M-17416268-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.ThreeCatsAdventure, new AdvertisingConfigs()
                {
                    PublisherID = "It is required to use manual id input",
                    BannerUnitID = "It is required to use manual id input",
                    InterstitialUnitID = "It is required to use manual id input",
                    RewardedUnitID = "It is required to use manual id input",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.ThreeCatsRacing, new AdvertisingConfigs()
                {
                    PublisherID = "It is required to use manual id input",
                    BannerUnitID = "It is required to use manual id input",
                    InterstitialUnitID = "It is required to use manual id input",
                    RewardedUnitID = "It is required to use manual id input",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.ThreeCatsPuzzles, new AdvertisingConfigs()
                {
                    PublisherID = "It is required to use manual id input",
                    BannerUnitID = "It is required to use manual id input",
                    InterstitialUnitID = "It is required to use manual id input",
                    RewardedUnitID = "It is required to use manual id input",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            }
        };
        #endregion

#region RuStore Yandex IDs
        private static Dictionary<AppName, AdvertisingConfigs> _ruStoreYandexAdvertisementID = new Dictionary<AppName, AdvertisingConfigs>()
        {
            {AppName.LeoAndTig, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17446186-1",
                    InterstitialUnitID = "R-M-17446186-2",
                    RewardedUnitID = "R-M-17446186-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MishkiAdventure, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17446200-1",
                    InterstitialUnitID = "R-M-17446200-2",
                    RewardedUnitID = "R-M-17446200-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MishkiInSpace, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17446269-1",
                    InterstitialUnitID = "R-M-17446269-2",
                    RewardedUnitID = "R-M-17446269-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MishkiTrueFriend, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17446356-1",
                    InterstitialUnitID = "R-M-17446356-2",
                    RewardedUnitID = "R-M-17446356-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.FairytalePatrolCafe, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17446429-1",
                    InterstitialUnitID = "R-M-17446429-2",
                    RewardedUnitID = "R-M-17446429-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.LeoAndTigTaiga, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17446461-1",
                    InterstitialUnitID = "R-M-17446461-2",
                    RewardedUnitID = "R-M-17446461-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MishkiPlanetOfCreativity, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17446482-1",
                    InterstitialUnitID = "R-M-17446482-2",
                    RewardedUnitID = "R-M-17446482-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MishkiBigConcert, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17446497-1",
                    InterstitialUnitID = "R-M-17446497-2",
                    RewardedUnitID = "R-M-17446497-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.FairytalePatrol, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17446546-1",
                    InterstitialUnitID = "R-M-17446546-2",
                    RewardedUnitID = "R-M-17446546-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.FairytalePatrolAdventure, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17446619-1",
                    InterstitialUnitID = "R-M-17446619-2",
                    RewardedUnitID = "R-M-17446619-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MusicalPatrol, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17446647-1",
                    InterstitialUnitID = "R-M-17446647-2",
                    RewardedUnitID = "R-M-17446647-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.Multiknowledge, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17446682-1",
                    InterstitialUnitID = "R-M-17446682-2",
                    RewardedUnitID = "R-M-17446682-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.Papers, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17446691-1",
                    InterstitialUnitID = "R-M-17446691-2",
                    RewardedUnitID = "R-M-17446691-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.HeroesOfEnvell, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17446704-1",
                    InterstitialUnitID = "R-M-17446704-2",
                    RewardedUnitID = "R-M-17446704-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.FourACube, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17446721-1",
                    InterstitialUnitID = "R-M-17446721-2",
                    RewardedUnitID = "R-M-17446721-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.ThreeCatsAdventure, new AdvertisingConfigs()
                {
                    PublisherID = "It is required to use manual id input",
                    BannerUnitID = "It is required to use manual id input",
                    InterstitialUnitID = "It is required to use manual id input",
                    RewardedUnitID = "It is required to use manual id input",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.ThreeCatsRacing, new AdvertisingConfigs()
                {
                    PublisherID = "It is required to use manual id input",
                    BannerUnitID = "It is required to use manual id input",
                    InterstitialUnitID = "It is required to use manual id input",
                    RewardedUnitID = "It is required to use manual id input",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.ThreeCatsPuzzles, new AdvertisingConfigs()
                {
                    PublisherID = "It is required to use manual id input",
                    BannerUnitID = "It is required to use manual id input",
                    InterstitialUnitID = "It is required to use manual id input",
                    RewardedUnitID = "It is required to use manual id input",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            }
        };
        #endregion

#region Huawei Store IDs
        private static Dictionary<AppName, AdvertisingConfigs> _huaweiStoreYandexAdvertisementID = new Dictionary<AppName, AdvertisingConfigs>()
        {
            {AppName.LeoAndTig, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17447106-1",
                    InterstitialUnitID = "R-M-17447106-2",
                    RewardedUnitID = "R-M-17447106-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MishkiAdventure, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17447166-1",
                    InterstitialUnitID = "R-M-17447166-2",
                    RewardedUnitID = "R-M-17447166-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MishkiInSpace, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17447207-1",
                    InterstitialUnitID = "R-M-17447207-2",
                    RewardedUnitID = "R-M-17447207-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MishkiTrueFriend, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17447244-1",
                    InterstitialUnitID = "R-M-17447244-2",
                    RewardedUnitID = "R-M-17447244-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.FairytalePatrolCafe, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17447441-1",
                    InterstitialUnitID = "R-M-17447441-2",
                    RewardedUnitID = "R-M-17447441-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.LeoAndTigTaiga, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17447446-1",
                    InterstitialUnitID = "R-M-17447446-2",
                    RewardedUnitID = "R-M-17447446-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MishkiPlanetOfCreativity, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17447464-1",
                    InterstitialUnitID = "R-M-17447464-2",
                    RewardedUnitID = "R-M-17447464-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MishkiBigConcert, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17447506-1",
                    InterstitialUnitID = "R-M-17447506-2",
                    RewardedUnitID = "R-M-17447506-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.FairytalePatrol, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17447534-1",
                    InterstitialUnitID = "R-M-17447534-2",
                    RewardedUnitID = "R-M-17447534-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.FairytalePatrolAdventure, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17447577-1",
                    InterstitialUnitID = "R-M-17447577-2",
                    RewardedUnitID = "R-M-17447577-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.MusicalPatrol, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17447617-1",
                    InterstitialUnitID = "R-M-17447617-2",
                    RewardedUnitID = "R-M-17447617-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.Multiknowledge, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17447665-1",
                    InterstitialUnitID = "R-M-17447665-2",
                    RewardedUnitID = "R-M-17447665-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.Papers, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17480693-1",
                    InterstitialUnitID = "R-M-17480693-2",
                    RewardedUnitID = "R-M-17480693-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.HeroesOfEnvell, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17480738-1",
                    InterstitialUnitID = "R-M-17480738-2",
                    RewardedUnitID = "R-M-17480738-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.FourACube, new AdvertisingConfigs()
                {
                    PublisherID = _yandexPublisherID,
                    BannerUnitID = "R-M-17480767-1",
                    InterstitialUnitID = "R-M-17480767-2",
                    RewardedUnitID = "R-M-17480767-3",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.ThreeCatsAdventure, new AdvertisingConfigs()
                {
                    PublisherID = "It is required to use manual id input",
                    BannerUnitID = "It is required to use manual id input",
                    InterstitialUnitID = "It is required to use manual id input",
                    RewardedUnitID = "It is required to use manual id input",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.ThreeCatsRacing, new AdvertisingConfigs()
                {
                    PublisherID = "It is required to use manual id input",
                    BannerUnitID = "It is required to use manual id input",
                    InterstitialUnitID = "It is required to use manual id input",
                    RewardedUnitID = "It is required to use manual id input",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            },
            {AppName.ThreeCatsPuzzles, new AdvertisingConfigs()
                {
                    PublisherID = "It is required to use manual id input",
                    BannerUnitID = "It is required to use manual id input",
                    InterstitialUnitID = "It is required to use manual id input",
                    RewardedUnitID = "It is required to use manual id input",
                    AppPrivacyPolicyURL = _appPrivacyPolicyURL
                }
            }
        };
        #endregion
    }
}
