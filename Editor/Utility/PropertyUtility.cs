#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RDTools.Editor
{
    public static class PropertyUtility
    {
        public static T GetAttribute<T>(SerializedProperty property) where T : class
        {
            T[] attributes = GetAttributes<T>(property);
            return attributes.Length > 0 ? attributes[0] : null;
        }

        public static T[] GetAttributes<T>(SerializedProperty property) where T : class
        {
            FieldInfo fieldInfo = ReflectionUtility.GetField(GetTargetObjectWithProperty(property), property.name);
            return fieldInfo == null ? Array.Empty<T>() : (T[])fieldInfo.GetCustomAttributes(typeof(T), true);
        }

        public static GUIContent GetLabel(SerializedProperty property)
        {
            LabelAttribute labelAttribute = GetAttribute<LabelAttribute>(property);
            string labelText = labelAttribute?.Label ?? property.displayName;
            return new GUIContent(labelText);
        }

        public static void CallOnValueChangedCallbacks(SerializedProperty property)
        {
            OnValueChangedAttribute[] attributes = GetAttributes<OnValueChangedAttribute>(property);
            if (attributes.Length == 0) return;

            object target = GetTargetObjectWithProperty(property);
            property.serializedObject.ApplyModifiedProperties(); // Ensure new value is applied

            foreach (var attribute in attributes)
            {
                MethodInfo callbackMethod = ReflectionUtility.GetMethod(target, attribute.CallbackName);
                if (callbackMethod != null && callbackMethod.ReturnType == typeof(void) && callbackMethod.GetParameters().Length == 0)
                {
                    callbackMethod.Invoke(target, null);
                }
                else
                {
                    Debug.LogWarning(
                        $"{attribute.GetType().Name} can only invoke methods with 'void' return type and no parameters.", 
                        property.serializedObject.targetObject
                    );
                }
            }
        }

        public static bool IsEnabled(SerializedProperty property)
        {
            if (GetAttribute<ReadOnlyAttribute>(property) != null)
                return false;

            EnableIfAttributeBase enableIf = GetAttribute<EnableIfAttributeBase>(property);
            if (enableIf == null) return true;

            object target = GetTargetObjectWithProperty(property);

            // Handle enum conditions
            if (enableIf.EnumValue != null)
            {
                Enum value = GetEnumValue(target, enableIf.Conditions[0]);
                if (value != null)
                {
                    bool matched = value.GetType().GetCustomAttribute<FlagsAttribute>() == null
                        ? enableIf.EnumValue.Equals(value)
                        : value.HasFlag(enableIf.EnumValue);

                    return matched != enableIf.Inverted;
                }

                Debug.LogWarning(
                    $"{enableIf.GetType().Name} requires a valid enum field, property, or method name.", 
                    property.serializedObject.targetObject
                );
                return false;
            }

            // Handle boolean conditions
            List<bool> conditionValues = GetConditionValues(target, enableIf.Conditions);
            if (conditionValues.Count > 0)
                return GetConditionsFlag(conditionValues, enableIf.ConditionOperator, enableIf.Inverted);

            Debug.LogWarning(
                $"{enableIf.GetType().Name} requires valid boolean conditions.", 
                property.serializedObject.targetObject
            );
            return false;
        }

        public static bool IsVisible(SerializedProperty property)
        {
            ShowIfAttributeBase showIf = GetAttribute<ShowIfAttributeBase>(property);
            if (showIf == null) return true;

            object target = GetTargetObjectWithProperty(property);

            // Handle enum conditions
            if (showIf.EnumValue != null)
            {
                Enum value = GetEnumValue(target, showIf.Conditions[0]);
                if (value != null)
                {
                    bool matched = value.GetType().GetCustomAttribute<FlagsAttribute>() == null
                        ? showIf.EnumValue.Equals(value)
                        : value.HasFlag(showIf.EnumValue);

                    return matched != showIf.Inverted;
                }

                Debug.LogWarning(
                    $"{showIf.GetType().Name} requires a valid enum field, property, or method name.", 
                    property.serializedObject.targetObject
                );
                return false;
            }

            // Handle boolean conditions
            List<bool> conditionValues = GetConditionValues(target, showIf.Conditions);
            if (conditionValues.Count > 0)
                return GetConditionsFlag(conditionValues, showIf.ConditionOperator, showIf.Inverted);

            Debug.LogWarning(
                $"{showIf.GetType().Name} requires valid boolean conditions.", 
                property.serializedObject.targetObject
            );
            return false;
        }

        private static Enum GetEnumValue(object target, string enumName)
        {
            FieldInfo field = ReflectionUtility.GetField(target, enumName);
            if (field?.FieldType.IsSubclassOf(typeof(Enum)) == true)
                return (Enum)field.GetValue(target);

            PropertyInfo property = ReflectionUtility.GetProperty(target, enumName);
            if (property?.PropertyType.IsSubclassOf(typeof(Enum)) == true)
                return (Enum)property.GetValue(target);

            MethodInfo method = ReflectionUtility.GetMethod(target, enumName);
            if (method?.ReturnType.IsSubclassOf(typeof(Enum)) == true)
                return (Enum)method.Invoke(target, null);

            return null;
        }

        public static List<bool> GetConditionValues(object target, string[] conditions)
        {
            var conditionValues = new List<bool>();

            foreach (var condition in conditions)
            {
                FieldInfo field = ReflectionUtility.GetField(target, condition);
                if (field?.FieldType == typeof(bool))
                {
                    conditionValues.Add((bool)field.GetValue(target));
                    continue;
                }

                PropertyInfo property = ReflectionUtility.GetProperty(target, condition);
                if (property?.PropertyType == typeof(bool))
                {
                    conditionValues.Add((bool)property.GetValue(target));
                    continue;
                }

                MethodInfo method = ReflectionUtility.GetMethod(target, condition);
                if (method?.ReturnType == typeof(bool) && method.GetParameters().Length == 0)
                {
                    conditionValues.Add((bool)method.Invoke(target, null));
                }
            }

            return conditionValues;
        }

        public static bool GetConditionsFlag(List<bool> conditionValues, EConditionOperator conditionOperator, bool invert)
        {
            bool flag = conditionOperator == EConditionOperator.And
                ? conditionValues.TrueForAll(v => v)
                : conditionValues.Exists(v => v);

            return invert ? !flag : flag;
        }

        public static Type GetPropertyType(SerializedProperty property)
        {
            object obj = GetTargetObjectOfProperty(property);
            return obj?.GetType();
        }

        public static object GetTargetObjectOfProperty(SerializedProperty property)
        {
            if (property == null) return null;

            string path = property.propertyPath.Replace(".Array.data[", "[");
            object obj = property.serializedObject.targetObject;

            foreach (var element in path.Split('.'))
            {
                if (element.Contains("["))
                {
                    string elementName = element.Substring(0, element.IndexOf("["));
                    int index = int.Parse(element.Substring(element.IndexOf("[")).Trim('[', ']'));
                    obj = GetValue_Imp(obj, elementName, index);
                }
                else
                {
                    obj = GetValue_Imp(obj, element);
                }
            }

            return obj;
        }

        public static object GetTargetObjectWithProperty(SerializedProperty property)
        {
            string path = property.propertyPath.Replace(".Array.data[", "[");
            object obj = property.serializedObject.targetObject;

            string[] elements = path.Split('.');
            for (int i = 0; i < elements.Length - 1; i++)
            {
                string element = elements[i];
                if (element.Contains("["))
                {
                    string elementName = element.Substring(0, element.IndexOf("["));
                    int index = int.Parse(element.Substring(element.IndexOf("[")).Trim('[', ']'));
                    obj = GetValue_Imp(obj, elementName, index);
                }
                else
                {
                    obj = GetValue_Imp(obj, element);
                }
            }

            return obj;
        }

        private static object GetValue_Imp(object source, string name)
        {
            if (source == null) return null;

            Type type = source.GetType();
            while (type != null)
            {
                FieldInfo field = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (field != null) return field.GetValue(source);

                PropertyInfo property = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (property != null) return property.GetValue(source);

                type = type.BaseType;
            }

            return null;
        }

        private static object GetValue_Imp(object source, string name, int index)
        {
            IEnumerable enumerable = GetValue_Imp(source, name) as IEnumerable;
            if (enumerable == null) return null;

            IEnumerator enumerator = enumerable.GetEnumerator();
            for (int i = 0; i <= index; i++)
            {
                if (!enumerator.MoveNext()) return null;
            }

            return enumerator.Current;
        }
    }
}
#endif
