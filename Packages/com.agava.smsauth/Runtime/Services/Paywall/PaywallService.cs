using System;
using UnityEngine;
using SmsAuthAPI.DTO;
using System.Collections;
using UnityEngine.Scripting;
using System.Threading.Tasks;
using KinDzaDzaGames.AdvertisementPlugin;

namespace Agava.Wink
{
    /// <summary>
    ///     Paywall: a point in the game (a trigger or N minutes of play) after which the Wink plugin is shown.
    ///     Until it is reached the Wink start windows are skipped, ads are paused and the ad offer is hidden.
    ///     Works only when the Wink plugin is available and the remote config "paywall_enabled" is true.
    /// </summary>
    public enum PaywallMode
    {
        /// <summary> The paywall appears after N seconds of play. </summary>
        PlayTime,
        /// <summary> The paywall is a point in the game: PaywallService.Reach() or PaywallTrigger. </summary>
        Trigger,
    }

    [Serializable, Preserve]
    public class PaywallService : IInterstitialBlocker, IBannerBlocker
    {
        private const string RemoteEnabledKey = "paywall_enabled";
        private const string PassedKey = "WinkPaywallPassed";
        private const string PlayTimeKey = "WinkPaywallPlayTime";
        private const float SavePlayTimeInterval = 5f;
        private const float MaxFrameDelta = 0.5f;
        private const float ReopenWindowDelay = 1f;

        [Tooltip("PlayTime: the paywall appears after the play time below. Trigger: only when the game calls PaywallService.Reach() (or a PaywallTrigger is enabled)")]
        [SerializeField] private PaywallMode _mode = PaywallMode.PlayTime;
        [Tooltip("PlayTime mode: play time before the paywall (seconds, saved between launches)")]
        [SerializeField, Min(0)] private float _playSecondsBeforePaywall = 600f;

        private MonoBehaviour _coroutineRunner;
        private WinkSignInHandlerUI _winkSignInHandlerUI;
        private WinkAccessManager _winkAccessManager;
        private Action _reloadWithCloudSaves;
        private Coroutine _passing;
        private float _playSeconds;
        private float _lastSavedPlaySeconds;
        private bool _started;
        private bool _focus = true;

        public static PaywallService Instance { get; private set; }

        /// <summary> Paywall is on and the player hasn't reached it yet. </summary>
        public static bool IsPending => Instance != null && Instance.Pending;

        /// <summary> The player has reached the paywall (at any launch). </summary>
        public static bool IsPassed => PlayerPrefs.GetInt(PassedKey, 0) == 1;

        /// <summary> Called when the paywall is reached and the Wink window is shown. </summary>
        public static event Action Reached;

        /// <summary> Called when the player has gone through the paywall windows and is back in the game. </summary>
        public static event Action Completed;

        public PaywallMode Mode => _mode;
        public bool RemoteEnabled { get; private set; }
        public bool Pending { get; private set; }
        public bool ReachedInThisSession { get; private set; }
        public float PlaySeconds => _playSeconds;
        public float PlaySecondsLeft => Mathf.Max(0, _playSecondsBeforePaywall - _playSeconds);

        public bool InterstitialDisplayBlocked => Pending;
        public bool BannerDisplayBlocked => Pending;

        public void RemoveRestriction() { }

        /// <summary>
        ///     Reach the paywall from a point in the game. Does nothing when the paywall isn't pending.
        /// </summary>
        public static void Reach()
        {
            if (Instance != null)
                Instance.ReachPaywall();
        }

        /// <summary>
        ///     Decides at boot whether the paywall mode is on. Call after the Wink plugin is found available.
        /// </summary>
        internal IEnumerator Initialize(MonoBehaviour coroutineRunner, WinkSignInHandlerUI winkSignInHandlerUI, WinkAccessManager winkAccessManager, Action reloadWithCloudSaves)
        {
            Instance = this;
            _coroutineRunner = coroutineRunner;
            _winkSignInHandlerUI = winkSignInHandlerUI;
            _winkAccessManager = winkAccessManager;
            _reloadWithCloudSaves = reloadWithCloudSaves;

            Task<bool> remoteTask = RemoteConfig.BoolRemoteConfig(RemoteEnabledKey, false);
            yield return new WaitUntil(() => remoteTask.IsCompleted);
            RemoteEnabled = remoteTask.IsCompletedSuccessfully && remoteTask.Result;

            if (RemoteEnabled == false)
            {
                Log("disabled by remote config");
                yield break;
            }

            if (IsPassed)
            {
                Log("already passed, plugins start as usual");
                yield break;
            }

            if (PlayerPrefs.HasKey(TokenLifeHelper.Tokens))
            {
                SetPassed();
                Log("player is already authorized, paywall skipped");
                yield break;
            }

            if (DemoTimer.SavedTimeExpired())
            {
                SetPassed();
                Log("demo time is already over, paywall skipped");
                yield break;
            }

            _playSeconds = _lastSavedPlaySeconds = PlayerPrefs.GetFloat(PlayTimeKey, 0);
            Pending = true;
            Log(_mode == PaywallMode.PlayTime
                ? $"pending (play time mode). Played {_playSeconds:0}s of {_playSecondsBeforePaywall:0}s"
                : "pending (trigger mode). Waiting for PaywallService.Reach()");
        }

