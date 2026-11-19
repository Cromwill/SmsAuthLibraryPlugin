using KinDzaDzaGames.AdvertisementPlugin;
using UnityEngine;
using UnityEngine.Scripting;

#if ADVERTISEMENT
[assembly: AlwaysLinkAssembly]
#endif

namespace KinDzaDza.RateUs.Advertisement
{
    [Preserve]
    public class AdvertisementBannerPause
    {
        [Preserve]
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Register()
        {
            RateUsPopupTimer.IsAdvertisementBannerPaused = () =>
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
