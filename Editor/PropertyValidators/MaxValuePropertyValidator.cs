#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace RDTools.Editor
{
    public class MaxValuePropertyValidator : PropertyValidatorBase
    {
        public override void ValidateProperty(SerializedProperty property)
        {
            MaxValueAttribute maxValueAttribute = PropertyUtility.GetAttribute<MaxValueAttribute>(property);

            if (maxValueAttribute == null)
            {
                Debug.LogWarning("MaxValueAttribute not found on property " + property.name, property.serializedObject.targetObject);
                return;
            }

            // Apply max value check based on property type
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    ApplyMaxValue(property.intValue, maxValueAttribute.MaxValue, value => property.intValue = value);
                    break;

                case SerializedPropertyType.Float:
                    ApplyMaxValue(property.floatValue, maxValueAttribute.MaxValue, value => property.floatValue = value);
                    break;

                case SerializedPropertyType.Vector2:
                    ApplyMaxValueToVector(property.vector2Value, maxValueAttribute.MaxValue, value => property.vector2Value = value);
                    break;

                case SerializedPropertyType.Vector3:
                    ApplyMaxValueToVector(property.vector3Value, maxValueAttribute.MaxValue, value => property.vector3Value = value);
                    break;

                case SerializedPropertyType.Vector4:
                    ApplyMaxValueToVector(property.vector4Value, maxValueAttribute.MaxValue, value => property.vector4Value = value);
                    break;

                case SerializedPropertyType.Vector2Int:
                    ApplyMaxValueToVectorInt(property.vector2IntValue, maxValueAttribute.MaxValue, value => property.vector2IntValue = value);
                    break;

                case SerializedPropertyType.Vector3Int:
                    ApplyMaxValueToVectorInt(property.vector3IntValue, maxValueAttribute.MaxValue, value => property.vector3IntValue = value);
                    break;

                default:
                    string warning = $"{maxValueAttribute.GetType().Name} can only be used on int, float, Vector or VectorInt fields. Property type: {property.propertyType}";
                    Debug.LogWarning(warning, property.serializedObject.targetObject);
                    break;
            }
        }

        private void ApplyMaxValue<T>(T propertyValue, float maxValue, System.Action<T> setPropertyValue)
        {
            if (propertyValue is float floatValue)
            {
                if (floatValue > maxValue)
                {
                    setPropertyValue((T)(object)maxValue);
                }
            }
            else if (propertyValue is int intValue)
            {
                if (intValue > maxValue)
                {
                    setPropertyValue((T)(object)(int)maxValue);
                }
            }
        }

        private void ApplyMaxValueToVector(Vector2 propertyValue, float maxValue, System.Action<Vector2> setPropertyValue)
        {
            setPropertyValue(Vector2.Min(propertyValue, new Vector2(maxValue, maxValue)));
        }

        private void ApplyMaxValueToVector(Vector3 propertyValue, float maxValue, System.Action<Vector3> setPropertyValue)
        {
            setPropertyValue(Vector3.Min(propertyValue, new Vector3(maxValue, maxValue, maxValue)));
        }

        private void ApplyMaxValueToVector(Vector4 propertyValue, float maxValue, System.Action<Vector4> setPropertyValue)
        {
            setPropertyValue(Vector4.Min(propertyValue, new Vector4(maxValue, maxValue, maxValue, maxValue)));
        }

        private void ApplyMaxValueToVectorInt(Vector2Int propertyValue, float maxValue, System.Action<Vector2Int> setPropertyValue)
        {
            setPropertyValue(Vector2Int.Min(propertyValue, new Vector2Int((int)maxValue, (int)maxValue)));
        }

        private void ApplyMaxValueToVectorInt(Vector3Int propertyValue, float maxValue, System.Action<Vector3Int> setPropertyValue)
        {
            setPropertyValue(Vector3Int.Min(propertyValue, new Vector3Int((int)maxValue, (int)maxValue, (int)maxValue)));
        }
    }
}
#endif
