using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;

namespace Agava.Wink
{
    [Preserve]
    internal class SubscriptionCheckWindowPresenter : WindowPresenter
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Image _loadImage;
        [SerializeField, Min(0)] private float _loadTime = 2f;
        [SerializeField, Min(0)] private float _punchScale = 0.1f;
        [SerializeField, Min(0)] private float _punchTime = 0.3f;
        [SerializeField, Min(0)] private int _vibrato = 1;
        [SerializeField] private List<XmlConfigText> _xmlConfigTexts;

        private IInternetChecker _internetChecker;
        private float _lastLoadTime = 0;
        private Coroutine _coroutine = null;

        public event Action LoadingStarted;
        public event Action LoadingCompleted;

        public void Construct(IInternetChecker internetChecker)
        {
            _internetChecker = internetChecker ?? throw new ArgumentNullException(nameof(internetChecker));
        }

        public void FillRemoteTexts() => _xmlConfigTexts.ForEach(t => t.FillText());

        public override void Enable()
        {
            LoadingStarted?.Invoke();
            _lastLoadTime = _loadImage.fillAmount = 0;
            EnableCanvasGroup(_canvasGroup);
            AnalyticsWinkService.SendShowRedirectWindow();

            _coroutine = StartCoroutine(Loading());
        }

        public override void Disable()
        {
            DisableCanvasGroup(_canvasGroup);

            if(_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            _loadImage.transform.DOKill();
            _loadImage.transform.localScale = Vector3.one;
        }

        private IEnumerator Loading()
        {
            while (_lastLoadTime < _loadTime)
            {
                _lastLoadTime += Time.unscaledDeltaTime;
                _loadImage.fillAmount = _lastLoadTime / _loadTime;

                yield return null;
            }

            yield return new WaitWhile(() => _internetChecker.HasInternet);

            _loadImage.transform.DOPunchScale(Vector3.one * _punchScale, _punchTime, _vibrato).SetEase(Ease.InOutSine).OnComplete(() => LoadingCompleted?.Invoke());
        }
    }
}
