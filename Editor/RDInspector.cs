#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace RDTools.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UnityEngine.Object), true)]
    public class RDInspector : UnityEditor.Editor
    {
        private List<SerializedProperty> _serializedProperties = new List<SerializedProperty>();
        private IEnumerable<FieldInfo> _nonSerializedFields;
        private IEnumerable<PropertyInfo> _nativeProperties;
        private IEnumerable<MethodInfo> _methods;
        private Dictionary<string, SavedBool> _foldouts = new Dictionary<string, SavedBool>();

        protected virtual void OnEnable()
        {
            _nonSerializedFields = GetFieldsWithAttribute<ShowNonSerializedFieldAttribute>();
            _nativeProperties = GetPropertiesWithAttribute<ShowNativePropertyAttribute>();
            _methods = GetMethodsWithAttribute<ButtonAttribute>();
        }

        public override void OnInspectorGUI()
        {
            GetSerializedProperties(ref _serializedProperties);

            bool anyNaughtyAttribute = _serializedProperties.Any(p => PropertyUtility.GetAttribute<INaughtyAttribute>(p) != null);
            if (anyNaughtyAttribute)
            {
                DrawSerializedProperties();
            }
            else
            {
                DrawDefaultInspector();
            }

            DrawNonSerializedFields();
            DrawNativeProperties();
            DrawButtons();
        }

        private void GetSerializedProperties(ref List<SerializedProperty> outSerializedProperties)
        {
            outSerializedProperties.Clear();
            using (var iterator = serializedObject.GetIterator())
            {
                if (iterator.NextVisible(true))
                {
                    do
                    {
                        outSerializedProperties.Add(serializedObject.FindProperty(iterator.name));
                    }
                    while (iterator.NextVisible(false));
                }
            }
        }

        private void DrawSerializedProperties()
        {
            serializedObject.Update();

            DrawNonGroupedProperties();
            DrawGroupedProperties();
            DrawFoldoutProperties();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawNonGroupedProperties()
        {
            foreach (var property in GetNonGroupedProperties(_serializedProperties))
            {
                if (property.name.Equals("m_Script", System.StringComparison.Ordinal))
                {
                    using (new EditorGUI.DisabledScope(true))
                    {
                        EditorGUILayout.PropertyField(property);
                    }
                }
                else
                {
                    RDEditorGUI.PropertyField_Layout(property, true);
                }
            }
        }

        private void DrawGroupedProperties()
        {
            foreach (var group in GetGroupedProperties(_serializedProperties))
            {
                var visibleProperties = group.Where(p => PropertyUtility.IsVisible(p));
                if (visibleProperties.Any())
                {
                    RDEditorGUI.BeginBoxGroup_Layout(group.Key);
                    foreach (var property in visibleProperties)
                    {
                        RDEditorGUI.PropertyField_Layout(property, true);
                    }
                    RDEditorGUI.EndBoxGroup_Layout();
                }
            }
        }

        private void DrawFoldoutProperties()
        {
            foreach (var group in GetFoldoutProperties(_serializedProperties))
            {
                var visibleProperties = group.Where(p => PropertyUtility.IsVisible(p));
                if (visibleProperties.Any())
                {
                    if (!_foldouts.ContainsKey(group.Key))
                    {
                        _foldouts[group.Key] = new SavedBool($"{target.GetInstanceID()}.{group.Key}", false);
                    }

                    _foldouts[group.Key].Value = EditorGUILayout.Foldout(_foldouts[group.Key].Value, group.Key, true);
                    if (_foldouts[group.Key].Value)
                    {
                        foreach (var property in visibleProperties)
                        {
                            RDEditorGUI.PropertyField_Layout(property, true);
                        }
                    }
                }
            }
        }

        private void DrawNonSerializedFields(bool drawHeader = false)
        {
            if (_nonSerializedFields.Any())
            {
                if (drawHeader)
                {
                    DrawSectionHeader("Non-Serialized Fields");
                }

                foreach (var field in _nonSerializedFields)
                {
                    RDEditorGUI.NonSerializedField_Layout(serializedObject.targetObject, field);
                }
            }
        }

        private void DrawNativeProperties(bool drawHeader = false)
        {
            if (_nativeProperties.Any())
            {
                if (drawHeader)
                {
                    DrawSectionHeader("Native Properties");
                }

                foreach (var property in _nativeProperties)
                {
                    RDEditorGUI.NativeProperty_Layout(serializedObject.targetObject, property);
                }
            }
        }

        private void DrawButtons(bool drawHeader = false)
        {
            if (_methods.Any())
            {
                if (drawHeader)
                {
                    DrawSectionHeader("Buttons");
                }

                foreach (var method in _methods)
                {
                    RDEditorGUI.Button(serializedObject.targetObject, method);
                }
            }
        }

        private void DrawSectionHeader(string header)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(header, GetHeaderGUIStyle());
            RDEditorGUI.HorizontalLine(
                EditorGUILayout.GetControlRect(false), HorizontalLineAttribute.DefaultHeight, HorizontalLineAttribute.DefaultColor.GetColor());
        }

        private static GUIStyle GetHeaderGUIStyle()
        {
            GUIStyle style = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
            {
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.UpperCenter
            };

            return style;
        }

        private static IEnumerable<SerializedProperty> GetNonGroupedProperties(IEnumerable<SerializedProperty> properties) =>
            properties.Where(p => PropertyUtility.GetAttribute<IGroupAttribute>(p) == null);

        private static IEnumerable<IGrouping<string, SerializedProperty>> GetGroupedProperties(IEnumerable<SerializedProperty> properties) =>
            properties
                .Where(p => PropertyUtility.GetAttribute<BoxGroupAttribute>(p) != null)
                .GroupBy(p => PropertyUtility.GetAttribute<BoxGroupAttribute>(p).Name);

        private static IEnumerable<IGrouping<string, SerializedProperty>> GetFoldoutProperties(IEnumerable<SerializedProperty> properties) =>
            properties
                .Where(p => PropertyUtility.GetAttribute<FoldoutAttribute>(p) != null)
                .GroupBy(p => PropertyUtility.GetAttribute<FoldoutAttribute>(p).Name);

        private IEnumerable<FieldInfo> GetFieldsWithAttribute<TAttribute>() where TAttribute : System.Attribute =>
            ReflectionUtility.GetAllFields(target, f => f.GetCustomAttributes(typeof(TAttribute), true).Any());

        private IEnumerable<PropertyInfo> GetPropertiesWithAttribute<TAttribute>() where TAttribute : System.Attribute =>
            ReflectionUtility.GetAllProperties(target, p => p.GetCustomAttributes(typeof(TAttribute), true).Any());

        private IEnumerable<MethodInfo> GetMethodsWithAttribute<TAttribute>() where TAttribute : System.Attribute =>
            ReflectionUtility.GetAllMethods(target, m => m.GetCustomAttributes(typeof(TAttribute), true).Any());
    }
}
#endif
