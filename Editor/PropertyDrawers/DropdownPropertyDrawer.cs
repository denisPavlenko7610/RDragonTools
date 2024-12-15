#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace RDTools.Editor
{
    [CustomPropertyDrawer(typeof(DropdownAttribute))]
    public class DropdownPropertyDrawer : PropertyDrawerBase
    {
        protected override float GetPropertyHeight_Internal(SerializedProperty property, GUIContent label)
        {
            var dropdownAttribute = (DropdownAttribute)attribute;
            object values = GetValues(property, dropdownAttribute.ValuesName);
            FieldInfo fieldInfo = ReflectionUtility.GetField(PropertyUtility.GetTargetObjectWithProperty(property), property.name);

            return AreValuesValid(values, fieldInfo)
                ? GetPropertyHeight(property)
                : GetPropertyHeight(property) + GetHelpBoxHeight();
        }

        protected override void OnGUI_Internal(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            var dropdownAttribute = (DropdownAttribute)attribute;
            object targetObject = PropertyUtility.GetTargetObjectWithProperty(property);
            object valuesObject = GetValues(property, dropdownAttribute.ValuesName);
            FieldInfo dropdownField = ReflectionUtility.GetField(targetObject, property.name);

            if (AreValuesValid(valuesObject, dropdownField))
            {
                DrawDropdown(rect, property, label, targetObject, dropdownField, valuesObject);
            }
            else
            {
                string errorMessage = $"Invalid values with name '{dropdownAttribute.ValuesName}' provided to '{dropdownAttribute.GetType().Name}'. " +
                                      "Ensure the values name is correct and the target field's type matches the values field/property/method.";
                DrawDefaultPropertyAndHelpBox(rect, property, errorMessage, MessageType.Warning);
            }

            EditorGUI.EndProperty();
        }

        private void DrawDropdown(Rect rect, SerializedProperty property, GUIContent label, object targetObject, FieldInfo dropdownField, object valuesObject)
        {
            if (valuesObject is IList valuesList && dropdownField.FieldType == GetElementType(valuesList))
            {
                DrawListDropdown(rect, property, label, targetObject, dropdownField, valuesList);
            }
            else if (valuesObject is IDropdownList dropdown)
            {
                DrawCustomDropdown(rect, property, label, targetObject, dropdownField, dropdown);
            }
        }

        private void DrawListDropdown(Rect rect, SerializedProperty property, GUIContent label, object targetObject, FieldInfo dropdownField, IList valuesList)
        {
            object selectedValue = dropdownField.GetValue(targetObject);

            string[] displayOptions = new string[valuesList.Count];
            object[] valuesArray = new object[valuesList.Count];

            for (int i = 0; i < valuesList.Count; i++)
            {
                valuesArray[i] = valuesList[i];
                displayOptions[i] = valuesList[i]?.ToString() ?? "<null>";
            }

            int selectedIndex = Array.IndexOf(valuesArray, selectedValue);
            selectedIndex = Mathf.Max(0, selectedIndex);

            RDEditorGUI.Dropdown(rect, property.serializedObject, targetObject, dropdownField, label.text, selectedIndex, valuesArray, displayOptions);
        }

        private void DrawCustomDropdown(Rect rect, SerializedProperty property, GUIContent label, object targetObject, FieldInfo dropdownField, IDropdownList dropdown)
        {
            object selectedValue = dropdownField.GetValue(targetObject);

            List<object> values = new List<object>();
            List<string> displayOptions = new List<string>();

            int selectedIndex = -1;
            int index = -1;

            foreach (var pair in dropdown)
            {
                index++;
                values.Add(pair.Value);
                displayOptions.Add(GetDisplayText(pair.Key));

                if (pair.Value?.Equals(selectedValue) == true)
                {
                    selectedIndex = index;
                }
            }

            selectedIndex = Mathf.Max(0, selectedIndex);

            RDEditorGUI.Dropdown(rect, property.serializedObject, targetObject, dropdownField, label.text, selectedIndex, values.ToArray(), displayOptions.ToArray());
        }

        private string GetDisplayText(string key)
        {
            if (key == null) return "<null>";
            return string.IsNullOrWhiteSpace(key) ? "<empty>" : key;
        }

        private object GetValues(SerializedProperty property, string valuesName)
        {
            object targetObject = PropertyUtility.GetTargetObjectWithProperty(property);

            FieldInfo field = ReflectionUtility.GetField(targetObject, valuesName);
            if (field != null)
            {
                return field.GetValue(targetObject);
            }

            PropertyInfo propertyInfo = ReflectionUtility.GetProperty(targetObject, valuesName);
            if (propertyInfo != null)
            {
                return propertyInfo.GetValue(targetObject);
            }

            MethodInfo method = ReflectionUtility.GetMethod(targetObject, valuesName);
            if (method != null && method.ReturnType != typeof(void) && method.GetParameters().Length == 0)
            {
                return method.Invoke(targetObject, null);
            }

            return null;
        }

        private bool AreValuesValid(object values, FieldInfo dropdownField)
        {
            if (values == null || dropdownField == null)
            {
                return false;
            }

            return (values is IList && dropdownField.FieldType == GetElementType(values)) || (values is IDropdownList);
        }

        private Type GetElementType(object values)
        {
            return ReflectionUtility.GetListElementType(values.GetType());
        }
    }
}
#endif
