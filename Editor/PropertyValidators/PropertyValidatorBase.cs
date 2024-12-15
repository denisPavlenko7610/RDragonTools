#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;

namespace RDTools.Editor
{
    // Base class for property validation
    public abstract class PropertyValidatorBase
    {
        public abstract void ValidateProperty(SerializedProperty property);
    }

    public static class ValidatorAttributeExtensions
    {
        // Cached validators for each Attribute type
        private static readonly Dictionary<Type, PropertyValidatorBase> ValidatorsByAttributeType = new()
        {
            { typeof(MinValueAttribute), new MinValuePropertyValidator() },
            { typeof(MaxValueAttribute), new MaxValuePropertyValidator() },
            { typeof(ValidateInputAttribute), new ValidateInputPropertyValidator() }
        };

        // Get the corresponding validator for a given Attribute
        public static PropertyValidatorBase GetValidator(this ValidatorAttribute attribute)
        {
            // Return the validator if found, otherwise return null
            ValidatorsByAttributeType.TryGetValue(attribute.GetType(), out var validator);
            return validator;
        }
    }
}
#endif