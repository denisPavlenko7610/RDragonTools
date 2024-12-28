using System.Linq;
using UnityEngine;

namespace RDTools.Runtime
{
    public static class TransformExtensions
    {
        public static void DestroyChildren(this Transform transform)
        {
            foreach (Transform child in transform)
            {
                Object.Destroy(child.gameObject);
            }
        }
        
        public static void DestroyChildrenImmediately(this Transform transform)
        {
            var tempList = transform.Cast<Transform>().ToList();
            foreach (Transform child in tempList)
            {
                Object.DestroyImmediate(child.gameObject);
            }
        }
    }
}
