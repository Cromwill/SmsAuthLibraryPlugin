using UnityEngine;
using System.Collections;

namespace Agava.Wink
{
    public class InternetAccessChecker : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _panel;

        private void OnEnable()
        {
            StartCoroutine(EnternetChecking());
        }

        private IEnumerator EnternetChecking()
        {
            var wait = new WaitForSecondsRealtime(1f);

            while (gameObject.activeSelf)
            {
                if (Application.internetReachability == NetworkReachability.NotReachable)
                    OpenPanel();
                else
                    ClosePanel();

                yield return wait;
            }
        }

        private void OpenPanel()
        {
            _panel.alpha = 1;
            _panel.interactable = true;
            _panel.blocksRaycasts = true;
        }

        private void ClosePanel()
        {
            _panel.alpha = 0;
            _panel.interactable = false;
            _panel.blocksRaycasts = false;
        }
    }
}
