using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace Agava.Wink
{
    [Serializable, Preserve]
    public class GameOrientation
    {
        private const GameScreenOrientation PluginOrientation = GameScreenOrientation.Portrait;

        [SerializeField] private GameScreenOrientation _appOrientation;

        private ScreenOrientation _screenOrientation;

        public GameScreenOrientation AppOrientation => _appOrientation;
        public bool NeedChangeOrientation => PluginOrientation != _appOrientation;
        public bool ChangedToLandscape => Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight;
        public float DeltaToLandscapeLeft { get; private set; } = -0.5f;
        public float DeltaToLandscapeRight { get; private set; } = 0.5f;
        public float CheckTime { get; private set; } = 0.1f;

        public void UnlockAutoOrientation()
        {
#if UNITY_EDITOR
            Debug.Log($"<color=yellow>ORIENTATION CHANGER: unlock auto orientation</color>");
#endif
            Screen.orientation = ScreenOrientation.AutoRotation;
            Screen.autorotateToLandscapeLeft = Screen.autorotateToLandscapeRight = true;
            Screen.autorotateToPortrait = Screen.autorotateToPortraitUpsideDown = true;
        }

        public void LockPortraitOrientation()
        {
#if UNITY_EDITOR
            Debug.Log($"<color=yellow>ORIENTATION CHANGER: lock portrait orientation</color>");
#endif
            Screen.autorotateToPortrait = Screen.autorotateToPortraitUpsideDown = false;
        }

        public void SetLandscapeOrientationPosibility()
        {
#if UNITY_EDITOR
            Debug.Log($"<color=yellow>ORIENTATION CHANGER: set landscape orientation posibility</color>");
#endif
            Screen.orientation = ScreenOrientation.AutoRotation;
            Screen.autorotateToLandscapeLeft = Screen.autorotateToLandscapeRight = true;
            Screen.autorotateToPortrait = Screen.autorotateToPortraitUpsideDown = false;
        }

        public void SetPortraitOrientation()
        {
#if UNITY_EDITOR
            Debug.Log($"<color=yellow>ORIENTATION CHANGER: try set portrait orientation</color>");
#endif
            Screen.orientation = ScreenOrientation.Portrait;
            Screen.autorotateToPortrait = Screen.autorotateToPortraitUpsideDown = true;
            Screen.autorotateToLandscapeLeft = Screen.autorotateToLandscapeRight = false;
            Screen.orientation = ScreenOrientation.AutoRotation;
        }

        public void SetLandscapeOrientation()
        {
#if UNITY_EDITOR
            Debug.Log($"<color=yellow>ORIENTATION CHANGER: try set landscape orientation</color>");
#endif
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            Screen.autorotateToLandscapeLeft = Screen.autorotateToLandscapeRight = true;
            Screen.autorotateToPortrait = Screen.autorotateToPortraitUpsideDown = false;
            Screen.orientation = ScreenOrientation.AutoRotation;
        }

        public void SaveGameOrientation()
        {
#if UNITY_EDITOR
            Debug.Log($"<color=yellow>ORIENTATION CHANGER: save game orientation = {Screen.orientation}</color>");
#endif
            _screenOrientation = Screen.orientation;
        }

        public void SetSavedOrientation()
        {
#if UNITY_EDITOR
            Debug.Log($"<color=yellow>ORIENTATION CHANGER: set saved orientation = {_screenOrientation}</color>");
#endif
            Screen.orientation = _screenOrientation;
        }
    }
}
