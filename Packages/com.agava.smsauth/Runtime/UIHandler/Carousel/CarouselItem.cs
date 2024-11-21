using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Agava.Wink
{
    public class CarouselItem : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private Image _mask;

        Coroutine _coroutine;

        public Vector3 Position { get; private set; }
        public Vector3 Scale { get; private set; }

        private void Awake()
        {
            UpdatePositionAndScale();
        }

        public void SetSprite(Sprite sprite)
        {
            _image.sprite = sprite;
        }

        public void Hide()
        {
            _image.enabled = false;
            _mask.enabled = false;
        }

        public void Show()
        {
            _image.enabled = true;
            _mask.enabled = true;
        }

        public void OneCycle(Vector3 targetPosition, Vector3 targetScale, float duration)
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }

            _coroutine = StartCoroutine(Moving(targetPosition, targetScale, duration));

            IEnumerator Moving(Vector3 targetPosition, Vector3 targetScale, float duration)
            {
                Vector3 startScale = transform.localScale;
                Vector3 startPosition = transform.localPosition;

                float elapsedTime = 0;
                float delta;

                while (elapsedTime < duration)
                {
                    elapsedTime += Time.unscaledDeltaTime;
                    delta = elapsedTime / duration;

                    transform.localPosition = Vector3.Lerp(startPosition, targetPosition, delta);
                    transform.localScale = Vector3.Lerp(startScale, targetScale, delta);

                    yield return null;
                }

                UpdatePositionAndScale();
                _coroutine = null;
            }
        }

        private void UpdatePositionAndScale()
        {
            Position = transform.localPosition;
            Scale = transform.localScale;
        }
    }
}
