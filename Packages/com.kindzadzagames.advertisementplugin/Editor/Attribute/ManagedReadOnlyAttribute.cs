using UnityEngine;

namespace KinDzaDzaGames.AdvertisementPlugin.EditorScripts
{
    public class ManagedReadOnlyAttribute : PropertyAttribute
    {
        public string ConditionFieldName;

        public ManagedReadOnlyAttribute(string conditionFieldName) => ConditionFieldName = conditionFieldName;
    }
}
