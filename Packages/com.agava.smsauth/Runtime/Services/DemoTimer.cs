using System;
using UnityEngine;
using UnityEngine.Scripting;
using SmsAuthAPI.Program;

namespace Agava.Wink
{
    /// <summary>
    ///     Demo timer Handler. Block app after expired allowed time.
    /// </summary>
    [Serializable, Preserve]
    internal class DemoTimer
    {
        private const string FirstTimeSave = nameof(FirstTimeSave);

        [Min(0)]
        [SerializeField] private int _defaultTimerSeconds = 1800;

        private IWinkAccessManager _winkAccessManager;
        private IWinkSignInHandlerUI _winkSignInHandlerUI;

        private TimeSpan _savedDemoTime;
        private bool _focus = true;
        private bool _stoped;
        private float _second;
        private float _delay = 5f;
        private float _remoteTempCfgSeconds;

        public event Action TimerExpired;
        public event Action FirstChecked;

        public bool Expired { get; private set; }

        internal void Construct(IWinkAccessManager winkAccessManager, int remoteCfgSeconds, int remoteTempCfgSeconds, IWinkSignInHandlerUI winkSignInHandlerUI)
        {
            _winkSignInHandlerUI = winkSignInHandlerUI;
            _winkAccessManager = winkAccessManager;

            if (remoteCfgSeconds <= 0)
                remoteCfgSeconds = _defaultTimerSeconds;

            _remoteTempCfgSeconds = remoteTempCfgSeconds <= 0 ? _defaultTimerSeconds : remoteTempCfgSeconds;

            if (UnityEngine.PlayerPrefs.HasKey(FirstTimeSave) == false)
            {
                _savedDemoTime = TimeSpan.FromSeconds(remoteCfgSeconds);
                UnityEngine.PlayerPrefs.SetString(FirstTimeSave, _savedDemoTime.ToString());
            }
            else
            {
                string time = UnityEngine.PlayerPrefs.GetString(FirstTimeSave);
                _savedDemoTime = TimeSpan.Parse(time);
            }

            _winkAccessManager.AuthorizationSuccessfully += Stop;
            _winkAccessManager.AccountDeleted += Start;
        }

        internal void Dispose()
        {
            if (_winkAccessManager != null)
            {
                _winkAccessManager.AuthorizationSuccessfully -= Stop;
                _winkAccessManager.AccountDeleted -= Start;
            }
        }

        internal void OnAppFocus(bool focus) => _focus = focus;

        internal void Start()
        {
            if (WinkAccessManager.Instance.HasAccess)
                return;

            Expired = false;
            _stoped = false;

            Debug.Log("Demo activated");
        }

        internal void Stop()
        {
            Expired = false;
            _stoped = true;

            Debug.Log("Demo Stoped");
        }

        internal void Update()
        {
            if (_focus == false || _stoped)
                return;

            if (_delay > 0)
            {
                _delay -= Time.unscaledDeltaTime;
                return;
            }

            if (_winkSignInHandlerUI == null || _winkSignInHandlerUI.IsAnyWindowEnabled || Expired || SmsAuthApi.Initialized == false)
                return;

            _second -= Time.unscaledDeltaTime;

            if (_second <= 0)
            {
                if (_savedDemoTime <= TimeSpan.Zero && WinkAccessManager.Instance.HasAccess == false)
                {
                    TimerExpired?.Invoke();
                    Expired = true;
                }

                _savedDemoTime = _savedDemoTime.Subtract(TimeSpan.FromSeconds(1f));
                UnityEngine.PlayerPrefs.SetString(FirstTimeSave, _savedDemoTime.ToString());
                _second = 1;
            }
        }

        internal void AddTempSubsDemoTime() => AddDemoTime(_remoteTempCfgSeconds);

        internal void AddDemoTime(float time)
        {
            _savedDemoTime += TimeSpan.FromSeconds(time);
            Expired = false;
            UnityEngine.PlayerPrefs.SetString(FirstTimeSave, _savedDemoTime.ToString());
        }

        internal void CheckOutTime()
        {
            if (_savedDemoTime <= TimeSpan.Zero && WinkAccessManager.Instance.HasAccess == false)
            {
                Expired = true;
                UnityEngine.PlayerPrefs.SetString(FirstTimeSave, _savedDemoTime.ToString());
            }

            FirstChecked?.Invoke();
        }
    }
}
