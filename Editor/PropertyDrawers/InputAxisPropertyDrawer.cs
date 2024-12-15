#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RDTools.Editor
{
    [CustomPropertyDrawer(typeof(InputAxisAttribute))]
    public class InputAxisPropertyDrawer : PropertyDrawerBase
    {
        private static readonly string AssetPath = Path.Combine("ProjectSettings", "InputManager.asset");
        private const string AxesPropertyPath = "m_Axes";
        private const string NamePropertyPath = "m_Name";

        protected override float GetPropertyHeight_Internal(SerializedProperty property, GUIContent label)
        {
            return property.propertyType == SerializedPropertyType.String
                ? base.GetPropertyHeight(property)
                : base.GetPropertyHeight(property) + GetHelpBoxHeight();
        }

        protected override void OnGUI_Internal(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            if (property.propertyType == SerializedPropertyType.String)
            {
                DrawInputAxisDropdown(rect, property, label);
            }
            else
            {
                DisplayUnsupportedFieldTypeMessage(rect, property);
            }

            EditorGUI.EndProperty();
        }

        private void DrawInputAxisDropdown(Rect rect, SerializedProperty property, GUIContent label)
        {
            var inputManagerAsset = AssetDatabase.LoadAssetAtPath(AssetPath, typeof(Object));
            if (inputManagerAsset == null)
            {
                Debug.LogError($"Failed to load InputManager asset at path: {AssetPath}");
                return;
            }

            var inputManager = new SerializedObject(inputManagerAsset);
            var axesProperty = inputManager.FindProperty(AxesPropertyPath);

            if (axesProperty == null || !axesProperty.isArray)
            {
                Debug.LogError("Failed to find 'm_Axes' property in InputManager asset.");
                return;
            }

            var axes = ExtractAxesNames(axesProperty);
            DrawAxesPopup(rect, property, label, axes);
        }

        private HashSet<string> ExtractAxesNames(SerializedProperty axesProperty)
        {
            var axesSet = new HashSet<string> { "(None)" };

            for (int i = 0; i < axesProperty.arraySize; i++)
            {
                var axis = axesProperty.GetArrayElementAtIndex(i).FindPropertyRelative(NamePropertyPath).stringValue;
                axesSet.Add(axis);
            }

            return axesSet;
        }

        private void DrawAxesPopup(Rect rect, SerializedProperty property, GUIContent label, HashSet<string> axes)
        {
            var axesArray = axes.ToArray();
            System.Array.Sort(axesArray, System.StringComparer.Ordinal);

            string currentValue = property.stringValue;
            int currentIndex = FindAxisIndex(currentValue, axesArray);

            int selectedIndex = EditorGUI.Popup(rect, label.text, currentIndex, axesArray);
            string newValue = selectedIndex > 0 ? axesArray[selectedIndex] : string.Empty;

            if (!currentValue.Equals(newValue, System.StringComparison.Ordinal))
            {
                property.stringValue = newValue;
            }
        }

        private int FindAxisIndex(string currentValue, string[] axesArray)
        {
            for (int i = 0; i < axesArray.Length; i++)
            {
                if (axesArray[i].Equals(currentValue, System.StringComparison.Ordinal))
                {
                    return i;
                }
            }
            return 0; // Default to "(None)"
        }

        private void DisplayUnsupportedFieldTypeMessage(Rect rect, SerializedProperty property)
        {
            string message = $"{typeof(InputAxisAttribute).Name} supports only string fields.";
            DrawDefaultPropertyAndHelpBox(rect, property, message, MessageType.Warning);
        }
    }
}
#endif
