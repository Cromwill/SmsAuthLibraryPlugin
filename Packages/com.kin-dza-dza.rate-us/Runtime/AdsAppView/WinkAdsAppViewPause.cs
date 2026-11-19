using AdsAppView.Program;
using UnityEngine;
using UnityEngine.Scripting;

#if ADS_APP_VIEW
[assembly: AlwaysLinkAssembly]
#endif

namespace KinDzaDza.RateUs.AdsAppView
{
    [Preserve]
    public class WinkAdsAppViewPause
    {
        private static bool _popupOpened;

        [Preserve]
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Register()
        {
            GamePause.GamePaused += () => _popupOpened = true;
            GamePause.GameUnpaused += () => _popupOpened = false;

            RateUsPopupTimer.IsPopupPaused = () => _popupOpened;
        }

        [Preserve]
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void RegisterRateUsPause()
        {
            if (RateUsPopup.Instance == null)
            {
                return;
            }

            RateUsPopup.Instance.Shown += PausePopups;
            RateUsPopup.Instance.Closed += ResumePopups;
        }

        private static void PausePopups()
        {
            if (PopupManager.Instance != null)
            {
                PopupManager.SetPause(true);
            }
        }

        private static void ResumePopups()
        {
            if (PopupManager.Instance != null)
            {
                PopupManager.SetPause(false);
            }
        }
    }
}
