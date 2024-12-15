using System;

namespace RDTools
{
    public abstract class EnableIfAttributeBase : MetaAttribute
    {
        public string[] Conditions { get; }
        public EConditionOperator ConditionOperator { get; }
        public bool Inverted { get; protected set; }

        /// <summary>
        /// If not null, <see cref="Conditions"/>[0] is the name of an enum variable.
        /// </summary>
        public Enum EnumValue { get; }

        protected EnableIfAttributeBase(string condition)
        {
            if (string.IsNullOrWhiteSpace(condition))
            {
                throw new ArgumentException("Condition cannot be null or whitespace.", nameof(condition));
            }

            ConditionOperator = EConditionOperator.And;
            Conditions = new[] { condition };
        }

        protected EnableIfAttributeBase(EConditionOperator conditionOperator, params string[] conditions)
        {
            if (conditions == null || conditions.Length == 0)
            {
                throw new ArgumentException("At least one condition must be provided.", nameof(conditions));
            }

            ConditionOperator = conditionOperator;
            Conditions = conditions;
        }

        protected EnableIfAttributeBase(string enumName, Enum enumValue)
            : this(enumName)
        {
            EnumValue = enumValue ?? throw new ArgumentNullException(nameof(enumValue), "Enum value cannot be null.");
        }
    }
}