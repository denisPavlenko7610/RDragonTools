#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace RDTools.Editor
{
    [CustomPropertyDrawer(typeof(InfoBoxAttribute))]
    public class InfoBoxDecoratorDrawer : DecoratorDrawer
    {
        private InfoBoxAttribute InfoBoxAttribute => (InfoBoxAttribute)attribute;

        public override float GetHeight()
        {
            return CalculateHelpBoxHeight();
        }

        public override void OnGUI(Rect rect)
        {
            float indentLength = RDEditorGUI.GetIndentLength(rect);
            Rect infoBoxRect = new Rect(
                rect.x + indentLength,
                rect.y,
                rect.width - indentLength,
                CalculateHelpBoxHeight());

            DrawInfoBox(infoBoxRect);
        }

        private float CalculateHelpBoxHeight()
        {
            float minHeight = EditorGUIUtility.singleLineHeight * 2.0f;
            float desiredHeight = GUI.skin.box.CalcHeight(new GUIContent(InfoBoxAttribute.Text), EditorGUIUtility.currentViewWidth);
            return Mathf.Max(minHeight, desiredHeight);
        }

        private void DrawInfoBox(Rect rect)
        {
            MessageType messageType = GetMessageType(InfoBoxAttribute.Type);
            RDEditorGUI.HelpBox(rect, InfoBoxAttribute.Text, messageType);
        }

        private MessageType GetMessageType(EInfoBoxType infoBoxType)
        {
            return infoBoxType switch
            {
                EInfoBoxType.Normal => MessageType.Info,
                EInfoBoxType.Warning => MessageType.Warning,
                EInfoBoxType.Error => MessageType.Error,
                _ => MessageType.None,
            };
        }
    }
}
#endif