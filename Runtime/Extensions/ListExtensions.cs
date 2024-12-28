using System;
using System.Collections;
using System.Collections.Generic;

namespace RDTools.Runtime
{
    public static class ListExtensions
    {
        public static T Random<T>(this IList<T> list) => list[UnityEngine.Random.Range(0, list.Count)];

        public static T RemoveRandom<T>(this IList<T> list)
        {
            int indexToRemoveAt = UnityEngine.Random.Range(0, list.Count);
            var item = list[indexToRemoveAt];

            list.RemoveAt(indexToRemoveAt);

            return item;
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            var rng = new Random();

            for (int i = list.Count - 1; i > 1; i--)
            {
                int k = rng.Next(i);
                var value = list[k];
                list[k] = list[i];
                list[i] = value;
            }
        }

        public static bool IsNullOrEmpty(this IList list) => list == null || list.Count == 0;

        public static bool IsNullOrEmpty<T>(this IList<T> list) => list == null || list.Count == 0;
    }
}