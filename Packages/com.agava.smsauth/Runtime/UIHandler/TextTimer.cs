using System;
using System.Collections;
using System.Threading.Tasks;
using SmsAuthAPI.Program;
using UnityEngine;
using UnityEngine.Scripting;

namespace Agava.Wink
{
    [Preserve]
    internal class TextTimer : MonoBehaviour
    {
        private const string SmsDelay = "sms-delay-seconds";
        private const string ExpirationTime = nameof(ExpirationTime);
        private const int SmsDelayDefaultTime = 60;

        [SerializeField] private TextPlaceholder _timePlaceholder;

        private int _smsDelaySeconds;
        private Coroutine _coroutine;

        public event Action TimerExpired;

        public bool Expired { get; private set; } = false;
        public bool Initialized { get; private set; } = false;
        public DateTime Now => DateTime.Now;

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => SmsAuthApi.Initialized);

            Task task = SetRemoteConfigs();
            yield return new WaitUntil(() => task.IsCompleted);

            Initialized = true;
        }

        internal void StartTimer()
        {
            DateTime expirationDate = Now;
            int startSeconds = 0;

            if (UnityEngine.PlayerPrefs.HasKey(ExpirationTime))
            {
                if (DateTime.TryParse(UnityEngine.PlayerPrefs.GetString(ExpirationTime), out expirationDate))
                    startSeconds = SubtractSeconds(expirationDate);
            }

            if (startSeconds <= 0)
            {
                expirationDate = Now.AddSeconds(_smsDelaySeconds);
                startSeconds = _smsDelaySeconds;
                UnityEngine.PlayerPrefs.SetString(ExpirationTime, expirationDate.ToString());
            }

            //Debug.Log($"seconds: {seconds}");

            _timePlaceholder.gameObject.SetActive(true);
            _coroutine ??= StartCoroutine(Ticking(startSeconds));

            IEnumerator Ticking(int seconds)
            {
                while (seconds > 0)
                {
                    seconds = SubtractSeconds(expirationDate);
                    _timePlaceholder.ReplaceValue(TimeString(seconds));
                    Expired = false;

                    yield return new WaitForEndOfFrame();
                }

                if (seconds <= 0)
                {
                    TimerExpired?.Invoke();
                    Expired = true;
                    ResetTimer();
                }
            }
        }

        internal void ResetTimer()
        {
            Expired = false;
            Disable();
            UnityEngine.PlayerPrefs.DeleteKey(ExpirationTime);
        }

        public void Disable()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
                _timePlaceholder.gameObject.SetActive(false);
            }
        }

        private async Task SetRemoteConfigs()
        {
            _smsDelaySeconds = await RemoteConfig.IntRemoteConfig(SmsDelay, SmsDelayDefaultTime);
        }

        private string TimeString(int seconds)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
            return $"{timeSpan.Minutes}:{timeSpan.Seconds:00}";
        }

        private int SubtractSeconds(DateTime expirationDate) => expirationDate.Subtract(Now).Seconds;
    }
}
