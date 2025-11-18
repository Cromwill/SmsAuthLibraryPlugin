namespace KinDzaDzaGames.AdvertisementPlugin.EditorScripts
{
    public static class ASMDEFSettings
    {
        public const string GUID = "GUID:";
        public const string ASMDEFSIGNATURE = "t:asmdef";

        public class YabbiAds
        {
            public const string YabbiDefine = "YABBI_AD";
            public const string SspnetSDK = "SspnetSDK";
            public const string SspnetSDKEditor = "SspnetSDK.Editor";
            public const string YabbiSDK = "YabbiSDK";
            public const string YabbiSDKEditor = "YabbiSDK.Editor";
            public const string FilePathSspnetSDK = "Assets/SspnetSDK/SspnetSDK.asmdef";
            public const string FilePathSspnetSDKEditor = "Assets/SspnetSDK/Editor/SspnetSDK.Editor.asmdef";
            public const string FilePathYabbiSDK = "Assets/YabbiSDK/YabbiSDK.asmdef";
            public const string FilePathYabbiSDKEditor = "Assets/YabbiSDK/Editor/YabbiSDK.Editor.asmdef";
        }

        public class YandexAds
        {
            public const string YandexDefine = "YANDEX_AD";
            public const string YandexSDK = "YandexSDK";
            public const string YandexSDKEditor = "YandexSDK.EditorScripts";
            public const string FilePathYandexSDK = "Assets/YandexMobileAds/YandexSDK.asmdef";
            public const string FilePathYandexSDKEditor = "Assets/YandexMobileAds/Editor/YandexSDK.EditorScripts.asmdef";
        }

        public class ExcludePlatforms
        {
            public const string Android = "Android";
            public const string iOS = "iOS";
            public const string Editor = "Editor";
        }

        public class IosATT
        {
            public const string ATTASMDEF = "Unity.Advertisement.IosSupport.asmdef";
        }
    }
}
