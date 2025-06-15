using UnityEngine;
using UnityEngine.Scripting;

namespace Agava.Wink
{
    [Preserve]
    public class ScreenshotProtector : MonoBehaviour
    {
        private bool _screenshotsDisabled = false;

        public void TryDisableScreenshots()
        {
            if (_screenshotsDisabled)
                return;

            _screenshotsDisabled = true;

#if UNITY_EDITOR
            Debug.Log("SCREEN PROTECTOR: disable screenshots possibility!");
#elif UNITY_ANDROID
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject myActivityHelper = new AndroidJavaObject("com.kindzadza.screenprotect.ScreenshotProtect");
                myActivityHelper.CallStatic("SetSecureFlag", currentActivity);
            }
#endif
        }

        public void TryEnableScreenshots()
        {
            if (_screenshotsDisabled == false)
                return;

            _screenshotsDisabled = false;

#if UNITY_EDITOR
            Debug.Log("SCREEN PROTECTOR: enable screenshots possibility!");
#elif UNITY_ANDROID
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject myActivityHelper = new AndroidJavaObject("com.kindzadza.screenprotect.ScreenshotProtect");
                myActivityHelper.CallStatic("ClearSecureFlag", currentActivity);
            }
#endif
        }
    }
}
