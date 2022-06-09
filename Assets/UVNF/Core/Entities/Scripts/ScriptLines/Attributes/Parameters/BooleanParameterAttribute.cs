using UnityEngine;

namespace UVNF.Core.Entities.ScriptLines
{
    public class BooleanParameter : ScriptLineParameterAttribute
    {
        public BooleanParameter(string label, bool defaultValue = false, bool optional = true) : base(label, defaultValue, optional) { }

        public override object ParseParameterValue(string parameter)
        {
            if (bool.TryParse(parameter, out bool value))
                return value;
            Debug.LogWarning("Tried to parse a parameter to a boolean.");
            return false;
        }

        public override string ValueToString(object value)
        {
            return (bool)value ? "true" : "false";
        }
    }
}
