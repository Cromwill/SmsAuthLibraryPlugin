using System;
using UnityEngine;

namespace Agava.Wink
{
    [Serializable]
    public class GameOrientation
    {
        private const GameScreenOrientation PluginOrientation = GameScreenOrientation.Portrait;
        private float RotateDelta = 0.5f;

        [SerializeField] private GameScreenOrientation _appOrientation;

        private ScreenOrientation _screenOrientation;

        public bool NeedChangeOrientation => PluginOrientation != _appOrientation;
        public bool ChangedToLandscape => Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight;
        public bool IsLandscapeRight => Input.acceleration.x > RotateDelta;
        public bool IsLandscapeLeft => Input.acceleration.x < -RotateDelta;
        public bool ChangedToPortrait => Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown;
        public bool IsPortrait => Input.acceleration.x > RotateDelta;
        public bool IsPortraitUpsideDown => Input.acceleration.x < -RotateDelta;
        public float CheckTime { get; private set; } = 0.2f;

        public void UnlockAutoOrientation()
        {
            Screen.orientation = ScreenOrientation.AutoRotation;
            Screen.autorotateToLandscapeLeft = Screen.autorotateToLandscapeRight = true;
            Screen.autorotateToPortrait = Screen.autorotateToPortraitUpsideDown = true;
        }

        public void LockPortraitOrientation() => Screen.autorotateToPortrait = Screen.autorotateToPortraitUpsideDown = false;
        public void LockLandscapeOrientation() => Screen.autorotateToLandscapeLeft = Screen.autorotateToLandscapeRight = false;

        public void SetLandscapeOrientationPosibility()
        {
            Screen.orientation = ScreenOrientation.AutoRotation;
            Screen.autorotateToLandscapeLeft = Screen.autorotateToLandscapeRight = true;
            Screen.autorotateToPortrait = Screen.autorotateToPortraitUpsideDown = false;
        }

        public void SetPortraitOrientation()
        {
            Screen.orientation = ScreenOrientation.Portrait;
            Screen.autorotateToPortrait = Screen.autorotateToPortraitUpsideDown = true;
            Screen.autorotateToLandscapeLeft = Screen.autorotateToLandscapeRight = false;
            Screen.orientation = ScreenOrientation.AutoRotation;
        }

        public void SetLandscapeOrientation()
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            Screen.autorotateToLandscapeLeft = Screen.autorotateToLandscapeRight = true;
            Screen.autorotateToPortrait = Screen.autorotateToPortraitUpsideDown = false;
            Screen.orientation = ScreenOrientation.AutoRotation;
        }

        public void SaveGameOrientation() => _screenOrientation = Screen.orientation;
        public void SetSavedOrientation() => Screen.orientation = _screenOrientation;

        private enum GameScreenOrientation
        {
            Auto,
            Portrait,
            Landscape,
        }
    }
}
