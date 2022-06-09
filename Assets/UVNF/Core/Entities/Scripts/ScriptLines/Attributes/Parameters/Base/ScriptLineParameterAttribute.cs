using System;

namespace UVNF.Core.Entities.ScriptLines
{
    [AttributeUsage(AttributeTargets.Field)]
    public abstract class ScriptLineParameterAttribute : ScriptLineAttribute
    {
        /// <param name="hidden">Hides the field from the Unity Editor if it's value is default</param>
        public ScriptLineParameterAttribute(string label, object defaultValue, bool optional) : base(label)
        {
            Optional = optional;
            DefaultValue = defaultValue;
        }

        public object DefaultValue { get; }

        public readonly bool Optional;

        public abstract object ParseParameterValue(string parameter);

        public virtual string ValueToString(object value) => value.ToString();
    }
}