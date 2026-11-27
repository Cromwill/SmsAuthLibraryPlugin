using System;
using UnityEngine;

namespace KinDzaDza.RateUs
{
    public class RateUsPopupTimer : MonoBehaviour
    {
        public bool IsExpired { get; private set; }

        private float _remainingSeconds;
        private float _second;
        private bool _isStarted;

        public event Action TimerExpired;

        public static Func<bool> IsPaused;
        public static Func<bool> IsPopupPaused;
        public static Func<bool> IsAcceptWindowPaused;

        private void OnApplicationPause(bool pause)
        {
            if (_isStarted)
            {
                SaveRemainingTime();
            }
        }

        private void OnApplicationQuit()
        {
            if (_isStarted)
            {
                SaveRemainingTime();
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(this);
        }

        private void Update()
        {
            if (_isStarted == false || IsExpired || IsSmsAuthVpsPaused() || IsAdsAppViewPaused() || IsAdvertisementPaused())
            {
                return;
            }

            _remainingSeconds -= Time.deltaTime;
            _second -= Time.deltaTime;

            if (_second <= 0f)
            {
                SaveRemainingTime(needSave: false);
                _second = 1f;
            }

            if (_remainingSeconds > 0f)
            {
                return;
            }

            Stop();
            TimerExpired?.Invoke();
        }

        public void Construct(float triggerTimeSeconds)
        {
            if (PlayerPrefs.HasKey(Constants.RateUsRemainingTimeKey))
            {
                _remainingSeconds = PlayerPrefs.GetFloat(Constants.RateUsRemainingTimeKey);
            }
            else
            {
                _remainingSeconds = Mathf.Max(0f, triggerTimeSeconds);
                SaveRemainingTime();
            }

            if (_remainingSeconds <= 0f)
            {
                IsExpired = true;
            }

            _isStarted = true;
        }

        public void Stop()
        {
            _remainingSeconds = 0f;
            IsExpired = true;

            _isStarted = false;
            SaveRemainingTime();
        }

        private void SaveRemainingTime(bool needSave = true)
        {
#if UNITY_EDITOR
            Debug.Log($"[{nameof(RateUsPopupTimer)}]: remaining time: {_remainingSeconds}");
#endif
            PlayerPrefs.SetFloat(Constants.RateUsRemainingTimeKey, Mathf.Max(0f, _remainingSeconds));

            if (needSave)
                PlayerPrefs.Save();
        }

        private bool IsSmsAuthVpsPaused()
        {
            return IsPaused != null && IsPaused.Invoke();
        }

        private bool IsAdsAppViewPaused()
        {
            return IsPopupPaused != null && IsPopupPaused.Invoke();
        }

        private bool IsAdvertisementPaused()
        {
            return IsAcceptWindowPaused != null && IsAcceptWindowPaused.Invoke();
        }
    }
}
