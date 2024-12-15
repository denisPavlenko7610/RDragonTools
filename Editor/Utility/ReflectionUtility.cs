#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace RDTools.Editor
{
    public static class ReflectionUtility
    {
        // Generic method to get fields, properties, or methods
        private static IEnumerable<T> GetAllMembers<T>(object target, Func<T, bool> predicate, Func<Type, IEnumerable<T>> getMembers) where T : MemberInfo
        {
            if (target == null)
                yield break;

            List<Type> types = GetSelfAndBaseTypes(target);

            foreach (var type in types)
            {
                foreach (var member in getMembers(type).Where(predicate))
                {
                    yield return member;
                }
            }
        }

        public static IEnumerable<FieldInfo> GetAllFields(object target, Func<FieldInfo, bool> predicate)
        {
            return GetAllMembers(target, predicate, t => t.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly));
        }

        public static IEnumerable<PropertyInfo> GetAllProperties(object target, Func<PropertyInfo, bool> predicate)
        {
            return GetAllMembers(target, predicate, t => t.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly));
        }

        public static IEnumerable<MethodInfo> GetAllMethods(object target, Func<MethodInfo, bool> predicate)
        {
            return GetAllMembers(target, predicate, t => t.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly));
        }

        // Helper methods to get the first matching field, property, or method
        public static FieldInfo GetField(object target, string fieldName)
        {
            return target == null ? null : GetAllFields(target, f => f.Name.Equals(fieldName, StringComparison.Ordinal)).FirstOrDefault();
        }

        public static PropertyInfo GetProperty(object target, string propertyName)
        {
            return target == null ? null : GetAllProperties(target, p => p.Name.Equals(propertyName, StringComparison.Ordinal)).FirstOrDefault();
        }

        public static MethodInfo GetMethod(object target, string methodName)
        {
            return target == null ? null : GetAllMethods(target, m => m.Name.Equals(methodName, StringComparison.Ordinal)).FirstOrDefault();
        }

        // Get the element type of a list
        public static Type GetListElementType(Type listType)
        {
            return listType.IsGenericType ? listType.GetGenericArguments()[0] : listType.GetElementType();
        }

        /// <summary>
        /// Get type and all base types of target, sorted as following:
        /// <para />[target's type, base type, base's base type, ...]
        /// </summary>
        private static List<Type> GetSelfAndBaseTypes(object target)
        {
            if (target == null)
                return null;

            List<Type> types = new List<Type> { target.GetType() };
            Type baseType = types.Last().BaseType;

            while (baseType != null)
            {
                types.Add(baseType);
                baseType = baseType.BaseType;
            }

            return types;
        }
    }
}
#endif
