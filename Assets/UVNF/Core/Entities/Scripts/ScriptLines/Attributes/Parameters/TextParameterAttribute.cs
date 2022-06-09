using System;

namespace UVNF.Core.Entities.ScriptLines
{
    [AttributeUsage(AttributeTargets.Field)]
    public class TextParameterAttribute : ScriptLineParameterAttribute
    {
        public TextParameterAttribute(string label, string defaultValue = "", bool optional = true) : base(label, defaultValue, optional) { }

        public override object ParseParameterValue(string parameter)
        {
            if (parameter[0] == '"')
                parameter = parameter.Remove(0, 1);
            if (parameter[parameter.Length - 1] == '"')
                parameter = parameter.Remove(parameter.Length - 1, 1);
            return parameter;
        }

        public override string ValueToString(object value)
        {
            return "\"" + base.ValueToString(value) + "\"";
        }
    }
}