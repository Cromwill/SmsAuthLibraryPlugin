#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Agava.Wink.EditorScripts
{
    /// <summary>
    ///     Editor helpers to test the paywall.
    /// </summary>
    internal static class PaywallTestMenu
    {
        private const string Root = "Tools/Wink Paywall/";

        [MenuItem(Root + "Reset paywall state (new player)")]
        private static void ResetState()
        {
            PaywallService.ResetSavedState();
            UnityEngine.PlayerPrefs.DeleteKey("FirstTimeSave");
            UnityEngine.PlayerPrefs.Save();
            Debug.Log("#Paywall# test: paywall state and demo time reset");
        }

        [MenuItem(Root + "Reach paywall now")]
        private static void ReachNow() => PaywallService.Reach();

        [MenuItem(Root + "Reach paywall now", true)]
        private static bool ValidateReachNow() => Application.isPlaying && PaywallService.IsPending;

        [MenuItem(Root + "Paywall in 30 seconds of play")]
        private static void ShortTime()
        {
            PaywallService.Instance.SetPlaySecondsBeforePaywall(PaywallService.Instance.PlaySeconds + 30);
            Debug.Log("#Paywall# test: paywall in 30 seconds of play");
        }

        [MenuItem(Root + "Paywall in 30 seconds of play", true)]
        private static bool ValidateShortTime() => Application.isPlaying && PaywallService.IsPending;

        [MenuItem(Root + "Simulate sign in without subscription")]
        private static void SimulateSignIn()
        {
            WinkAccessManager manager = WinkAccessManager.Instance;
            typeof(WinkAccessManager).GetProperty(nameof(WinkAccessManager.Authenficated)).SetValue(manager, true);

            var field = typeof(WinkAccessManager).GetField(nameof(WinkAccessManager.SignInSuccessfully),
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            (field.GetValue(manager) as System.Action<bool, bool>)?.Invoke(false, false);
            Debug.Log("#Paywall# test: signed in without subscription (simulated, no server calls)");
        }

        [MenuItem(Root + "Simulate sign in without subscription", true)]
        private static bool ValidateSimulateSignIn() => Application.isPlaying && WinkAccessManager.Instance != null
            && UnityEngine.PlayerPrefs.HasKey(WinkAccessManager.Instance.PhoneNumber) && WinkAccessManager.Instance.Authenficated == false;

        [MenuItem(Root + "Simulate subscription")]
        private static void SimulateSubscription() => WinkAccessManager.Instance.TestEnableSubsription();

        [MenuItem(Root + "Simulate subscription", true)]
        private static bool ValidateSimulateSubscription() => Application.isPlaying && WinkAccessManager.Instance != null;

        [MenuItem(Root + "Log state")]
        private static void LogState()
        {
            PaywallService paywall = PaywallService.Instance;

            if (paywall == null)
            {
                Debug.Log($"#Paywall# state: not initialized, passed = {PaywallService.IsPassed}");
                return;
            }

            Debug.Log($"#Paywall# state: remote = {paywall.RemoteEnabled}, pending = {paywall.Pending}, passed = {PaywallService.IsPassed}, " +
                $"reached now = {paywall.ReachedInThisSession}, played = {paywall.PlaySeconds:0}s, left = {paywall.PlaySecondsLeft:0}s");
        }
    }
}
#endif
