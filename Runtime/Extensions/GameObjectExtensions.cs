using UnityEngine;

namespace RDTools.Runtime
{
    public static class GameObjectExtensions
    {
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            if (gameObject.TryGetComponent<T>(out T requestedComponent))
            {
                return requestedComponent;
            }

            return gameObject.AddComponent<T>();
        }

        public static void SetParent(this GameObject gameObject, GameObject parent) =>
            gameObject.transform.SetParent(parent.transform);

        public static void SetParent(this GameObject gameObject, GameObject parent, bool worldPositionStays) =>
            gameObject.transform.SetParent(parent.transform, worldPositionStays);
    }
}