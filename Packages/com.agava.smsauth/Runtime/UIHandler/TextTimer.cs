using System;
using System.Collections;
using System.Threading.Tasks;
using SmsAuthAPI.Program;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Scripting;

namespace Agava.Wink
{
    [Preserve]
    internal class TextTimer : MonoBehaviour
    {
        private const string SmsDelay = "sms-delay-seconds";
        private const string CodeLifespan = "code-lifespan-seconds";
        private const string SavedTime = nameof(SavedTime);
        private const int SmsDelayDefaultTime = 60;
        private const int CodeLifespanDefaultTime = 600;
        private const int AdditiveTime = 10;

        [SerializeField] private TextPlaceholder _timePlaceholder;

        private int _smsDelaySeconds;
        private int _codeLifespan;

        private int _seconds = 0;
        private Coroutine _coroutine;

        public event Action TimerExpired;
        public bool Expired = true;

        private IEnumerator Start()
        {
            if (_seconds <= 0)
                _seconds = SmsDelayDefaultTime;

            yield return new WaitUntil(() => SmsAuthApi.Initialized);

            Task task = SetRemoteConfigs();
            yield return new WaitUntil(() => task.IsCompleted);

            _seconds = _codeLifespan;
        }

        public void SetSmsDelayConfig()
        {
            UnityEngine.PlayerPrefs.DeleteKey(SavedTime);
            _seconds = _smsDelaySeconds;
        }

        public void SetCodeLifespanConfig()
        {
            UnityEngine.PlayerPrefs.DeleteKey(SavedTime);
            _seconds = _codeLifespan;
        }

        internal void Enable()
        {
            _timePlaceholder.gameObject.SetActive(true);
            _coroutine ??= StartCoroutine(Ticking());

            IEnumerator Ticking()
            {
                int sec = _seconds;

                if (UnityEngine.PlayerPrefs.HasKey(SavedTime))
                    sec = UnityEngine.PlayerPrefs.GetInt(SavedTime);

                Expired = false;
                var tick = new WaitForSecondsRealtime(1);

                while (sec > 0)
                {
                    sec--;
                    _timePlaceholder.ReplaceValue(TimeString(sec));
                    UnityEngine.PlayerPrefs.SetInt(SavedTime, sec);

                    yield return tick;
                }

                if (sec <= 0)
                {
                    TimerExpired?.Invoke();
                    Expired = true;
                    UnityEngine.PlayerPrefs.DeleteKey(SavedTime);
                    _timePlaceholder.gameObject.SetActive(false);
                }
            }
        }

        internal void Disable()
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
            _smsDelaySeconds = await RemoteConfig(SmsDelay, SmsDelayDefaultTime);
            _codeLifespan = await RemoteConfig(CodeLifespan, CodeLifespanDefaultTime);
        }

        private async Task<int> RemoteConfig(string configName, int defaultTime)
        {
            var response = await SmsAuthApi.GetRemoteConfig(configName);

            if (response.statusCode == UnityWebRequest.Result.Success)
            {
                if (string.IsNullOrEmpty(response.body) == false)
                {
                    return ParseConfig(response.body, defaultTime);
                }
                else
                {
                    Debug.LogError($"Fail to recieve remote config '{configName}': value is NULL");
                }
            }
            else
            {
                Debug.LogError($"Fail to recieve remote config '{configName}': BAD REQUEST");
            }

            return defaultTime;
        }

        private int ParseConfig(string timeStr, int defaultValue)
        {
            bool success = int.TryParse(timeStr, out int time);
            return success ? time + AdditiveTime : defaultValue;
        }

        private string TimeString(int seconds)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
            return $"{timeSpan.Minutes}:{timeSpan.Seconds:00}";
        }
    }
}
