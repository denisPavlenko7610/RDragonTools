using System;
using System.Collections;
using System.Linq;
using RDTools.AutoAttach.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RDTools.AutoAttach.Setters
{
    public class ListSetter : SetterBase
    {
        public override int Order => -90;

        public override bool Compatible(Type value)
        {
            return value.ImplementsOrInherits(typeof(IList));
        }

        public override bool TrySetField(Component target, object context, object currentValue, Type fieldType, AttachAttribute attribute, out object newValue)
        {
            newValue = null;
            if (currentValue is not IList list)
            {
                return false;
            }

            // If the list is not read-only and contains non-null elements, return false
            if (!attribute.ReadOnly && list.Count > 0 && list.Cast<object>().Any(x => x != null))
            {
                return false;
            }
            
            newValue = list;

            // Get the generic element type of the list
            Type elementType = fieldType.GetElementType() ?? fieldType.GenericTypeArguments.FirstOrDefault();

            if (elementType == null)
            {
                return false;
            }

            // Retrieve components of the specified type
            var components = GetComponents(target, context, elementType, attribute);
            bool updatedValues = false;

            // Update the list with components
            for (int i = 0; i < components.Count; i++)
            {
                Object value = components[i];

                if (list.Count > i)
                {
                    if (!ReferenceEquals(list[i], value))
                    {
                        list[i] = value;
                        updatedValues = true;
                    }
                }
                else
                {
                    list.Add(value);
                    updatedValues = true;
                }
            }

            return updatedValues;
        }
    }
}
