#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using System;

namespace RDTools.Editor
{
    [CustomPropertyDrawer(typeof(ResizableTextAreaAttribute))]
    public class ResizableTextAreaPropertyDrawer : PropertyDrawerBase
    {
        protected override float GetPropertyHeight_Internal(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                float labelHeight = EditorGUIUtility.singleLineHeight;
                float textAreaHeight = CalculateTextAreaHeight(property.stringValue);
                return labelHeight + textAreaHeight;
            }
            
            return base.GetPropertyHeight(property) + GetHelpBoxHeight();
        }

        protected override void OnGUI_Internal(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            if (property.propertyType == SerializedPropertyType.String)
            {
                Rect labelRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(labelRect, label.text);

                EditorGUI.BeginChangeCheck();

                Rect textAreaRect = new Rect(
                    rect.x,
                    rect.y + EditorGUIUtility.singleLineHeight,
                    rect.width,
                    CalculateTextAreaHeight(property.stringValue)
                );

                string newValue = EditorGUI.TextArea(textAreaRect, property.stringValue);

                if (EditorGUI.EndChangeCheck())
                {
                    property.stringValue = newValue;
                }
            }
            else
            {
                string warningMessage = $"{nameof(ResizableTextAreaAttribute)} can only be used on string fields.";
                DrawDefaultPropertyAndHelpBox(rect, property, warningMessage, MessageType.Warning);
            }

            EditorGUI.EndProperty();
        }

        private int CountLines(string text)
        {
            if (string.IsNullOrEmpty(text)) return 1;
            string normalizedText = Regex.Replace(text, "\r\n|\n\r|\r|\n", Environment.NewLine);
            return normalizedText.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Length;
        }

        private float CalculateTextAreaHeight(string text)
        {
            int numberOfLines = CountLines(text);
            return (EditorGUIUtility.singleLineHeight - 3.0f) * numberOfLines + 3.0f;
        }
    }
}
#endif
