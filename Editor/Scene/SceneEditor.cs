#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace RDTools
{
    [CustomPropertyDrawer(typeof(Scene))]
    public class SceneEditor : PropertyDrawer
    {
        // Scene in build data:
        const float sceneInBuildSeparationLeft = 1;
        const float sceneInBuildSeparationRight = 10;
        const float sceneInBuildSeparationTotal = sceneInBuildSeparationLeft + sceneInBuildSeparationRight;

        GUIContent sceneInBuildYesContent = new GUIContent("In build");
        GUIContent sceneInBuildNoContent = new GUIContent("Not in build");
        GUIContent sceneInBuildUnassignedContent = new GUIContent("Unassigned");
        GUIContent sceneInBuildMultipleContent = new GUIContent("—");

        GUIStyle _sceneInBuildStyle;
        GUIStyle SceneInBuildStyle => _sceneInBuildStyle ?? (_sceneInBuildStyle = new GUIStyle(EditorStyles.miniLabel));

        float _buildIndexWidth;

        float BuildIndexWidth
        {
            get
            {
                if (_buildIndexWidth == 0)
                    SceneInBuildStyle.CalcMinMaxWidth(sceneInBuildNoContent, out _buildIndexWidth, out _);

                return _buildIndexWidth;
            }
        }


        // Scene is required data:
        GUIContent sceneIsRequiredContent = new GUIContent("Required",
            "Logs an error and fails the build if the scene is not added to builds");

        GUIStyle _sceneIsRequiredStyleNormal;

        GUIStyle SceneIsRequiredStyleNormal => _sceneIsRequiredStyleNormal ??= new GUIStyle(EditorStyles.miniLabel);

        GUIStyle _sceneIsRequiredStylePrefabOverride;

        GUIStyle SceneIsRequiredStylePrefabOverride =>
            _sceneIsRequiredStylePrefabOverride ??= new GUIStyle(EditorStyles.miniBoldLabel);

        float _sceneIsRequiredWidth;

        float SceneIsRequiredWidth
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
        /// Implementation of <see cref="PropertyDrawer.OnGUI(Rect, SerializedProperty, GUIContent)"/>.
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty sceneAssetProp = property.FindPropertyRelative("sceneAsset");
            SerializedProperty buildIndexProp = property.FindPropertyRelative("buildIndex");
            SerializedProperty requiredProp = property.FindPropertyRelative("required");

            position.height = EditorGUIUtility.singleLineHeight;

            // Scene asset:
            position.width -= BuildIndexWidth + sceneInBuildSeparationTotal + SceneIsRequiredWidth;

            using (new EditorGUI.PropertyScope(position, label, sceneAssetProp))
            {
                EditorGUI.PropertyField(position, sceneAssetProp, label);
            }

            // Is the scene in builds?:
            position.x += position.width + sceneInBuildSeparationLeft;
            position.width = BuildIndexWidth + sceneInBuildSeparationRight;

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
            
            // Scene required:
            position.x += position.width;
            position.width = SceneIsRequiredWidth;

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