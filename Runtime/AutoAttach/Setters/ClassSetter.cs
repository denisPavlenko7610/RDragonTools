using System;
using System.Collections;
using RDTools.AutoAttach.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RDTools.AutoAttach.Setters
{
    public class ClassSetter : SetterBase
    {
        public override int Order => 10000;

        public override bool Compatible(Type value)
        {
            return value.IsClass && !value.ImplementsOrInherits(typeof(ICollection));
        }

        public override bool TrySetField(Component target, object context, object currentValue, Type fieldType, 
            AttachAttribute attribute, out object newValue)
        {
            // Early return for ReadOnly Attribute or null currentValue
            if (!attribute.ReadOnly && currentValue != null)
            {
                if (currentValue is Object obj && obj)
                {
                    newValue = null;
                    return false;
                }

                newValue = null;
                return false;
            }

            // Get the new value from the target, context, and fieldType
            newValue = GetComponent(target, context, fieldType, attribute);
            return !Equals(currentValue, newValue);
        }
    }
}