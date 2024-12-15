using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class DestroyChildrenEditor : Editor
{
    [MenuItem("Custom/Destroy All Children")]
    private static void DestroyAllChildrenMenuItem()
    {
        GameObject selectedObject = Selection.activeGameObject;

        if (selectedObject != null)
        {
            DestroyAllChildren(selectedObject.transform);
            Debug.Log($"Successfully destroyed all children of: {selectedObject.name}");
        }
        else
        {
            Debug.LogWarning("No GameObject selected. Please select a GameObject to destroy its children.");
        }
    }

    private static void DestroyAllChildren(Transform parent)
    {
        if (parent == null) return;

        int childCount = parent.childCount;
        if (childCount == 0)
        {
            Debug.LogWarning($"The GameObject {parent.name} has no children to destroy.");
            return;
        }

        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = parent.GetChild(i);
            if (child != null)
            {
                DestroyImmediate(child.gameObject);
                Debug.Log($"Destroyed child: {child.name}");
            }
            else
            {
                Debug.LogWarning("Encountered a null child during destruction.");
            }
        }
    }
}
#endif