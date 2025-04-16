using System;
using UnityEngine;

namespace Agava.Wink
{
    [Serializable]
    public class GameOrientation
    {
        private const GameScreenOrientation PluginOrientation = GameScreenOrientation.Portrait;

        [SerializeField] private GameScreenOrientation _appOrientation;

        private ScreenOrientation _screenOrientation;

        public bool NeedChangeOrientation => PluginOrientation != _appOrientation;
        public bool ChangedToLandscape => Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight;
        public float DeltaToLandscapeLeft { get; private set; } = -0.5f;
        public float DeltaToLandscapeRight { get; private set; } = 0.5f;
        public float CheckTime { get; private set; } = 0.1f;

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
