using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;

namespace Agava.Wink
{
    [Preserve]
    public class CarouselItem : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private Image _border;
        [SerializeField] private TMP_Text _label;

        private Coroutine _moveCoroutine;
        private Coroutine _transparentCoroutine;
        private Coroutine _transparentBorderCoroutine;
        private Color _defaultIconColor;
        private Color _defaultTextColor;
        private Color _defaultBorderColor;
        private Color _transparentIconColor;
        private Color _transparentTextColor;
        private Color _transparentBorderColor;

        public int Index { get; private set; }

        public void Construct()
        {
            _defaultIconColor = _transparentIconColor = _image.color;
            _defaultTextColor = _transparentTextColor = _label.color;
            _defaultBorderColor = _transparentBorderColor = _border.color;
            _transparentIconColor.a = 0.5f;
            _transparentTextColor.a = 0.5f;
            _transparentBorderColor.a = 0;
        }

        public void SetPositionIndex(int index)
        {
            Index = index;
        }

        public void Initialize(CarouselData data)
        {
            _image.sprite = data.Sprite;
            _label.text = data.AppName;
        }

        public void Hide()
        {
            _image.enabled = false;
            _label.enabled = false;
        }

        public void Show()
        {
            _image.enabled = true;
            _label.enabled = true;
        }

        public void Stop()
        {
            if (_moveCoroutine != null)
            {
                StopCoroutine(_moveCoroutine);
                _moveCoroutine = null;
            }

            if (_transparentCoroutine != null)
            {
                StopCoroutine(_transparentCoroutine);
                _transparentCoroutine = null;
            }

            if (_transparentBorderCoroutine != null)
            {
                StopCoroutine(_transparentBorderCoroutine);
                _transparentBorderCoroutine = null;
            }
        }

        public void OneCycle(Vector3 targetPosition, float duration, Action<CarouselItem> onEnd = null)
        {
            _moveCoroutine = StartCoroutine(Moving(targetPosition, duration));

            IEnumerator Moving(Vector3 targetPosition, float duration)
            {
                Vector3 startScale = transform.localScale;
                Vector3 startPosition = transform.localPosition;

                if (duration > 0)
                {
                    float elapsedTime = 0;
                    float delta;

                    while (elapsedTime < duration)
                    {
                        elapsedTime += Time.unscaledDeltaTime;
                        delta = elapsedTime / duration;

                        transform.localPosition = Vector3.Lerp(startPosition, targetPosition, delta);

                        yield return null;
                    }
                }

                transform.localPosition = targetPosition;
                onEnd?.Invoke(this);

                _moveCoroutine = null;
            }
        }

        public void MakeTransparent(float duration)
        {
            _transparentCoroutine = StartCoroutine(Transparenting(duration, _transparentIconColor, _transparentTextColor));
        }

        public void MakeOpaque(float duration)
        {
            _transparentCoroutine = StartCoroutine(Transparenting(duration, _defaultIconColor, _defaultTextColor));
        }

        public void  ShowBorder(float duration)
        {
            _border.color = _transparentBorderColor;
            _border.enabled = true;
            _transparentBorderCoroutine = StartCoroutine(Transparenting(duration, _defaultBorderColor));
        }

        public void  HideBorder(float duration)
        {
            _transparentBorderCoroutine = StartCoroutine(Transparenting(duration, _transparentBorderColor, () => _border.enabled = false));
        }

        private IEnumerator Transparenting(float duration, Color targetImageColor, Color targetTextColor)
        {
            Color startImageColor = _image.color;
            Color startTextColor = _label.color;

            if (duration > 0)
            {
                float elapsedTime = 0;
                float delta;

                while (elapsedTime < duration)
                {
                    elapsedTime += Time.unscaledDeltaTime;
                    delta = elapsedTime / duration;

                    _image.color = Color.Lerp(startImageColor, targetImageColor, delta);
                    _label.color = Color.Lerp(startTextColor, targetTextColor, delta);

                    yield return null;
                }
            }

            _image.color = targetImageColor;
            _label.color = targetTextColor;

            _transparentCoroutine = null;
        }

        private IEnumerator Transparenting(float duration, Color targetBorderColor, Action endAction = null)
        {
            Color startImageColor = _border.color;

            if (duration > 0)
            {
                float elapsedTime = 0;
                float delta;

                while (elapsedTime < duration)
                {
                    elapsedTime += Time.unscaledDeltaTime;
                    delta = elapsedTime / duration;

                    _border.color = Color.Lerp(startImageColor, targetBorderColor, delta);

                    yield return null;
                }
            }

            _border.color = targetBorderColor;
            endAction?.Invoke();

            _transparentBorderCoroutine = null;
        }
    }
}
