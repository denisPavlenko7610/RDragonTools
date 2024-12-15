#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace RDTools.Editor
{
    public class MinValuePropertyValidator : PropertyValidatorBase
    {
        public override void ValidateProperty(SerializedProperty property)
        {
            // Retrieve the MinValueAttribute for the current property
            MinValueAttribute minValueAttribute = PropertyUtility.GetAttribute<MinValueAttribute>(property);

            // Check if the min value Attribute exists
            if (minValueAttribute == null)
            {
                Debug.LogWarning("MinValueAttribute not found on property: " + property.name, property.serializedObject.targetObject);
                return;
            }

            // Perform validation based on property type
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    ValidateIntegerProperty(property, minValueAttribute.MinValue);
                    break;

                case SerializedPropertyType.Float:
                    ValidateFloatProperty(property, minValueAttribute.MinValue);
                    break;

                case SerializedPropertyType.Vector2:
                    ValidateVectorProperty(property.vector2Value, minValueAttribute.MinValue, (v) => property.vector2Value = v);
                    break;

                case SerializedPropertyType.Vector3:
                    ValidateVectorProperty(property.vector3Value, minValueAttribute.MinValue, (v) => property.vector3Value = v);
                    break;

                case SerializedPropertyType.Vector4:
                    ValidateVectorProperty(property.vector4Value, minValueAttribute.MinValue, (v) => property.vector4Value = v);
                    break;

                case SerializedPropertyType.Vector2Int:
                    ValidateVectorIntProperty(property.vector2IntValue, minValueAttribute.MinValue, (v) => property.vector2IntValue = v);
                    break;

                case SerializedPropertyType.Vector3Int:
                    ValidateVectorIntProperty(property.vector3IntValue, minValueAttribute.MinValue, (v) => property.vector3IntValue = v);
                    break;

                default:
                    Debug.LogWarning($"{minValueAttribute.GetType().Name} can be used only on int, float, Vector or VectorInt fields", property.serializedObject.targetObject);
                    break;
            }
        }

        private void ValidateIntegerProperty(SerializedProperty property, float minValue)
        {
            if (property.intValue < minValue)
            {
                property.intValue = Mathf.FloorToInt(minValue);
            }
        }

        private void ValidateFloatProperty(SerializedProperty property, float minValue)
        {
            if (property.floatValue < minValue)
            {
                property.floatValue = minValue;
            }
        }

        private void ValidateVectorProperty<T>(T propertyValue, float minValue, System.Action<T> setValue)
            where T : struct
        {
            if (propertyValue is Vector2 vector2Value)
            {
                setValue((T)(object)Vector2.Max(vector2Value, new Vector2(minValue, minValue)));
            }
            else if (propertyValue is Vector3 vector3Value)
            {
                setValue((T)(object)Vector3.Max(vector3Value, new Vector3(minValue, minValue, minValue)));
            }
            else if (propertyValue is Vector4 vector4Value)
            {
                setValue((T)(object)Vector4.Max(vector4Value, new Vector4(minValue, minValue, minValue, minValue)));
            }
        }

        private void ValidateVectorIntProperty<T>(T propertyValue, float minValue, System.Action<T> setValue)
            where T : struct
        {
            if (propertyValue is Vector2Int vector2IntValue)
            {
                setValue((T)(object)Vector2Int.Max(vector2IntValue, new Vector2Int(Mathf.FloorToInt(minValue), Mathf.FloorToInt(minValue))));
            }
            else if (propertyValue is Vector3Int vector3IntValue)
            {
                setValue((T)(object)Vector3Int.Max(vector3IntValue, new Vector3Int(Mathf.FloorToInt(minValue), Mathf.FloorToInt(minValue), Mathf.FloorToInt(minValue))));
            }
        }
    }
}
#endif
