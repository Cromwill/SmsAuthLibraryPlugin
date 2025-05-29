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
        private UnlinkWindowPresenter _unlinkWindowPresenter;
        private InputWindowPresenter _inputWindowPresenter;

        public void Construct(GameOrientation gameOrientation, IInternetChecker internetChecker, UnlinkWindowPresenter unlinkWindowPresenter, InputWindowPresenter inputWindowPresenter)
        {
            _gameOrientation = gameOrientation ?? throw new ArgumentNullException(nameof(gameOrientation));
            _internetChecker = internetChecker ?? throw new ArgumentNullException(nameof(internetChecker));
            _unlinkWindowPresenter = unlinkWindowPresenter ?? throw new ArgumentNullException(nameof(unlinkWindowPresenter));
            _inputWindowPresenter = inputWindowPresenter ?? throw new ArgumentNullException(nameof(inputWindowPresenter));

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
            yield return new WaitUntil(() => _unlinkWindowPresenter.Enabled == false && _inputWindowPresenter.Enabled == false);

            _gameOrientation.UnlockAutoOrientation();
#if UNITY_EDITOR && TEST_CHANGE_ORIENTATION
            yield return new WaitForSeconds(2);
#else
            while (_gameOrientation.ChangedToPortrait == false)
            {
                if (_gameOrientation.IsPortrait)
                {
                    Screen.orientation = ScreenOrientation.Portrait;
                    _gameOrientation.LockLandscapeOrientation();
                    Screen.orientation = ScreenOrientation.AutoRotation;
                }
                else if (_gameOrientation.IsPortraitUpsideDown)
                {
                    Screen.orientation = ScreenOrientation.PortraitUpsideDown;
                    _gameOrientation.LockLandscapeOrientation();
                    Screen.orientation = ScreenOrientation.AutoRotation;
                }

                if (_gameOrientation.ChangedToPortrait == false)
                    yield return new WaitForSeconds(_gameOrientation.CheckTime);
            }
#endif
            _gameOrientation.LockLandscapeOrientation();

            yield return new WaitWhile(() => _internetChecker.HasInternet);

            Disable();
        }
    }
}
