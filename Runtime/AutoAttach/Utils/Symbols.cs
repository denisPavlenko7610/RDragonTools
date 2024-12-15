using UnityEditor;
using UnityEditor.Build;

namespace RDTools.AutoAttach.Utils
{
    public static class Symbols
    {
        public static void AddSymbol(string define)
        {
#if UNITY_EDITOR
            var buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);
            var defines = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);

            if (!defines.Contains(define))
            {
                var updatedDefines = string.IsNullOrEmpty(defines) ? define : $"{defines};{define}";
                PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, updatedDefines);
            }
#endif
        }
    }
}