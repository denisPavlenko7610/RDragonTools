#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace RDTools.Editor
{
    /// <summary>
    /// Custom property drawer for the AllowNestingAttribute.
    /// Allows nested properties to be displayed in the Unity Inspector.
    /// </summary>
    [CustomPropertyDrawer(typeof(AllowNestingAttribute))]
    public class AllowNestingPropertyDrawer : PropertyDrawerBase
    {
        /// <summary>
        /// Draws the property in the Unity Inspector.
        /// </summary>
        /// <param name="rect">The rectangle on the Inspector where the property will be drawn.</param>
        /// <param name="property">The serialized property to draw.</param>
        /// <param name="label">The label of the property.</param>
        protected override void OnGUI_Internal(Rect rect, SerializedProperty property, GUIContent label)
        {
            if (property == null)
            {
                Debug.LogError("SerializedProperty is null. Unable to draw the property.");
                return;
            }

            EditorGUI.BeginProperty(rect, label, property);

            // Draw the property field with nesting support
            EditorGUI.PropertyField(rect, property, label, includeChildren: true);

            EditorGUI.EndProperty();
        }
    }
}
#endif