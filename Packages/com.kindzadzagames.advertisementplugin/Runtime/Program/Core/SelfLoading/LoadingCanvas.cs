using UnityEngine;
using UnityEngine.UI;

namespace KinDzaDzaGames.AdvertisementPlugin
{
    internal class LoadingCanvas : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Image _loadingImage;

        private void Update()
        {
            _loadingImage.transform.localEulerAngles += new Vector3(0, 0, 2f);
        }
    }
}
