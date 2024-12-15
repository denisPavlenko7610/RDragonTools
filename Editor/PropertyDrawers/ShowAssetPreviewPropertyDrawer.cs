#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace RDTools.Editor
{
    [CustomPropertyDrawer(typeof(ShowAssetPreviewAttribute))]
    public class ShowAssetPreviewPropertyDrawer : PropertyDrawerBase
    {
        protected override float GetPropertyHeight_Internal(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference)
                return GetPropertyHeight(property) + GetHelpBoxHeight();

            Texture2D previewTexture = GetAssetPreview(property);
            return previewTexture != null 
                ? GetPropertyHeight(property) + GetAssetPreviewSize(property).y 
                : GetPropertyHeight(property);
        }

        protected override void OnGUI_Internal(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                DrawPropertyField(rect, property, label);
                DrawAssetPreview(rect, property);
            }
            else
            {
                string message = $"{property.name} doesn't have an asset preview";
                DrawDefaultPropertyAndHelpBox(rect, property, message, MessageType.Warning);
            }

            EditorGUI.EndProperty();
        }

        private void DrawPropertyField(Rect rect, SerializedProperty property, GUIContent label)
        {
            Rect propertyRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(propertyRect, property, label);
        }

        private void DrawAssetPreview(Rect rect, SerializedProperty property)
        {
            Texture2D previewTexture = GetAssetPreview(property);
            if (previewTexture == null) return;

            Rect previewRect = new Rect(
                rect.x + RDEditorGUI.GetIndentLength(rect),
                rect.y + EditorGUIUtility.singleLineHeight,
                rect.width,
                GetAssetPreviewSize(property).y
            );

            GUI.Label(previewRect, previewTexture);
        }

        private Texture2D GetAssetPreview(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue != null
                ? AssetPreview.GetAssetPreview(property.objectReferenceValue)
                : null;
        }

        private Vector2 GetAssetPreviewSize(SerializedProperty property)
        {
            Texture2D previewTexture = GetAssetPreview(property);
            if (previewTexture == null) return Vector2.zero;

            ShowAssetPreviewAttribute attribute = PropertyUtility.GetAttribute<ShowAssetPreviewAttribute>(property);
            int targetWidth = attribute?.Width ?? ShowAssetPreviewAttribute.DefaultWidth;
            int targetHeight = attribute?.Height ?? ShowAssetPreviewAttribute.DefaultHeight;

            int width = Mathf.Clamp(targetWidth, 0, previewTexture.width);
            int height = Mathf.Clamp(targetHeight, 0, previewTexture.height);

            return new Vector2(width, height);
        }
    }
}
#endif
