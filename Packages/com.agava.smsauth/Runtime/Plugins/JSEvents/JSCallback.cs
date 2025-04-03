using System;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;

namespace JSPlugin
{
    public static class JSCallback
    {
        public delegate void JSCallbackMessage(string message);

        private static JSCallbackMessage s_onCallback;

        public static void SetCSharpTimeout(JSCallbackMessage onCallback)
        {
            JsSetTimeout("Hello World", 10, CSSharpCallback);
            s_onCallback = onCallback;
        }

        [DllImport("__Internal")]
        private static extern void JsSetTimeout(string message, int timeout, JSCallbackMessage action);

        [MonoPInvokeCallback(typeof(JSCallbackMessage))]
        private static void CSSharpCallback(string message)
        {
            s_onCallback?.Invoke(message);
        }
    }
}
