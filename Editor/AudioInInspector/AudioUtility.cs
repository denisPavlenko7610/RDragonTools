#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEngine;

namespace UnityEditor
{
    public static class AudioUtility
    {
        private static readonly Type AudioUtilClass;
        private static readonly Assembly UnityEditorAssembly;

        static AudioUtility()
        {
            UnityEditorAssembly = typeof(AudioImporter).Assembly;
            AudioUtilClass = UnityEditorAssembly.GetType("UnityEditor.AudioUtil");
        }

        public static void PlayClip(AudioClip clip, int startSample = 0, bool loop = false)
        {
            InvokeAudioUtilMethod("PlayPreviewClip", clip, startSample, loop);
            SetClipSamplePosition(clip, startSample);
        }

        public static bool IsClipPlaying(AudioClip clip)
        {
            return (bool)InvokeAudioUtilMethod("IsPreviewClipPlaying", clip);
        }

        public static void StopAllClips()
        {
            InvokeAudioUtilMethod("StopAllPreviewClips");
        }

        public static int GetClipSamplePosition(AudioClip clip)
        {
            return (int)InvokeAudioUtilMethod("GetPreviewClipSamplePosition", clip);
        }

        private static void SetClipSamplePosition(AudioClip clip, int samplePosition)
        {
            InvokeAudioUtilMethod("SetPreviewClipSamplePosition", clip, samplePosition);
        }

        public static int GetSampleCount(AudioClip clip)
        {
            return (int)InvokeAudioUtilMethod("GetSampleCount", clip);
        }

        private static object InvokeAudioUtilMethod(string methodName, params object[] parameters)
        {
            MethodInfo method = AudioUtilClass.GetMethod(
                methodName,
                BindingFlags.Static | BindingFlags.Public
            );

            if (method == null)
            {
                Debug.LogError($"AudioUtil method '{methodName}' not found.");
                return null;
            }

            return method.Invoke(null, parameters);
        }
    }
}
#endif
