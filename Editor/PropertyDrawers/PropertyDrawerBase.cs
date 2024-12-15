#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace RDTools.Editor
{
    public abstract class PropertyDrawerBase : PropertyDrawer
    {
        public sealed override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            if (!PropertyUtility.IsVisible(property))
            {
                return;
            }

            // Validate property
            foreach (var validatorAttribute in PropertyUtility.GetAttributes<ValidatorAttribute>(property))
            {
                validatorAttribute.GetValidator().ValidateProperty(property);
            }

            // Draw property if enabled
            EditorGUI.BeginChangeCheck();
            using (new EditorGUI.DisabledScope(!PropertyUtility.IsEnabled(property)))
            {
                OnGUI_Internal(rect, property, PropertyUtility.GetLabel(property));
            }

            // Trigger OnValueChanged callbacks
            if (EditorGUI.EndChangeCheck())
            {
                PropertyUtility.CallOnValueChangedCallbacks(property);
            }
        }

        protected abstract void OnGUI_Internal(Rect rect, SerializedProperty property, GUIContent label);

        public sealed override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return PropertyUtility.IsVisible(property) ? GetPropertyHeight_Internal(property, label) : 0.0f;
        }

        protected virtual float GetPropertyHeight_Internal(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, includeChildren: true);
        }

        protected float GetPropertyHeight(SerializedProperty property)
        {
            var specialCaseAttribute = PropertyUtility.GetAttribute<SpecialCaseDrawerAttribute>(property);
            return specialCaseAttribute != null
                ? specialCaseAttribute.GetDrawer().GetPropertyHeight(property)
                : EditorGUI.GetPropertyHeight(property, includeChildren: true);
        }

        public virtual float GetHelpBoxHeight()
        {
            return EditorGUIUtility.singleLineHeight * 2.0f;
        }

        public void DrawDefaultPropertyAndHelpBox(Rect rect, SerializedProperty property, string message, MessageType messageType)
        {
            float indentLength = RDEditorGUI.GetIndentLength(rect);

            // Draw HelpBox
            Rect helpBoxRect = new Rect(
                rect.x + indentLength,
                rect.y,
                rect.width - indentLength,
                GetHelpBoxHeight());
            RDEditorGUI.HelpBox(helpBoxRect, message, messageType, property.serializedObject.targetObject);

            // Draw property field
            Rect propertyRect = new Rect(
                rect.x,
                rect.y + GetHelpBoxHeight(),
                rect.width,
                GetPropertyHeight(property));
            EditorGUI.PropertyField(propertyRect, property, true);
        }
    }
}
#endif
