#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace RDTools.Editor
{
    [CustomPropertyDrawer(typeof(LayerAttribute))]
    public class LayerPropertyDrawer : PropertyDrawerBase
    {
        private const string TypeWarningMessage = "{0} must be an int or a string";

        protected override float GetPropertyHeight_Internal(SerializedProperty property, GUIContent label)
        {
            bool isValidPropertyType = property.propertyType == SerializedPropertyType.String || property.propertyType == SerializedPropertyType.Integer;
            return isValidPropertyType
                ? base.GetPropertyHeight(property)
                : base.GetPropertyHeight(property) + GetHelpBoxHeight();
        }

        protected override void OnGUI_Internal(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            if (property.propertyType == SerializedPropertyType.String)
            {
                DrawStringProperty(rect, property, label);
            }
            else if (property.propertyType == SerializedPropertyType.Integer)
            {
                DrawIntProperty(rect, property, label);
            }
            else
            {
                string message = string.Format(TypeWarningMessage, property.name);
                DrawDefaultPropertyAndHelpBox(rect, property, message, MessageType.Warning);
            }

            EditorGUI.EndProperty();
        }

        private void DrawStringProperty(Rect rect, SerializedProperty property, GUIContent label)
        {
            string[] layers = GetLayers();
            int currentIndex = Array.IndexOf(layers, property.stringValue);
            currentIndex = Mathf.Clamp(currentIndex, 0, layers.Length - 1);

            int newIndex = EditorGUI.Popup(rect, label.text, currentIndex, layers);

            if (!property.stringValue.Equals(layers[newIndex], StringComparison.Ordinal))
            {
                property.stringValue = layers[newIndex];
            }
        }

        private void DrawIntProperty(Rect rect, SerializedProperty property, GUIContent label)
        {
            string[] layers = GetLayers();
            string currentLayerName = LayerMask.LayerToName(property.intValue);
            int currentIndex = Array.FindIndex(layers, layer => layer.Equals(currentLayerName, StringComparison.Ordinal));
            currentIndex = Mathf.Clamp(currentIndex, 0, layers.Length - 1);

            int newIndex = EditorGUI.Popup(rect, label.text, currentIndex, layers);
            int newLayerValue = LayerMask.NameToLayer(layers[newIndex]);

            if (property.intValue != newLayerValue)
            {
                property.intValue = newLayerValue;
            }
        }

        private string[] GetLayers() => UnityEditorInternal.InternalEditorUtility.layers;
    }
}
#endif
