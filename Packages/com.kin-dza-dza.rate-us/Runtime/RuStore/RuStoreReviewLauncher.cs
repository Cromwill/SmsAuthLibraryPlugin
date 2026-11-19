using System;
using RuStore.Review;
using UnityEngine;
using UnityEngine.Scripting;

#if RU_STORE
[assembly: AlwaysLinkAssembly]
#endif

namespace KinDzaDza.RateUs.RuStore
{
    [Preserve]
    public class RuStoreReviewLauncher
    {
        public event Action<bool> CompleteOpening;

        public void TryOpenRuStorePopup()
        {
            Debug.LogError("TryOpenRuStorePopup");
            RuStoreReviewManager manager = RuStoreReviewManager.Instance;

            if (manager.IsInitialized == false && manager.Init() == false)
            {
                Debug.LogError("RuStore Review Init failed");
                CompleteOpening?.Invoke(false);

                return;
            }

            manager.RequestReviewFlow(
                onFailure: error =>
                {
                    Debug.LogError($"RuStore RequestReviewFlow: {error.name} {error.description}");
                    CompleteOpening?.Invoke(false);
                },
                onSuccess: () =>
                {
                    manager.LaunchReviewFlow(
                        onFailure: error =>
                        {
                            Debug.LogError($"RuStore LaunchReviewFlow: {error.name} {error.description}");
                            CompleteOpening?.Invoke(false);
                        },
                        onSuccess: () =>
                        {
                            Debug.LogError($"RuStore LaunchReviewFlow TRUE");
                            CompleteOpening?.Invoke(true);
                        });
                });
        }

        [Preserve]
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Register()
        {
            Debug.LogError("Register");
            RateUsPopup.LaunchRuStore = popup =>
            {
                RuStoreReviewLauncher launcher = new RuStoreReviewLauncher();

                launcher.CompleteOpening += opened =>
                {
                    if (opened)
                    {
                        popup.Hide();
                        Debug.LogError("Hide");
                    }
                    else
                    {
                        Debug.LogError("OpenStorePage");
                        popup.OpenStorePage();
                    }
                };

                launcher.TryOpenRuStorePopup();
            };
        }
    }
}
