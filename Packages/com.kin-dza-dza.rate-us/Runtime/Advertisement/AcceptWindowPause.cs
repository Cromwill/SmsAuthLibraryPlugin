using KinDzaDzaGames.AdvertisementPlugin;
using UnityEngine;
using UnityEngine.Scripting;

#if ADVERTISEMENT
[assembly: AlwaysLinkAssembly]
#endif

namespace KinDzaDza.RateUs.Advertisement
{
    [Preserve]
    public class AcceptWindowPause
    {
        [Preserve]
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Register()
        {
            Debug.Log($"[{nameof(AcceptWindowPause)}]: registred");

            RateUsPopupTimer.IsAcceptWindowPaused = () =>
            {
                AdvertisementController advertisement = AdvertisementController.Instance;

                if (advertisement == null)
                {
                    return false;
                }

                bool consentWindowOpen = advertisement.WaitConcernPolicy && advertisement.PolicyAccepted == false && advertisement.AgreementClosed == false;

                return consentWindowOpen;
            };
        }
    }
}
