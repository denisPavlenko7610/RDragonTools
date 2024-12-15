#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace RDTools.Editor
{
    [CustomPropertyDrawer(typeof(HorizontalLineAttribute))]
    public class HorizontalLineDecoratorDrawer : DecoratorDrawer
    {
        // Override to calculate the height of the property including the horizontal line
        public override float GetHeight()
        {
            var lineAttr = attribute as HorizontalLineAttribute;
            return EditorGUIUtility.singleLineHeight + (lineAttr?.Height ?? 1);
        }

        // Override to draw the horizontal line in the property drawer
        public override void OnGUI(Rect position)
        {
            var rect = EditorGUI.IndentedRect(position);
            rect.y += EditorGUIUtility.singleLineHeight / 3.0f;

            if (attribute is HorizontalLineAttribute lineAttr)
            {
                RDEditorGUI.HorizontalLine(rect, lineAttr.Height, lineAttr.Color.GetColor());
            }
        }
    }
}
#endif