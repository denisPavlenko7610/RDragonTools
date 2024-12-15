#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RDTools.Editor
{
    public abstract class SpecialCasePropertyDrawerBase
    {
        public void OnGUI(Rect rect, SerializedProperty property)
        {
            // Early exit if the property is not visible
            if (!PropertyUtility.IsVisible(property)) return;

            // Validate property with attributes
            ValidateProperty(property);

            // Check if the property is enabled and draw it
            bool isEnabled = PropertyUtility.IsEnabled(property);
            EditorGUI.BeginChangeCheck();
            
            using (new EditorGUI.DisabledScope(!isEnabled))
            {
                OnGUI_Internal(rect, property, PropertyUtility.GetLabel(property));
            }

            // Call on value changed callbacks if there were changes
            if (EditorGUI.EndChangeCheck())
            {
                PropertyUtility.CallOnValueChangedCallbacks(property);
            }
        }

        public float GetPropertyHeight(SerializedProperty property)
        {
            return GetPropertyHeight_Internal(property);
        }

        protected abstract void OnGUI_Internal(Rect rect, SerializedProperty property, GUIContent label);
        protected abstract float GetPropertyHeight_Internal(SerializedProperty property);

        private void ValidateProperty(SerializedProperty property)
        {
            foreach (var validatorAttribute in PropertyUtility.GetAttributes<ValidatorAttribute>(property))
            {
                validatorAttribute.GetValidator().ValidateProperty(property);
            }
        }
    }

    public static class SpecialCaseDrawerAttributeExtensions
    {
        private static readonly Dictionary<Type, SpecialCasePropertyDrawerBase> DrawersByAttributeType = new();

        public static SpecialCasePropertyDrawerBase GetDrawer(this SpecialCaseDrawerAttribute attr)
        {
            return DrawersByAttributeType.TryGetValue(attr.GetType(), out var drawer) ? drawer : null;
        }
    }
}
#endif