        /// <summary>
        ///     Starts tracking the play time once the game scene is loading.
        /// </summary>
        internal void Start()
        {
            if (_started || _winkSignInHandlerUI == null)
                return;

            _started = true;
            _winkSignInHandlerUI.DemoCompleted += OnDemoCompleted;
            _winkAccessManager.SignInSuccessfully += OnSignInSuccessfully;

            if (Pending)
            {
                AdvertisementController.Instance?.AddInterstitialBlocker(this);
                AdvertisementController.Instance?.SuspendDisplayBanner(this);
            }
        }

        internal void Dispose()
        {
            if (_started == false)
                return;

            SavePlayTime();
            _winkSignInHandlerUI.DemoCompleted -= OnDemoCompleted;
            _winkAccessManager.SignInSuccessfully -= OnSignInSuccessfully;
        }

        internal void OnAppFocus(bool focus)
        {
            _focus = focus;

            if (focus == false)
                SavePlayTime();
        }

        internal void Update()
        {
            if (_started == false || Pending == false || _mode != PaywallMode.PlayTime || _focus == false)
                return;

            if (_winkSignInHandlerUI.IsAnyWindowEnabled)
                return;

            _playSeconds += Mathf.Min(Time.unscaledDeltaTime, MaxFrameDelta);

            if (_playSeconds - _lastSavedPlaySeconds >= SavePlayTimeInterval)
                SavePlayTime();

            if (_playSeconds >= _playSecondsBeforePaywall)
                ReachPaywall();
        }

        /// <summary>
        ///     The "sign in" button of the game menu was pressed while the paywall is pending.
        /// </summary>
        internal void ReachFromMenu() => ReachPaywall();

        private void ReachPaywall()
        {
            if (Pending == false || _started == false)
                return;

            Log("reached");
            Pending = false;
            ReachedInThisSession = true;
            SetPassed();
            SavePlayTime();

            _winkSignInHandlerUI.OpenPaywall(firstOpen: true);
            Reached?.Invoke();

            _passing = _coroutineRunner.StartCoroutine(Passing(waitAuthorization: true));
        }

        private void OnDemoCompleted()
        {
            if (Pending == false)
                return;

            // Demo time ran out before the paywall: the demo windows of the plugin take its place.
            Log("demo time is over before the paywall, paywall counted as passed");
            Pending = false;
            ReachedInThisSession = true;
            SetPassed();
            SavePlayTime();

            if (_passing == null)
                _passing = _coroutineRunner.StartCoroutine(Passing(waitAuthorization: false));
        }

        private IEnumerator Passing(bool waitAuthorization)
        {
            if (waitAuthorization)
            {
                float noWindowTime = 0;

                // The player can't go back to the game until he is authorized.
                while (_winkAccessManager.Authenficated == false)
                {
                    if (_winkSignInHandlerUI.IsAnyWindowEnabled)
                    {
                        noWindowTime = 0;
                    }
                    else
                    {
                        noWindowTime += Time.unscaledDeltaTime;

                        if (noWindowTime >= ReopenWindowDelay)
                        {
                            noWindowTime = 0;
                            _winkSignInHandlerUI.OpenPaywall(firstOpen: false);
                        }
                    }

                    yield return null;
                }
            }

            yield return new WaitWhile(() => _winkSignInHandlerUI.IsAnyWindowEnabled);

            if (waitAuthorization)
                _winkSignInHandlerUI.ClosePaywall();

            if (_winkAccessManager.HasAccess == false && _winkAccessManager.HasTempAccess == false)
                AdvertisementController.Instance?.StartInterstitialTimer();

            Log("completed, plugins work as usual");
            _passing = null;
            Completed?.Invoke();
        }

        private void OnSignInSuccessfully(bool hasAccess, bool hasTempAccess)
        {
            // A full subscription loads cloud saves through AuthorizationSuccessfully.
            // A temporary one has to do it here, as it is done at boot.
            if (ReachedInThisSession == false || hasAccess || hasTempAccess == false)
                return;

            Log("authorized with a temporary subscription, loading cloud saves");
            _reloadWithCloudSaves?.Invoke();
        }

        private void SetPassed()
        {
            PlayerPrefs.SetInt(PassedKey, 1);
            PlayerPrefs.Save();
        }

        private void SavePlayTime()
        {
            if (Pending == false && ReachedInThisSession == false)
                return;

            _lastSavedPlaySeconds = _playSeconds;
            PlayerPrefs.SetFloat(PlayTimeKey, _playSeconds);
        }

        private static void Log(string message) => Debug.Log("#Paywall# " + message);

#if UNITY_EDITOR || TEST
        /// <summary> Test: forget the paywall state on this device. </summary>
        public static void ResetSavedState()
        {
            PlayerPrefs.DeleteKey(PassedKey);
            PlayerPrefs.DeleteKey(PlayTimeKey);
            PlayerPrefs.Save();
        }

        /// <summary> Test: change the play time before the paywall. </summary>
        public void SetPlaySecondsBeforePaywall(float seconds) => _playSecondsBeforePaywall = Mathf.Max(0, seconds);

        /// <summary> Test: switch between the play time and the trigger modes. </summary>
        public void SetMode(PaywallMode mode) => _mode = mode;
#endif
    }
}
