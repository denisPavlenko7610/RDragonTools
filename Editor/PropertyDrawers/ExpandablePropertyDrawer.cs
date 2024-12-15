#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace RDTools.Editor
{
    [CustomPropertyDrawer(typeof(ExpandableAttribute))]
    public class ExpandablePropertyDrawer : PropertyDrawerBase
    {
        protected override float GetPropertyHeight_Internal(SerializedProperty property, GUIContent label)
        {
            if (property.objectReferenceValue == null)
                return GetPropertyHeight(property);

            var propertyType = PropertyUtility.GetPropertyType(property);
            if (!typeof(ScriptableObject).IsAssignableFrom(propertyType))
                return GetPropertyHeight(property) + GetHelpBoxHeight();

            var scriptableObject = property.objectReferenceValue as ScriptableObject;
            if (scriptableObject == null || !property.isExpanded)
                return GetPropertyHeight(property);

            using (var serializedObject = new SerializedObject(scriptableObject))
            {
                float totalHeight = EditorGUIUtility.singleLineHeight;
                var iterator = serializedObject.GetIterator();

                if (iterator.NextVisible(true))
                {
                    do
                    {
                        if (ShouldSkipProperty(iterator))
                            continue;

                        totalHeight += GetPropertyHeight(serializedObject.FindProperty(iterator.name));
                    }
                    while (iterator.NextVisible(false));
                }

                totalHeight += EditorGUIUtility.standardVerticalSpacing;
                return totalHeight;
            }
        }

        protected override void OnGUI_Internal(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            if (property.objectReferenceValue == null)
            {
                EditorGUI.PropertyField(rect, property, label, false);
            }
            else
            {
                var propertyType = PropertyUtility.GetPropertyType(property);
                if (typeof(ScriptableObject).IsAssignableFrom(propertyType))
                {
                    HandleScriptableObjectField(rect, property, label);
                }
                else
                {
                    DrawDefaultPropertyAndHelpBox(rect, property, $"{typeof(ExpandableAttribute).Name} can only be used on ScriptableObjects", MessageType.Warning);
                }
            }

            property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty();
        }

        private void HandleScriptableObjectField(Rect rect, SerializedProperty property, GUIContent label)
        {
            var scriptableObject = property.objectReferenceValue as ScriptableObject;
            if (scriptableObject == null)
            {
                EditorGUI.PropertyField(rect, property, label, false);
                return;
            }

            // Draw foldout
            var foldoutRect = new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label, true);

            // Draw main property field
            var propertyRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(propertyRect, property, label, false);

            // Draw child properties if expanded
            if (property.isExpanded)
            {
                DrawChildProperties(rect, property);
            }
        }

        private void DrawChildProperties(Rect rect, SerializedProperty property)
        {
            var scriptableObject = property.objectReferenceValue as ScriptableObject;
            if (scriptableObject == null)
                return;

            var boxRect = new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, rect.width, rect.height - EditorGUIUtility.singleLineHeight);
            GUI.Box(boxRect, GUIContent.none);

            using (new EditorGUI.IndentLevelScope())
            {
                var serializedObject = new SerializedObject(scriptableObject);
                serializedObject.Update();

                var iterator = serializedObject.GetIterator();
                float yOffset = EditorGUIUtility.singleLineHeight;

                if (iterator.NextVisible(true))
                {
                    do
                    {
                        if (ShouldSkipProperty(iterator))
                            continue;

                        var childProperty = serializedObject.FindProperty(iterator.name);
                        var childHeight = GetPropertyHeight(childProperty);

                        var childRect = new Rect(rect.x, rect.y + yOffset, rect.width, childHeight);
                        RDEditorGUI.PropertyField(childRect, childProperty, true);

                        yOffset += childHeight;
                    }
                    while (iterator.NextVisible(false));
                }

                serializedObject.ApplyModifiedProperties();
            }
        }

        private bool ShouldSkipProperty(SerializedProperty property)
        {
            return property.name.Equals("m_Script", System.StringComparison.Ordinal) || !PropertyUtility.IsVisible(property);
        }
    }
}
#endif
