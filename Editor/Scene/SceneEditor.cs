#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace RDTools
{
    [CustomPropertyDrawer(typeof(Scene))]
    public class SceneEditor : PropertyDrawer
    {
        // Constants for separation
        private const float SceneInBuildSeparationLeft = 1;
        private const float SceneInBuildSeparationRight = 10;
        private const float SceneInBuildSeparationTotal = SceneInBuildSeparationLeft + SceneInBuildSeparationRight;

        // GUIContents for different states
        private readonly GUIContent sceneInBuildYesContent = new GUIContent("In build");
        private readonly GUIContent sceneInBuildNoContent = new GUIContent("Not in build");
        private readonly GUIContent sceneInBuildUnassignedContent = new GUIContent("Unassigned");
        private readonly GUIContent sceneInBuildMultipleContent = new GUIContent("—");
        private readonly GUIContent sceneIsRequiredContent = new GUIContent("Required", "Logs an error and fails the build if the scene is not added to builds");

        // GUIStyles
        private GUIStyle _sceneInBuildStyle;
        private GUIStyle SceneInBuildStyle => _sceneInBuildStyle ??= new GUIStyle(EditorStyles.miniLabel);

        private float _buildIndexWidth;
        private float BuildIndexWidth
        {
            get
            {
                if (_buildIndexWidth == 0)
                    SceneInBuildStyle.CalcMinMaxWidth(sceneInBuildNoContent, out _buildIndexWidth, out _);
                return _buildIndexWidth;
            }
        }

        private GUIStyle _sceneIsRequiredStyleNormal;
        private GUIStyle SceneIsRequiredStyleNormal => _sceneIsRequiredStyleNormal ??= new GUIStyle(EditorStyles.miniLabel);

        private GUIStyle _sceneIsRequiredStylePrefabOverride;
        private GUIStyle SceneIsRequiredStylePrefabOverride => _sceneIsRequiredStylePrefabOverride ??= new GUIStyle(EditorStyles.miniBoldLabel);

        private float _sceneIsRequiredWidth;
        private float SceneIsRequiredWidth
        {
            get
            {
                if (_sceneIsRequiredWidth == 0)
                {
                    SceneIsRequiredStylePrefabOverride.CalcMinMaxWidth(sceneIsRequiredContent, out float min, out _);
                    _sceneIsRequiredWidth = min;

                    EditorStyles.toggle.CalcMinMaxWidth(GUIContent.none, out min, out _);
                    _sceneIsRequiredWidth += min;
                }

                return _sceneIsRequiredWidth;
            }
        }

        /// <summary>
        /// Override OnGUI for custom property rendering.
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Serialized properties for scene-related data
            SerializedProperty sceneAssetProp = property.FindPropertyRelative("sceneAsset");
            SerializedProperty buildIndexProp = property.FindPropertyRelative("buildIndex");
            SerializedProperty requiredProp = property.FindPropertyRelative("required");

            position.height = EditorGUIUtility.singleLineHeight;

            // Render Scene Asset field
            position.width -= BuildIndexWidth + SceneInBuildSeparationTotal + SceneIsRequiredWidth;
            using (new EditorGUI.PropertyScope(position, label, sceneAssetProp))
            {
                EditorGUI.PropertyField(position, sceneAssetProp, label);
            }

            // Render Scene In Build Status
            position.x += position.width + SceneInBuildSeparationLeft;
            position.width = BuildIndexWidth + SceneInBuildSeparationRight;
            RenderSceneInBuildStatus(position, sceneAssetProp, buildIndexProp, requiredProp);

            // Render Scene Required Toggle
            position.x += position.width;
            position.width = SceneIsRequiredWidth;
            RenderSceneRequiredToggle(position, requiredProp);
        }

        private void RenderSceneInBuildStatus(Rect position, SerializedProperty sceneAssetProp, SerializedProperty buildIndexProp, SerializedProperty requiredProp)
        {
            if (sceneAssetProp.hasMultipleDifferentValues)
            {
                GUI.Label(position, sceneInBuildMultipleContent, SceneInBuildStyle);
            }
            else if (sceneAssetProp.objectReferenceValue != null)
            {
                bool isInBuilds = buildIndexProp.intValue >= 0;
                Color prevColor = GUI.contentColor;

                if (!isInBuilds && requiredProp.boolValue)
                    GUI.contentColor *= Color.red;

                GUI.Label(position, isInBuilds ? sceneInBuildYesContent : sceneInBuildNoContent, SceneInBuildStyle);
                GUI.contentColor = prevColor;
            }
            else if (requiredProp.boolValue)
            {
                Color prevColor = GUI.contentColor;
                GUI.contentColor *= Color.red;
                GUI.Label(position, sceneInBuildUnassignedContent, SceneInBuildStyle);
                GUI.contentColor = prevColor;
            }
        }

        private void RenderSceneRequiredToggle(Rect position, SerializedProperty requiredProp)
        {
            using (new EditorGUI.PropertyScope(position, sceneIsRequiredContent, requiredProp))
            using (new EditorGUI.IndentLevelScope(-EditorGUI.indentLevel))
            using (var changeCheck = new EditorGUI.ChangeCheckScope())
            {
                EditorGUI.showMixedValue = requiredProp.hasMultipleDifferentValues;
                bool newValue = EditorGUI.ToggleLeft(position, sceneIsRequiredContent, requiredProp.boolValue,
                    requiredProp.prefabOverride && !requiredProp.hasMultipleDifferentValues
                        ? SceneIsRequiredStylePrefabOverride
                        : SceneIsRequiredStyleNormal);
                EditorGUI.showMixedValue = false;

                if (changeCheck.changed)
                    requiredProp.boolValue = newValue;
            }
        }
    }
}
#endif
