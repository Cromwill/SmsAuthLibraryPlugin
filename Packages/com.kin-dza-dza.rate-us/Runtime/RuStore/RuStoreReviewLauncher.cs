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
            Debug.Log($"[{nameof(RuStoreReviewLauncher)}]: Try open RuStore review popup");
            RuStoreReviewManager manager = RuStoreReviewManager.Instance;

            if (manager.IsInitialized == false && manager.Init() == false)
            {
                Debug.LogError($"[{nameof(RuStoreReviewLauncher)}]: RuStore review initializing failed!");
                CompleteOpening?.Invoke(false);

                return;
            }

            manager.RequestReviewFlow(
                onFailure: error =>
                {
                    Debug.LogError($"[{nameof(RuStoreReviewLauncher)}]: RuStore RequestReviewFlow: {error.name} {error.description}");
                    CompleteOpening?.Invoke(false);
                },
                onSuccess: () =>
                {
                    manager.LaunchReviewFlow(
                        onFailure: error =>
                        {
                            Debug.LogError($"[{nameof(RuStoreReviewLauncher)}]: RuStore LaunchReviewFlow: {error.name} {error.description}");
                            CompleteOpening?.Invoke(false);
                        },
                        onSuccess: () =>
                        {
                            Debug.Log($"[{nameof(RuStoreReviewLauncher)}]: RuStore review window successfully opened");
                            CompleteOpening?.Invoke(true);
                        });
                });
        }

        [Preserve]
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Register()
        {
            Debug.Log($"[{nameof(RuStoreReviewLauncher)}]: registred");

            RateUsPopup.LaunchRuStore = popup =>
            {
                RuStoreReviewLauncher launcher = new RuStoreReviewLauncher();

                launcher.CompleteOpening += opened =>
                {
                    if (opened)
                    {
                        popup.Hide();
                    }
                    else
                    {
                        popup.OpenStorePage();
                    }
                };

                launcher.TryOpenRuStorePopup();
            };
        }
    }
}
