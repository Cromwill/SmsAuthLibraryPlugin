using UnityEngine;
using UnityEngine.Scripting;

namespace Agava.Wink
{
    [Preserve]
    public static class ScreenshotProtector
    {
        public static bool Locked { get; private set; } = false;

        public static void DisableScreenshots()
        {
            if(Locked == false)
                SetSecureFlag(true);
        }

        public static void EnableScreenshots()
        {
            if (Locked)
                SetSecureFlag(false);
        }

        private static void SetSecureFlag(bool protectScreen)
        {
            Locked = protectScreen;
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject window = activity.Call<AndroidJavaObject>("getWindow");

            AndroidJavaClass layoutParamsClass = new AndroidJavaClass("android.view.WindowManager$LayoutParams");
            int flagSecure = layoutParamsClass.GetStatic<int>("FLAG_SECURE");

            if (protectScreen)
                window.Call("setFlags", flagSecure, flagSecure);
            else
                window.Call("clearFlags", flagSecure);
        }
    }
}
