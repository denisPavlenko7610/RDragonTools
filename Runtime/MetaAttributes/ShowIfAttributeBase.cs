using System;
using System.Linq;

namespace RDTools
{
    public class ShowIfAttributeBase : MetaAttribute
    {
        public string[] Conditions { get; private set; }
        public EConditionOperator ConditionOperator { get; private set; }
        public bool Inverted { get; protected set; }
        public Enum EnumValue { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowIfAttributeBase"/> class with a single condition.
        /// </summary>
        /// <param name="condition">The condition to evaluate.</param>
        public ShowIfAttributeBase(string condition)
        {
            if (string.IsNullOrWhiteSpace(condition))
            {
                throw new ArgumentException("Condition cannot be null or empty.", nameof(condition));
            }

            ConditionOperator = EConditionOperator.And;
            Conditions = new[] { condition };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowIfAttributeBase"/> class with multiple conditions.
        /// </summary>
        /// <param name="conditionOperator">The logical operator to apply between conditions.</param>
        /// <param name="conditions">The conditions to evaluate.</param>
        public ShowIfAttributeBase(EConditionOperator conditionOperator, params string[] conditions)
        {
            if (conditions == null || conditions.Length == 0 || conditions.Any(string.IsNullOrWhiteSpace))
            {
                throw new ArgumentException("Conditions cannot be null, empty, or contain empty elements.", nameof(conditions));
            }

            ConditionOperator = conditionOperator;
            Conditions = conditions;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowIfAttributeBase"/> class with an enum condition.
        /// </summary>
        /// <param name="enumName">The name of the enum variable.</param>
        /// <param name="enumValue">The enum value to evaluate.</param>
        public ShowIfAttributeBase(string enumName, Enum enumValue)
            : this(enumName)
        {
            EnumValue = enumValue ?? throw new ArgumentNullException(nameof(enumValue), "This parameter must be an enum value.");
        }
    }
}
