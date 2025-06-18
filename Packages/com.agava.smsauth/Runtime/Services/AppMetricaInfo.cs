using UnityEngine;

namespace Agava.Wink
{
    [CreateAssetMenu(fileName = "AppMetricaInfo", menuName = "Create AppMetricaInfo/SmsAuthVps")]
    public class AppMetricaInfo : ScriptableObject
    {
        [field: SerializeField] public string Key { get; private set; }
        [field: SerializeField] public string VarioqubId { get; private set; }
    }
}
