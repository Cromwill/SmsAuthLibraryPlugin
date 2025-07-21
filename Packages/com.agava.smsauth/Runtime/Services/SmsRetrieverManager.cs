using System;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.Networking;

namespace Agava.Wink
{
    [Preserve]
    public class SmsRetrieverManager : MonoBehaviour
    {
        private const string HashCodePattern = "{code}%2E%0A";

#if UNITY_EDITOR == false && UNITY_ANDROID
        private AndroidJavaClass _smsRetrieverClass;
#endif
        public string HashCode { get; private set; } = string.Empty;

        public event Action <string> SmsReceived;

        public void Construct()
        {
#if UNITY_EDITOR == false && UNITY_ANDROID
            _smsRetrieverClass = new AndroidJavaClass("com.kddg.smsretrieverplugin.SmsRetrieverPlugin");
            _smsRetrieverClass.CallStatic("getAppHash", gameObject.name, "OnAppHashReceived");
#endif
        }

        public void ReloadRetriever()
        {
#if UNITY_EDITOR == false && UNITY_ANDROID
            _smsRetrieverClass.CallStatic("startSmsListener", gameObject.name, "OnSmsReceived");
#endif

            Debug.Log($"SMS Retriever: initialized listener.");
        }

        public void OnSmsReceived(string message)
        {
            SmsReceived?.Invoke(message);
            Debug.Log($"SMS Retriever: sms received. Message: {message}");
        }

        public void OnAppHashReceived(string hash)
        {
            HashCode = HashCodePattern + UnityWebRequest.EscapeURL(hash);
            Debug.Log($"SMS Retriever: hash generated. Hash: {hash}");
        }
    }
}
