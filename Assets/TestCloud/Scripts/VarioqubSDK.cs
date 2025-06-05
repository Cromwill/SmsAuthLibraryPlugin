using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.Yandex.Varioqub;
using UnityEngine.UI;

public class VarioqubSDK : MonoBehaviour
{
    [SerializeField] private Button _btn;
    [SerializeField] private Text _log;
    [SerializeField] private Text _idPreset;
    [SerializeField] private Text _presetInfo1;
    [SerializeField] private Text _presetInfo2;
    [SerializeField] private InputField _inpFieldId;

    private IEnumerator Start()
    {
        string id = "appmetrica.4230628";
        _btn.onClick.AddListener(GetFlags);
        _log.text = $"AB Started! [{id}]";
        _btn.interactable = false;

        var settings = new VarioqubSettings(id);
        settings.Logs = true;
        settings.ThrottleInterval = 60;

        var defaultConfig = new Dictionary<string, object>(){
            {"version", "-1"}
        };

        Varioqub.InitVarioqubWithAppMetricaAdapter(settings);
        ActivateConfig();

        yield return RepeatFetch();
    }

    private IEnumerator RepeatFetch()
    {
        bool success = false;
        yield return new WaitForSeconds(2f);
        _log.text = "Fetch started!";

        while (success == false)
        {
            Varioqub.Fetch(
                onSuccessDelegate: () =>
                {
                    _btn.interactable = true;
                    _log.text = "Fetch successed!";
                    success = true;
                },
                onErrorDelegate: error =>
                {
                    _log.text = $"Error: {error}!";
                }
            );
            yield return new WaitForSeconds(5f);

            if (success == false)
                _log.text = "Fetch restarted!";
        }
    }

    private void GetFlags()
    {
        _idPreset.text = $"GetId: [{Varioqub.GetId()}]";
        _inpFieldId.text = _idPreset.text;
        var flags = Varioqub.GetString("version", "-1");
        var flag2s = Varioqub.GetString("text", "default text");
        _presetInfo1.text = $"[version:{flags}]";
        _presetInfo2.text = $"[text:{flag2s}]";
    }

    private void ActivateConfig()
    {
        Debug.Log($"Activate configs");
        Varioqub.ActivateConfig();
    }
}
