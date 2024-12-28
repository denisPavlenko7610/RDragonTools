using UnityEngine;
using UnityEditor;

public class DestroyChildrenEditor : Editor
{
    [MenuItem("Custom/Destroy All Children")]
    private static void DestroyAllChildrenMenuItem()
    {
        GameObject selectedObject = Selection.activeGameObject;

        if (selectedObject != null)
        {
            DestroyAllChildren(selectedObject.transform);
            Debug.Log("Destroyed all children of: " + selectedObject.name);
        }
        else
        {
            Debug.LogWarning("No GameObject selected.");
        }
    }

    private static void DestroyAllChildren(Transform parent)
    {
        int childCount = parent.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = parent.GetChild(i);
            DestroyImmediate(child.gameObject);
        }
    }
}