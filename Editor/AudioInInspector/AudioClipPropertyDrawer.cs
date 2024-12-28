#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.AudioUtil
{
    [CustomPropertyDrawer(typeof(AudioClip))]
    public class AudioClipPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(prop);
        }

        private Dictionary<ButtonState, Action<SerializedProperty, AudioClip>> _audioButtonStates =
            new()
            {
                { ButtonState.Play, Play },
                { ButtonState.Stop, Stop },
            };

        private enum ButtonState
        {
            Play,
            Stop
        }

        private static string CurrentClip;
        private static GUIStyle tempStyle = new GUIStyle();


        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, prop);

            if (prop.objectReferenceValue != null)
            {
                float totalWidth = position.width;
                position.width = totalWidth - (totalWidth / 3.5f);
                EditorGUI.PropertyField(position, prop, label, true);

                position.x += position.width;
                position.width = totalWidth / 3.5f;
                position.height += 2f;
                DrawButton(position, prop);
            }
            else
            {
                EditorGUI.PropertyField(position, prop, label, true);
            }

            EditorGUI.EndProperty();
        }

        private void DrawButton(Rect position, SerializedProperty prop)
        {
            if (prop.objectReferenceValue != null)
            {
                position.x += 4;
                position.width -= 5;

                AudioClip clip = prop.objectReferenceValue as AudioClip;

                Rect buttonRect = new Rect(position);
                buttonRect.width = 30;
                buttonRect.height += 2;

                bool guiEnabledCache = GUI.enabled;
                GUI.enabled = true;

                Rect waveformRect = new Rect(position);
                waveformRect.x += 22;
                waveformRect.width -= 25;
                waveformRect.height += 2;
                if (Event.current.type == EventType.Repaint)
                {
                    Texture2D waveformTexture = AssetPreview.GetAssetPreview(prop.objectReferenceValue);
                    if (waveformTexture != null)
                    {
                        GUI.color = Color.white;
                        tempStyle.normal.background = waveformTexture;
                        tempStyle.Draw(waveformRect, GUIContent.none, false, false, false, false);
                        GUI.color = Color.white;
                    }
                }

                bool isPlaying = AudioUtility.IsClipPlaying(clip) && (CurrentClip == prop.propertyPath);
                string buttonText = "";
                Action<SerializedProperty, AudioClip> buttonAction;
                if (isPlaying)
                {
                    EditorUtility.SetDirty(prop.serializedObject.targetObject);
                    buttonAction = GetStateInfo(ButtonState.Stop, out buttonText);

                    Rect progressRect = new Rect(waveformRect);
                    float percentage = (float)AudioUtility.GetClipSamplePosition(clip) /
                                       AudioUtility.GetSampleCount(clip);
                    float width = progressRect.width * percentage;
                    progressRect.width = Mathf.Clamp(width, 6, width);
                    GUI.Box(progressRect, "", "SelectionRect");
                }
                else
                {
                    buttonAction = GetStateInfo(ButtonState.Play, out buttonText);
                }

                if (GUI.Button(buttonRect, buttonText))
                {
                    AudioUtility.StopAllClips();
                    buttonAction(prop, clip);
                }

                GUI.enabled = guiEnabledCache;
            }
        }

        private static void Play(SerializedProperty prop, AudioClip clip)
        {
            CurrentClip = prop.propertyPath;
            AudioUtility.PlayClip(clip);
        }

        private static void Stop(SerializedProperty prop, AudioClip clip)
        {
            CurrentClip = "";
            AudioUtility.StopAllClips();
        }

        private Action<SerializedProperty, AudioClip> GetStateInfo(ButtonState state, out string buttonText)
        {
            switch (state)
            {
                case ButtonState.Play:
                    buttonText = "►";
                    break;
                case ButtonState.Stop:
                    buttonText = "■";
                    break;
                default:
                    buttonText = "?";
                    break;
            }

            return _audioButtonStates[state];
        }
    }
}
#endif
