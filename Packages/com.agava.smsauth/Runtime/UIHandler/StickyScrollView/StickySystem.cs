using UnityEngine;

namespace Agava.Wink
{
    public class StickySystem : MonoBehaviour
    {
        [SerializeField] private StickyScrollView _stickyScrollView;
        [SerializeField] private CanvasGroup _canvasGroup;

        private bool _visible = false;

        private void OnEnable()
        {
            Hide();

            _stickyScrollView.ScrollRect.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnDisable()
        {
            _stickyScrollView.ScrollRect.onValueChanged.RemoveListener(OnValueChanged);
        }

        private void OnValueChanged(Vector2 position)
        {
            if (transform.position.y < _stickyScrollView.StickyElement.transform.position.y && _visible == false)
                Show();
            else if (transform.position.y >= _stickyScrollView.StickyElement.transform.position.y && _visible)
                Hide();
        }

        private void Hide()
        {
            _visible = false;
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        private void Show()
        {
            _visible = true;
            _canvasGroup.alpha = 1f;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }
    }
}
