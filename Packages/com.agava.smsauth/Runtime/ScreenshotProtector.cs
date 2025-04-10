using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Scripting;

namespace Agava.Wink
{
    [Serializable, Preserve]
    public class ScreenshotProtector
    {
        [SerializeField] private GameObject _webView;
        [SerializeField] private GameObject _screenshotProtectorWindow;

        private ICoroutine _coroutineRoot;
        private bool _screenshotsDisabled = false;

        [DllImport("__Internal")]
        private static extern void startScreenshotDetection();

        [DllImport("__Internal")]
        private static extern void stopScreenshotDetection();

        public void Construct(ICoroutine coroutineRoot) => _coroutineRoot = coroutineRoot;

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
                startScreenshotDetection();   
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
                stopScreenshotDetection(); 
#endif
        }

#if UNITY_IOS
        private void OnScreenshotTaken(string _)
        {
            _webView.SetActive(false);
            _screenshotProtectorWindow.SetActive(true);

            _coroutineRoot.StartCoroutine(WaitTwoSeconds());
            IEnumerator WaitTwoSeconds()
            {
                yield return new WaitForSeconds(2);
                _webView.SetActive(true);
                _screenshotProtectorWindow.SetActive(false);
            }
        }
#endif

    }
}
