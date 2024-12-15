using System;

namespace RDTools.AutoAttach.Utils
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Determines whether a type implements or inherits from another type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="to">The target type to compare against.</param>
        /// <returns>True if the type implements or inherits from the target type, otherwise false.</returns>
        public static bool ImplementsOrInherits(this Type type, Type to)
        {
            return to.IsAssignableFrom(type);
        }

        /// <summary>
        /// Determines whether a type can be instantiated.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the type is instantiable, otherwise false.</returns>
        public static bool IsInstantiable(this Type type)
        {
            return !type.IsInterface && !type.IsGenericTypeDefinition && !type.IsAbstract;
        }

        /// <summary>
        /// Attempts to get the closest base generic type of a given type.
        /// </summary>
        /// <param name="type">The type to analyze.</param>
        /// <param name="baseGenericType">The resulting base generic type, if found.</param>
        /// <returns>True if a base generic type is found, otherwise false.</returns>
        public static bool TryGetBaseGeneric(this Type type, out Type baseGenericType)
        {
            baseGenericType = type;

            while (baseGenericType != null)
            {
                baseGenericType = baseGenericType.BaseType;
                if (baseGenericType?.IsGenericType == true)
                {
                    return true;
                }
            }

            return false;
        }
    }
}