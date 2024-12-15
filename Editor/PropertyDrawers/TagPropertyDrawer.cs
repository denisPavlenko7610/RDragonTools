#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RDTools.Editor
{
    [CustomPropertyDrawer(typeof(TagAttribute))]
    public class TagPropertyDrawer : PropertyDrawerBase
    {
        protected override float GetPropertyHeight_Internal(SerializedProperty property, GUIContent label)
        {
            float height = GetPropertyHeight(property);
            return property.propertyType == SerializedPropertyType.String ? height : height + GetHelpBoxHeight();
        }

        protected override void OnGUI_Internal(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            if (property.propertyType == SerializedPropertyType.String)
            {
                // Prepare the tag list
                List<string> tagList = GetTagList();

                string currentTag = property.stringValue;
                int currentIndex = tagList.IndexOf(currentTag);

                // If no matching tag is found, default to (None)
                if (currentIndex == -1) currentIndex = 0;

                // Draw the popup with the current selected index
                int selectedIndex = EditorGUI.Popup(rect, label.text, currentIndex, tagList.ToArray());

                // Only update if the selected value has changed
                string newTag = selectedIndex > 0 ? tagList[selectedIndex] : string.Empty;
                if (property.stringValue != newTag)
                {
                    property.stringValue = newTag;
                }
            }
            else
            {
                // Handle non-string properties
                string message = $"{typeof(TagAttribute).Name} supports only string fields";
                DrawDefaultPropertyAndHelpBox(rect, property, message, MessageType.Warning);
            }

            EditorGUI.EndProperty();
        }

        private List<string> GetTagList()
        {
            // Generate the list of tags, including default options
            List<string> tagList = new List<string>
            {
                "(None)",
                "Untagged"
            };
            tagList.AddRange(UnityEditorInternal.InternalEditorUtility.tags);

            return tagList;
        }
    }
}
#endif
