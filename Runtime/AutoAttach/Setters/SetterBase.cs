using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RDTools.AutoAttach.Setters
{
    public abstract class SetterBase
    {
        private static readonly List<Object> Buffer = new();

        public virtual int Order => 0;

        public abstract bool Compatible(Type value);

        public static Object GetComponent(Component target, object context, Type elementType, AttachAttribute attribute)
        {
            return attribute.IsFiltered 
                ? GetFilteredComponent(target, context, elementType, attribute) 
                : GetUnfilteredComponent(target, elementType, attribute);
        }

        public static IReadOnlyList<Object> GetComponents(Component target, object context, Type elementType, AttachAttribute attribute)
        {
            Buffer.Clear();
            var rawComponents = GetUnfilteredComponents(target, elementType, attribute.Type);

            if (!attribute.IsFiltered)
                return rawComponents;

            foreach (var component in rawComponents)
            {
                if (attribute.Filter(context, component))
                    Buffer.Add(component);
            }

            return Buffer;
        }

        public virtual bool TrySetField(
            Component monoBehaviour,
            object context,
            object currentValue,
            Type fieldType,
            AttachAttribute attribute,
            out object newValue)
        {
            newValue = null;
            return false;
        }

        private static Object GetFilteredComponent(Component target, object context, Type elementType, AttachAttribute attribute)
        {
            var components = GetUnfilteredComponents(target, elementType, attribute.Type);
            foreach (var component in components)
            {
                if (attribute.Filter(context, component))
                    return component;
            }

            return null;
        }

        private static Object GetUnfilteredComponent(Component target, Type elementType, AttachAttribute attribute)
        {
            return attribute.Type switch
            {
                Attach.Child => target.GetComponentInChildren(elementType, true),
                Attach.Parent => target.GetComponentInParent(elementType),
                Attach.Scene => Object.FindObjectOfType(elementType, true),
                Attach.Default => EnsureComponent(target, elementType, attribute),
                _ => null
            };
        }

        private static IReadOnlyList<Object> GetUnfilteredComponents(Component target, Type elementType, Attach attachType)
        {
            return attachType switch
            {
                Attach.Child => target.GetComponentsInChildren(elementType, true),
                Attach.Parent => target.GetComponentsInParent(elementType, true),
                Attach.Scene => Object.FindObjectsOfType(elementType, true),
                _ => target.GetComponents(elementType)
            };
        }

        private static Object EnsureComponent(Component target, Type type, AttachAttribute attribute)
        {
            var component = target.GetComponent(type);
            if (attribute is AttachOrAddAttribute && component == null)
                component = target.gameObject.AddComponent(type);

            return component;
        }
    }
}
