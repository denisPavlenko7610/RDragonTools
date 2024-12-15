#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace RDTools.Editor
{
    [CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
    public class EnumFlagsPropertyDrawer : PropertyDrawerBase
    {
        // Calculate property height based on whether the property is a valid Enum
        protected override float GetPropertyHeight_Internal(SerializedProperty property, GUIContent label)
        {
            var targetEnum = PropertyUtility.GetTargetObjectOfProperty(property) as Enum;
            return targetEnum != null 
                ? GetPropertyHeight(property) 
                : GetPropertyHeight(property) + GetHelpBoxHeight();
        }

        // Render the property in the inspector
        protected override void OnGUI_Internal(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            var targetEnum = PropertyUtility.GetTargetObjectOfProperty(property) as Enum;

            if (targetEnum != null)
            {
                // Display the EnumFlagsField and update the property value
                var updatedEnum = EditorGUI.EnumFlagsField(rect, label.text, targetEnum);
                property.intValue = Convert.ToInt32(updatedEnum);
            }
            else
            {
                // Show a warning message if the Attribute is used incorrectly
                var message = $"{attribute.GetType().Name} can be used only on enums.";
                DrawDefaultPropertyAndHelpBox(rect, property, message, MessageType.Warning);
            }

            EditorGUI.EndProperty();
        }
    }
}
#endif