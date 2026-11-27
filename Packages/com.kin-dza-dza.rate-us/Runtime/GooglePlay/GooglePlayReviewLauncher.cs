using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Scripting;

#if GOOGLE_PLAY
using Google.Play.Review;
using Google.Play.Common;
#endif

#if GOOGLE_PLAY
[assembly: AlwaysLinkAssembly]
#endif

namespace KinDzaDza.RateUs.GooglePlay
{
    [Preserve]
    public class GooglePlayReviewLauncher
    {
        private const float RequestTimeOut = 5f;

#if GOOGLE_PLAY
        private Coroutine _waitTimeOutCoroutine;
        private ReviewManager _reviewManager;
#endif

        private MonoBehaviour _coroutineRunner;

        public event Action<bool> CompleteOpening;

        public GooglePlayReviewLauncher(MonoBehaviour coroutineRuner)
        {
            _coroutineRunner = coroutineRuner;
            _reviewManager = new ReviewManager();
        }

        public void TryOpenGoogleStorePopup()
        {
            _coroutineRunner.StartCoroutine(OpenGoogleStorePopupCoroutine());

            _waitTimeOutCoroutine ??= _coroutineRunner.StartCoroutine(WaitRating());
        }

        private IEnumerator WaitRating()
        {
            float timer = 0;

            while (timer < RequestTimeOut)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            Debug.Log($"[{nameof(GooglePlayReviewLauncher)}]: popup didn't load in time {RequestTimeOut} seconds");
            CompleteOpening?.Invoke(false);
            _waitTimeOutCoroutine = null;
        }

        private IEnumerator OpenGoogleStorePopupCoroutine()
        {
#if UNITY_EDITOR
            yield return null;
            CompleteOpening?.Invoke(false);
#elif GOOGLE_PLAY
            PlayAsyncOperation<PlayReviewInfo, ReviewErrorCode> requestReviewOperation = _reviewManager.RequestReviewFlow();
            yield return requestReviewOperation;

            if (requestReviewOperation.Error != ReviewErrorCode.NoError)
            {
                CompleteOpening?.Invoke(false);
                yield break;
            }

            if (_waitTimeOutCoroutine != null)
            {
                _coroutineRunner.StopCoroutine(_waitTimeOutCoroutine);
                _waitTimeOutCoroutine = null;
            }

            PlayAsyncOperation<VoidResult, ReviewErrorCode> launchFlowOperation = _reviewManager.LaunchReviewFlow(requestReviewOperation.GetResult());
            yield return launchFlowOperation;

            if (launchFlowOperation.Error != ReviewErrorCode.NoError)
            {
                CompleteOpening?.Invoke(false);
                yield break;
            }

            CompleteOpening?.Invoke(true);
#else
            yield return null;
            CompleteOpening?.Invoke(false);
#endif
        }

        [Preserve]
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Register()
        {
            Debug.Log($"[{nameof(GooglePlayReviewLauncher)}]: registred");

            RateUsPopup.LaunchGoogle = popup =>
            {
                GooglePlayReviewLauncher launcher = new GooglePlayReviewLauncher(popup);

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

                launcher.TryOpenGoogleStorePopup();
            };
        }
    }
}
