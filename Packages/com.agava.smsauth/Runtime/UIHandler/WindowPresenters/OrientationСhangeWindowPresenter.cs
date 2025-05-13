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

        public void Construct(GameOrientation gameOrientation, IInternetChecker internetChecker)
        {
            _gameOrientation = gameOrientation ?? throw new ArgumentNullException(nameof(gameOrientation));
            _internetChecker = internetChecker ?? throw new ArgumentNullException(nameof(internetChecker));

            _orientationСhangeAnimation.Construct();
        }

        public override void Enable()
        {
            EnableCanvasGroup(_canvasGroup);
            _orientationСhangeAnimation.StartAnimation();
            AnalyticsWinkService.SendChangeOrientationWindow();

            _waitPhoneRotateCoroutine = StartCoroutine(WaitRotatePhone());
        }

        public override void Disable()
        {
            if(Enabled)
                AnalyticsWinkService.SendPlayerRotateDevice();

            DisableCanvasGroup(_canvasGroup);

            if (_waitPhoneRotateCoroutine != null)
            {
                StopCoroutine(_waitPhoneRotateCoroutine);
                _waitPhoneRotateCoroutine = null;
            }

            _orientationСhangeAnimation.StopAnimation();
        }

        private IEnumerator WaitRotatePhone()
        {
            _gameOrientation.UnlockAutoOrientation();
#if UNITY_EDITOR && TEST_CHANGE_ORIENTATION
            yield return new WaitForSeconds(2);
#else
            while(_gameOrientation.ChangedToLandscape == false)
            {
                if(Input.acceleration.x < _gameOrientation.DeltaToLandscapeLeft)
                {
                    Screen.orientation = ScreenOrientation.LandscapeLeft;
                    _gameOrientation.LockPortraitOrientation();
                    Screen.orientation = ScreenOrientation.AutoRotation;
                }
                else if(Input.acceleration.x > _gameOrientation.DeltaToLandscapeRight)
                {
                    Screen.orientation = ScreenOrientation.LandscapeRight;
                    _gameOrientation.LockPortraitOrientation();
                    Screen.orientation = ScreenOrientation.AutoRotation;
                }

                yield return new WaitForSeconds(_gameOrientation.CheckTime);
            }
#endif
            _gameOrientation.LockPortraitOrientation();
            yield return new WaitWhile(() => _internetChecker.HasInternet);

            Disable();
        }
    }
}
