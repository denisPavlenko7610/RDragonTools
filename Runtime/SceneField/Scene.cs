using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#endif

namespace RDTools
{
    /// <summary>
    /// Makes it possible to assign a scene asset in the inspector and load the scene data in a build.
    /// </summary>
    [Serializable]
    public class Scene
#if UNITY_EDITOR
        : ISerializationCallbackReceiver
#endif
    {
        #region Parameters

#if UNITY_EDITOR
        [SerializeField] private SceneAsset sceneAsset;
        [FormerlySerializedAs("logErrorIfNotInBuild")] [SerializeField] private bool isRequired = false;
#endif

        [SerializeField] private int buildIndex = -1; // Default to -1 to indicate no scene assigned.

        #endregion

        /// <summary>
        /// Gets the scene build index. Returns -1 if no scene is assigned or it's not added to builds.
        /// Avoid using this within <see cref="ISerializationCallbackReceiver"/> methods.
        /// </summary>
        public int BuildIndex
        {
            get
            {
#if UNITY_EDITOR
                UpdateBuildIndexIfNeeded();
#endif
                return buildIndex;
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Called before serialization to update the build index and check if required scenes are missing.
        /// </summary>
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            UpdateBuildIndexIfNeeded();
            if (isRequired && buildIndex < 0)
                BuildProcessor.RegisterMissingRequiredScene(sceneAsset);
        }

        /// <summary>
        /// Implementation of <see cref="ISerializationCallbackReceiver.OnAfterDeserialize"/>.
        /// </summary>
        void ISerializationCallbackReceiver.OnAfterDeserialize() { }

        /// <summary>
        /// Updates the build index for the assigned scene asset.
        /// </summary>
        private void UpdateBuildIndexIfNeeded()
        {
            if (sceneAsset != null)
                buildIndex = GetSceneBuildIndex(sceneAsset);
            
            if (isRequired && buildIndex < 0)
            {
                LogMissingSceneError();
            }
        }

        /// <summary>
        /// Logs an error when a required scene is not added to the build settings.
        /// </summary>
        private void LogMissingSceneError()
        {
            if (sceneAsset != null)
            {
                Debug.LogError($"RDTools.Scene: The following scene is assigned as required, but isn't added to builds: {AssetDatabase.GetAssetPath(sceneAsset)}");
            }
            else
            {
                Debug.LogError("RDTools.Scene: A required scene field is marked as required, but no scene is assigned.");
            }
        }

        #region Build Processor (Editor only)
        class BuildProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
        {
            private static HashSet<SceneAsset> missingRequiredSceneAssets = new();
            private static bool isRequiredSceneUnassigned;

            public static void RegisterMissingRequiredScene(SceneAsset sceneAsset)
            {
                if (sceneAsset != null)
                    missingRequiredSceneAssets.Add(sceneAsset);
                else
                    isRequiredSceneUnassigned = true;
            }

            public int callbackOrder => 0;

            public void OnPreprocessBuild(BuildReport report)
            {
                ClearBuildCache();
            }

            public void OnPostprocessBuild(BuildReport report)
            {
                ValidateBuild();
            }

            private static void ClearBuildCache()
            {
                missingRequiredSceneAssets.Clear();
                isRequiredSceneUnassigned = false;
            }

            private static void ValidateBuild()
            {
                string errorMessage = null;

                if (isRequiredSceneUnassigned)
                    errorMessage += "  - A required scene field doesn't have an assigned scene.";

                if (missingRequiredSceneAssets.Count > 0)
                {
                    errorMessage = string.IsNullOrEmpty(errorMessage) ? "" : errorMessage + "\n";
                    errorMessage += "  - The following scenes are required but are not added to builds:";

                    foreach (var sceneAsset in missingRequiredSceneAssets)
                        errorMessage += $"\n    - {AssetDatabase.GetAssetPath(sceneAsset)}";

                    missingRequiredSceneAssets.Clear();
                }

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    errorMessage = $"RDTools.Scene: The following errors have been found:\n{errorMessage}";
                    throw new BuildFailedException(errorMessage);
                }
            }
        }
        #endregion

#if UNITY_EDITOR
        private static Dictionary<SceneAsset, int> cachedBuildIndexes = new();

        static Scene()
        {
            // Automatically refresh build indexes whenever scenes change in the build settings.
            EditorBuildSettings.sceneListChanged -= UpdateCachedBuildIndexes;
            EditorBuildSettings.sceneListChanged += UpdateCachedBuildIndexes;
            UpdateCachedBuildIndexes();
        }

        /// <summary>
        /// Updates the cached build indexes based on the scenes in the build settings.
        /// </summary>
        private static void UpdateCachedBuildIndexes()
        {
            cachedBuildIndexes.Clear();
            int buildIndex = -1;

            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                {
                    buildIndex++;
                    var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path);
                    if (sceneAsset != null)
                        cachedBuildIndexes[sceneAsset] = buildIndex;
                }
            }
        }

        /// <summary>
        /// Retrieves the build index of the specified scene asset.
        /// </summary>
        /// <param name="sceneAsset">The scene asset to retrieve the build index for.</param>
        /// <returns>The build index of the scene asset, or -1 if not found.</returns>
        private static int GetSceneBuildIndex(SceneAsset sceneAsset)
        {
            return sceneAsset != null && cachedBuildIndexes.TryGetValue(sceneAsset, out var index) ? index : -1;
        }

        /// <summary>
        /// ** Editor-only ** Gets the scene asset assigned in the inspector.
        /// </summary>
        public SceneAsset EditorSceneAsset => sceneAsset;

#endif
    }
}
#endif
