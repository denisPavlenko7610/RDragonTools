#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnityEditor.AudioUtils
{
    [CustomPropertyDrawer(typeof(AudioClip))]
    public class AudioClipPropertyDrawer : PropertyDrawer
    {
        private enum PlaybackState
        {
            Play,
            Stop
        }

        private static readonly GUIStyle WaveformStyle = new GUIStyle();
        private static string currentClipPath;

        private readonly Dictionary<PlaybackState, Action<SerializedProperty, AudioClip>> playbackActions =
            new()
            {
                { PlaybackState.Play, PlayClip },
                { PlaybackState.Stop, StopClip },
            };

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            if (property.objectReferenceValue != null)
            {
                DrawAudioClipField(position, property, label);
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, true);
            }

            EditorGUI.EndProperty();
        }

        private void DrawAudioClipField(Rect position, SerializedProperty property, GUIContent label)
        {
            float totalWidth = position.width;
            position.width = totalWidth - (totalWidth / 3.5f);
            EditorGUI.PropertyField(position, property, label, true);

            position.x += position.width;
            position.width = totalWidth / 3.5f;
            position.height += 2f;
            DrawPlaybackControls(position, property);
        }

        private void DrawPlaybackControls(Rect position, SerializedProperty property)
        {
            if (property.objectReferenceValue is not AudioClip clip) return;

            position.x += 4;
            position.width -= 5;

            Rect buttonRect = new(position) { width = 30, height = position.height + 2 };
            Rect waveformRect = new(position) { x = position.x + 22, width = position.width - 25, height = position.height + 2 };

            bool guiEnabledCache = GUI.enabled;
            GUI.enabled = true;

            if (Event.current.type == EventType.Repaint)
            {
                DrawWaveform(waveformRect, property);
            }

            bool isPlaying = AudioUtility.IsClipPlaying(clip) && (currentClipPath == property.propertyPath);
            string buttonText = GetButtonText(isPlaying);
            Action<SerializedProperty, AudioClip> buttonAction = playbackActions[isPlaying ? PlaybackState.Stop : PlaybackState.Play];

            if (isPlaying)
            {
                DrawPlaybackProgress(waveformRect, clip);
            }

            if (GUI.Button(buttonRect, buttonText))
            {
                AudioUtility.StopAllClips();
                buttonAction(property, clip);
            }

            GUI.enabled = guiEnabledCache;
        }

        private void DrawWaveform(Rect waveformRect, SerializedProperty property)
        {
            Texture2D waveformTexture = AssetPreview.GetAssetPreview(property.objectReferenceValue);
            if (waveformTexture == null) return;

            GUI.color = Color.white;
            WaveformStyle.normal.background = waveformTexture;
            WaveformStyle.Draw(waveformRect, GUIContent.none, false, false, false, false);
            GUI.color = Color.white;
        }

        private void DrawPlaybackProgress(Rect waveformRect, AudioClip clip)
        {
            Rect progressRect = new(waveformRect)
            {
                width = Mathf.Clamp(waveformRect.width * GetPlaybackProgress(clip), 6, waveformRect.width)
            };
            GUI.Box(progressRect, GUIContent.none, "SelectionRect");
        }

        private static float GetPlaybackProgress(AudioClip clip)
        {
            return (float)AudioUtility.GetClipSamplePosition(clip) / AudioUtility.GetSampleCount(clip);
        }

        private static string GetButtonText(bool isPlaying)
        {
            return isPlaying ? "■" : "►";
        }

        private static void PlayClip(SerializedProperty property, AudioClip clip)
        {
            currentClipPath = property.propertyPath;
            AudioUtility.PlayClip(clip);
        }

        private static void StopClip(SerializedProperty property, AudioClip clip)
        {
            currentClipPath = string.Empty;
            AudioUtility.StopAllClips();
        }
    }
}
#endif
