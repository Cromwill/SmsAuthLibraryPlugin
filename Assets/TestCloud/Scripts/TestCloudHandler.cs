using AdsAppView.Program;
using Agava.Wink;
using Newtonsoft.Json;
using SmsAuthAPI.DTO;
using SmsAuthAPI.Program;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TestCloudHandler : MonoBehaviour
{
    [SerializeField] private Button _savePresf;
    [SerializeField] private Button _loadPrefs;
    [SerializeField] private TMP_InputField _input;
    [SerializeField] private Button _checkDevices;
    [SerializeField] private Image _indicator;
    [Header("Popup test")]
    [SerializeField] private Button _showPopup;
    //[Header("Analytics test")]
    //[SerializeField] private Button _sendAccTrue;
    //[SerializeField] private Button _sendAccFalse;
    [Header("Lang buttons")]
    [SerializeField] private Button _ruButton;
    [SerializeField] private Button _enButton;

    private IEnumerator Start()
    {
        yield return new WaitWhile(() => WinkSignInHandlerUI.Instance == null);

        _savePresf.onClick.AddListener(OnSavePrefsClicked);
        _loadPrefs.onClick.AddListener(OnLoadPrefsClicked);
        _checkDevices.onClick.AddListener(ShowDevices);

        //_sendAccTrue.onClick.AddListener(SendAccTrue);
        //_sendAccFalse.onClick.AddListener(SendAccFalse);

        _ruButton.onClick.AddListener(OnRuClicked);
        _enButton.onClick.AddListener(OnEnClicked);

        _showPopup.onClick.AddListener(OnShowPopupClicked);
    }

    //private void SendAccTrue() => AnalyticsWinkService.SendHasActiveAccountUser(true);
    //private void SendAccFalse() => AnalyticsWinkService.SendHasActiveAccountUser(false);
    private void OnEnClicked() => WinkLocalization.Instance.SetCurrentLang(SystemLanguage.English);
    private void OnRuClicked() => WinkLocalization.Instance.SetCurrentLang(SystemLanguage.Russian);

    private void OnShowPopupClicked()
    {
        if (AdsAppView.Program.PopupManager.Instance == null)
            Debug.LogError("PopupManager.Instance == null");
        else
            AdsAppView.Program.PopupManager.ShowPopupPayedApp();
    }

    private void Update()
    {
        if (WinkAccessManager.Instance == null) return;

        if (WinkAccessManager.Instance.HasAccess)
            _indicator.color = Color.green;
        else if (WinkAccessManager.Instance.Authenficated)
            _indicator.color = Color.yellow;
        else
            _indicator.color = Color.red;
    }

    private void OnSavePrefsClicked()
    {
        Debug.Log("Wink: " + WinkAccessManager.Instance.HasAccess);

        if (WinkAccessManager.Instance.HasAccess == false)
            throw new System.Exception("Wink not authorizated!");

        SmsAuthAPI.Utility.PlayerPrefs.SetString("key", _input.text);
        SmsAuthAPI.Utility.PlayerPrefs.Save();
    }

    private void OnLoadPrefsClicked()
    {
        Debug.Log("Wink: " + WinkAccessManager.Instance.HasAccess);

        if (WinkAccessManager.Instance.HasAccess == false)
            throw new System.Exception("Wink not authorizated!");

        var data = SmsAuthAPI.Utility.PlayerPrefs.GetString("key");

        if (string.IsNullOrEmpty(data))
            Debug.Log($"<color=red>Load fail</color>: data empty {data}");
        else
            Debug.Log("Loaded: " + data);
    }

    private async void ShowDevices()
    {
        Debug.Log("Wink access: " + WinkAccessManager.Instance.HasAccess);

        if (WinkAccessManager.Instance.Authenficated == false)
            throw new System.Exception("Wink not authenticated!");

        Tokens tokens = TokenLifeHelper.GetTokens();
        var response = await SmsAuthApi.GetDevices(tokens.access, Application.identifier);

        if (response.statusCode != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error");
        }
        else
        {
            List<string> devices = JsonConvert.DeserializeObject<List<string>>(response.body);

            foreach (var device in devices)
            {
                Debug.Log(device);
            }
        }
    }

    [ContextMenu("Set pause on")]
    private void SetPauseOn()
    {
        PopupManager.SetPause(true);
    }

    [ContextMenu("Set pause off")]
    private void SetPauseOff()
    {
        PopupManager.SetPause(false);
    }
}

public class TestData
{
    public string Text;
}
