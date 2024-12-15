#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using RDTools.AutoAttach.Setters;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RDTools.AutoAttach.Editor
{
    [EditorTool(nameof(AutoAttachTool), typeof(MonoBehaviour))]
    internal partial class AutoAttachTool : EditorTool
    {
        private static readonly Dictionary<Type, FieldData[]> Cache = new();
        private static readonly List<SetterBase> Setters = new();

        public static int MaxDepth => 7;

        private void OnEnable()
        {
            foreach (Object target in targets)
            {
                if (target is MonoBehaviour monoBehaviour)
                {
                    ApplySettings(monoBehaviour);
                }
            }
        }

        /// <summary>
        /// Applies the cached settings to the given MonoBehaviour component.
        /// </summary>
        /// <param name="component">The MonoBehaviour component.</param>
        /// <returns>True if any settings were applied; otherwise, false.</returns>
        private static bool ApplySettings(Component component)
        {
            if (!Cache.TryGetValue(component.GetType(), out var fieldDataArray))
                return false;

            int fieldsSetCount = 0;

            foreach (FieldData fieldData in fieldDataArray)
            {
                if (!TryFindSetter(fieldData.Field.FieldType, out var setter))
                    continue;

                object context = fieldData.GetContext(component);
                fieldData.Attribute.BeforeSet(context);

                if (setter.TrySetField(
                        component,
                        context,
                        fieldData.Field.GetValue(context),
                        fieldData.Field.FieldType,
                        fieldData.Attribute,
                        out var newValue))
                {
                    fieldData.Field.SetValue(context, newValue);
                    fieldsSetCount++;
                }

                fieldData.Attribute.AfterSet(context);
            }

            if (fieldsSetCount > 0)
            {
                EditorUtility.SetDirty(component);
            }

            return fieldsSetCount > 0;
        }

        /// <summary>
        /// Attempts to find a compatible setter for the specified type.
        /// </summary>
        /// <param name="type">The field type.</param>
        /// <param name="setter">The compatible setter if found; otherwise, null.</param>
        /// <returns>True if a compatible setter is found; otherwise, false.</returns>
        private static bool TryFindSetter(Type type, out SetterBase setter)
        {
            foreach (var registeredSetter in Setters)
            {
                if (registeredSetter.Compatible(type))
                {
                    setter = registeredSetter;
                    return true;
                }
            }

            setter = null;
            return false;
        }
    }
}
#endif
