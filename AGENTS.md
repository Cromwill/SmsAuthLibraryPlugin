# AGENTS.md

## Notes
- Paywall: Packages/com.agava.smsauth/Runtime/Services/Paywall (PaywallService held by Boot, PaywallTrigger component). Enabled by remote config `paywall_enabled`; play time before paywall is `_playSecondsBeforePaywall` on Boot (600 s). Games trigger it with `PaywallService.Reach()`.
- Editor test menu: Tools > Wink Paywall (reset state, reach now, paywall in 30 s, simulate sign in/subscription, log state). Logs are prefixed `#Paywall#`.
- Editor PlayerPrefs live in registry HKCU\Software\Unity\UnityEditor\Agava\SmsAuthLibraryPlugin.
- Interstitials don't show in the Editor, only in builds; WebView "not supported on this platform" error in Editor is expected.
- Test menu also has 'Forget authorization (delete tokens)' and 'Simulate no internet window'. After editing Editor scripts press Ctrl+R in Unity (it did not auto-recompile).
- Paywall mode is set on Boot (PaywallService._mode): PlayTime (after _playSecondsBeforePaywall) or Trigger (only PaywallService.Reach()/PaywallTrigger). If the demo time ends before the paywall (WinkSignInHandlerUI.DemoCompleted), the paywall counts as reached and the demo-expired windows are shown. Test menu: 'Mode: trigger only', 'Demo time left = 10 s'. Editing files with PowerShell Get-Content/Set-Content breaks Cyrillic and BOM: use edit_file.
