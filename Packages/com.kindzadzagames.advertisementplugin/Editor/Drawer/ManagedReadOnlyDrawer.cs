using UnityEditor;
using UnityEngine;

namespace KinDzaDzaGames.AdvertisementPlugin.EditorScripts
{
    [CustomPropertyDrawer(typeof(ManagedReadOnlyAttribute))]
    public class ManagedReadOnlyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUI.GetPropertyHeight(property, label, true);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ManagedReadOnlyAttribute condAttribute = (ManagedReadOnlyAttribute)attribute;
            SerializedProperty conditionProperty = property.serializedObject.FindProperty(condAttribute.ConditionFieldName);

            if (conditionProperty != null && conditionProperty.propertyType == SerializedPropertyType.Boolean)
            {
                bool shouldBeReadOnly = conditionProperty.boolValue;

                GUI.enabled = shouldBeReadOnly;
                EditorGUI.PropertyField(position, property, label, true);
                GUI.enabled = true;
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, true);

                if (conditionProperty == null)
                {
                    EditorGUI.HelpBox(new Rect(position.x, position.y + position.height + 2, position.width, 20),
                        $"Field '{condAttribute.ConditionFieldName}' not found", MessageType.Error);
                }
                else if (conditionProperty.propertyType != SerializedPropertyType.Boolean)
                {
                    EditorGUI.HelpBox(new Rect(position.x, position.y + position.height + 2, position.width, 20),
                        $"Field '{condAttribute.ConditionFieldName}' must be boolean", MessageType.Error);
                }
            }
        }
    }
}
