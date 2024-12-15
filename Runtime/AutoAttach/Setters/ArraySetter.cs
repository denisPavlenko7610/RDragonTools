#if UNITY_EDITOR
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RDTools.AutoAttach.Setters
{
    public class ArraySetter : SetterBase
    {
        public override int Order => -100;

        public override bool Compatible(Type value)
        {
            return value.IsArray;
        }

        [SuppressMessage("ReSharper", "CoVariantArrayConversion")]
        public override bool TrySetField(Component target, object context, object currentValue, Type fieldType,
            AttachAttribute attribute, out object newValue)
        {
            Type elementType = fieldType.GetElementType();
            var componentArray = GetComponents(target, context, elementType, attribute);
            var prevArray = (Array)currentValue;

            // Check if the array is read-only or contains valid values
            if (!attribute.ReadOnly && prevArray?.Length > 0 && ((object[])prevArray).Any(x => x != null))
            {
                newValue = null;
                return false;
            }

            // Determine whether to reuse the previous array or create a new one
            int newArrayLength = componentArray.Count;
            Array newArray = prevArray?.Length == newArrayLength
                ? prevArray
                : Array.CreateInstance(elementType, newArrayLength);

            bool hasNewValues = false;
            for (int i = 0; i < newArray.Length; i++)
            {
                Object component = componentArray[i];

                // Skip if the value at the current index is the same
                if (ReferenceEquals(newArray.GetValue(i), component))
                    continue;

                newArray.SetValue(component, i);
                hasNewValues = true;
            }

            newValue = newArray;
            return hasNewValues;
        }
    }
}
#endif