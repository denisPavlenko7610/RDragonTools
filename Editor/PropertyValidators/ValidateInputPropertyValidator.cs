#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
using System;

namespace RDTools.Editor
{
    public class ValidateInputPropertyValidator : PropertyValidatorBase
    {
        public override void ValidateProperty(SerializedProperty property)
        {
            // Retrieve the ValidateInputAttribute and the target object
            var validateInputAttribute = PropertyUtility.GetAttribute<ValidateInputAttribute>(property);
            var target = PropertyUtility.GetTargetObjectWithProperty(property);

            // Get the validation callback method
            MethodInfo validationCallback = ReflectionUtility.GetMethod(target, validateInputAttribute.CallbackName);

            if (validationCallback == null || validationCallback.ReturnType != typeof(bool))
            {
                return;
            }

            // Get the parameters of the callback method
            ParameterInfo[] callbackParameters = validationCallback.GetParameters();

            // Handle callback with no parameters
            if (callbackParameters.Length == 0)
            {
                ValidateWithoutParameter(validationCallback, target, validateInputAttribute, property);
            }
            // Handle callback with one parameter
            else if (callbackParameters.Length == 1)
            {
                ValidateWithParameter(validationCallback, target, property, callbackParameters[0], validateInputAttribute);
            }
            // Invalid number of parameters
            else
            {
                string warning = $"{validateInputAttribute.GetType().Name} requires a callback with a boolean return type and an optional single parameter of the same type as the field.";
                ShowHelpBox(warning, MessageType.Warning, property);
            }
        }

        private void ValidateWithoutParameter(MethodInfo validationCallback, object target, ValidateInputAttribute validateInputAttribute, SerializedProperty property)
        {
            if (!(bool)validationCallback.Invoke(target, null))
            {
                string message = string.IsNullOrEmpty(validateInputAttribute.Message) 
                    ? $"{property.name} is not valid" 
                    : validateInputAttribute.Message;

                ShowHelpBox(message, MessageType.Error, property);
            }
        }

        private void ValidateWithParameter(MethodInfo validationCallback, object target, SerializedProperty property, ParameterInfo callbackParameter, ValidateInputAttribute validateInputAttribute)
        {
            FieldInfo fieldInfo = ReflectionUtility.GetField(target, property.name);
            Type fieldType = fieldInfo.FieldType;
            Type parameterType = callbackParameter.ParameterType;

            if (fieldType == parameterType)
            {
                if (!(bool)validationCallback.Invoke(target, new object[] { fieldInfo.GetValue(target) }))
                {
                    string message = string.IsNullOrEmpty(validateInputAttribute.Message) 
                        ? $"{property.name} is not valid" 
                        : validateInputAttribute.Message;

                    ShowHelpBox(message, MessageType.Error, property);
                }
            }
            else
            {
                string warning = "The field type does not match the callback's parameter type.";
                ShowHelpBox(warning, MessageType.Warning, property);
            }
        }

        private void ShowHelpBox(string message, MessageType messageType, SerializedProperty property)
        {
            RDEditorGUI.HelpBox_Layout(message, messageType, context: property.serializedObject.targetObject);
        }
    }
}
#endif
