#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

namespace RDTools.Editor
{
    [CustomPropertyDrawer(typeof(SortingLayerAttribute))]
    public class SortingLayerPropertyDrawer : PropertyDrawerBase
    {
        private const string TypeWarningMessage = "{0} must be an int or a string";
        private static string[] sortingLayers;

        // Cache sorting layers to avoid repeated calls to GetLayers
        private static string[] SortingLayers => sortingLayers ??= GetSortingLayers();

        protected override float GetPropertyHeight_Internal(SerializedProperty property, GUIContent label)
        {
            bool isValidType = property.propertyType == SerializedPropertyType.String || property.propertyType == SerializedPropertyType.Integer;
            return GetPropertyHeight(property) + (isValidType ? 0 : GetHelpBoxHeight());
        }

        protected override void OnGUI_Internal(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            switch (property.propertyType)
            {
                case SerializedPropertyType.String:
                    DrawStringProperty(rect, property, label);
                    break;
                case SerializedPropertyType.Integer:
                    DrawIntProperty(rect, property, label);
                    break;
                default:
                    DrawInvalidProperty(rect, property);
                    break;
            }

            EditorGUI.EndProperty();
        }

        private static string[] GetSortingLayers()
        {
            Type internalEditorUtilityType = typeof(UnityEditorInternal.InternalEditorUtility);
            PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
            return (string[])sortingLayersProperty.GetValue(null);
        }

        private static void DrawStringProperty(Rect rect, SerializedProperty property, GUIContent label)
        {
            int currentIndex = Array.IndexOf(SortingLayers, property.stringValue);
            int newIndex = EditorGUI.Popup(rect, label.text, Mathf.Max(currentIndex, 0), SortingLayers);
            string selectedLayer = SortingLayers[newIndex];

            if (!property.stringValue.Equals(selectedLayer, StringComparison.Ordinal))
            {
                property.stringValue = selectedLayer;
            }
        }

        private static void DrawIntProperty(Rect rect, SerializedProperty property, GUIContent label)
        {
            string currentLayer = SortingLayer.IDToName(property.intValue);
            int currentIndex = Array.IndexOf(SortingLayers, currentLayer);
            int newIndex = EditorGUI.Popup(rect, label.text, Mathf.Max(currentIndex, 0), SortingLayers);
            string selectedLayer = SortingLayers[newIndex];
            int newLayerID = SortingLayer.NameToID(selectedLayer);

            if (property.intValue != newLayerID)
            {
                property.intValue = newLayerID;
            }
        }

        private void DrawInvalidProperty(Rect rect, SerializedProperty property)
        {
            string message = string.Format(TypeWarningMessage, property.name);
            DrawDefaultPropertyAndHelpBox(rect, property, message, MessageType.Warning);
        }
    }
}
#endif
