using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Scripting;

namespace Agava.Wink
{
    [Preserve]
    internal class OrientationСhangeWindowPresenter : WindowPresenter
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private OrientationСhangeAnimation _orientationСhangeAnimation;

        private IInternetChecker _internetChecker;
        private GameOrientation _gameOrientation;
        private Coroutine _waitPhoneRotateCoroutine;
#if UNITY_EDITOR
        private bool _needChangeOrientation = false;
#endif
        public void Construct(GameOrientation gameOrientation, IInternetChecker internetChecker)
        {
            _gameOrientation = gameOrientation ?? throw new ArgumentNullException(nameof(gameOrientation));
            _internetChecker = internetChecker ?? throw new ArgumentNullException(nameof(internetChecker));

            _orientationСhangeAnimation.Construct();
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
                _needChangeOrientation = true;
        }
#endif

        public override void Enable()
        {
            EnableCanvasGroup(_canvasGroup);
            _orientationСhangeAnimation.StartAnimation();
            AnalyticsWinkService.SendChangeOrientationWindow();

            _waitPhoneRotateCoroutine = StartCoroutine(WaitRotatePhone());
        }

        public override void Disable()
        {
            DisableCanvasGroup(_canvasGroup);
            AnalyticsWinkService.SendPlayerRotateDevice();

            if (_waitPhoneRotateCoroutine != null)
            {
                StopCoroutine(_waitPhoneRotateCoroutine);
                _waitPhoneRotateCoroutine = null;
            }

            _orientationСhangeAnimation.StopAnimation();
#if UNITY_EDITOR
            _needChangeOrientation = false;
#endif
        }

        private IEnumerator WaitRotatePhone()
        {
            _gameOrientation.SetLandscapeOrientation();
#if UNITY_EDITOR
            yield return new WaitUntil(() => _needChangeOrientation);
#else
            while(_gameOrientation.ChangedToLandscape == false)
        {
            if(Input.acceleration.x < _gameOrientation.DeltaToLandscapeLeft)
            {
                Screen.orientation = ScreenOrientation.LandscapeLeft;
                Screen.orientation = ScreenOrientation.AutoRotation;
            }
            else if(Input.acceleration.x > _gameOrientation.DeltaToLandscapeRight)
            {
                Screen.orientation = ScreenOrientation.LandscapeRight;
                Screen.orientation = ScreenOrientation.AutoRotation;
            }

            yield return new WaitForSeconds(_gameOrientation.CheckTime);
        }
#endif
            yield return new WaitWhile(() => _internetChecker.HasInternet);

            Disable();
        }
    }
}
