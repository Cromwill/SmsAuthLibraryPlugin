using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Scripting;

namespace Agava.Wink
{
    [Preserve]
    public class ScreenshotProtector : MonoBehaviour
    {
        [SerializeField] private WebViewPresenter _webViewPresenter;
        [SerializeField] private GameObject _warningMessage;

        private bool _screenshotsDisabled = false;

        [DllImport("__Internal")]
        private static extern void startScreenshotDetection();

        [DllImport("__Internal")]
        private static extern void stopScreenshotDetection();

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
#elif UNITY_IOS
            //startScreenshotDetection();
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
#elif UNITY_IOS
            //stopScreenshotDetection();
#endif
        }

#if UNITY_IOS
        private void OnScreenshotTaken(string _)
        {
            EnableWarningMessage();
            Invoke(nameof(DisableWarningMessage), 2);
        }

        private void EnableWarningMessage()
        {
            _webViewPresenter.Hide();
            _warningMessage.SetActive(true);
        }

        private void DisableWarningMessage()
        {
            _webViewPresenter.Show();
            _warningMessage.SetActive(false);
        }
#endif

    }
}
