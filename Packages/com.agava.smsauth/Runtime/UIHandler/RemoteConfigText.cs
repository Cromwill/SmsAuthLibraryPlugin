using System.Collections;
using System.Threading.Tasks;
using Agava.Wink;
using SmsAuthAPI.Program;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class RemoteConfigText : MonoBehaviour
{
    [SerializeField] private string _remoteConfigName;
    [SerializeField] private string _fallbackText;

    private TMP_Text _text;

    public bool Initialized { get; private set; } = false;

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => SmsAuthApi.Initialized);

        Task<string> task = RemoteConfig.StringRemoteConfig(_remoteConfigName, string.Empty);
        yield return new WaitUntil(() => task.IsCompleted);

        string result = task.Result;
        _text.text = string.IsNullOrEmpty(result) ? _fallbackText : result;

        Initialized = true;
    }
}
