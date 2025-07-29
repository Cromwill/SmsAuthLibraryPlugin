using UnityEngine;
using System.Collections;
using UnityEngine.Scripting;

namespace KinDzaDzaGames.AdvertisementPlugin
{
    [Preserve]
    public class InternetChecker : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;

        private bool _isOpened = false;

        public IEnumerator EnternetChecking()
        {
            DontDestroyOnLoad(this);

            var wait = new WaitForSecondsRealtime(1f);

            while (true)
            {
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    _isOpened = true;
                    _canvasGroup.alpha = 1f;
                    _canvasGroup.blocksRaycasts = true;
                    Debug.LogError("NO CONNECTION");
                }
                else
                {
                    if(_isOpened)
                    {
                        _isOpened = false;
                        _canvasGroup.alpha = 0f;
                        _canvasGroup.blocksRaycasts = false;
                    }
                }

                yield return wait;
            }
        }
    }
}
