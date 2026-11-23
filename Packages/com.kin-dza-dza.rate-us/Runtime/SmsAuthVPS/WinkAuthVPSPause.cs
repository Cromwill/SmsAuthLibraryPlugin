using Agava.Wink;
using UnityEngine;
using UnityEngine.Scripting;

#if SMS_AUTH_VPS
[assembly: AlwaysLinkAssembly]
#endif

namespace KinDzaDza.RateUs.SmsAuthVPS
{
    [Preserve]
    public class WinkAuthVPSPause
    {
        [Preserve]
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Register()
        {
            Debug.Log($"[{nameof(WinkAuthVPSPause)}]: registred");

            RateUsPopupTimer.IsPaused = () =>
            {
                WinkSignInHandlerUI wink = WinkSignInHandlerUI.Instance;
                return wink == null || wink.IsAnyWindowEnabled;
            };
        }
    }
}
