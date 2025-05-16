using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.Yandex.Varioqub;
using UnityEngine.UI;

public class VarioqubSDK : MonoBehaviour
{
    [SerializeField] private Button _btn;

    private IEnumerator Start()
    {
        _btn.onClick.AddListener(GetFlags);
        Debug.Log("AB Started!");
        var settings = new VarioqubSettings("appmetrica.4230628");
#if UNITY_EDITOR
        settings.Logs = true;
        settings.ThrottleInterval = 2;
#elif !UNITY_EDITOR
        settings.ThrottleInterval = 60;
#endif

        var defaultConfig = new Dictionary<string, object>(){
            {"version", "-1"}
        };

        Varioqub.InitVarioqubWithAppMetricaAdapter(settings);
        ActivateConfig();
        //Varioqub.SetDefaults(defaultConfig);

        yield return RepeatFetch();
    }

    private IEnumerator RepeatFetch()
    {
        bool success = false;
        Debug.Log("Fetch started!");
        while (success == false)
        {
            Varioqub.Fetch(
                onSuccessDelegate: () =>
                {
                    Debug.Log("Fetch successed!");
                    success = true;
                },
                onErrorDelegate: error =>
                {
                    Debug.Log($"Error: {error}!");
                }
            );
            yield return new WaitForSeconds(3f);
            Debug.Log("Fetch restarted!");
        }
    }

    [ContextMenu("Get flags")]
    private void GetFlags()
    {
        Debug.Log($"id [{Varioqub.GetId()}]");

        var flags = Varioqub.GetString("version", "-1");
        Debug.Log($"flag  [version:{flags}]");
    }

    [ContextMenu("Activate Config")]
    private void ActivateConfig()
    {
        Debug.Log($"Activate configs");
        Varioqub.ActivateConfig();
    }
}
