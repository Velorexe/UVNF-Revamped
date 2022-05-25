using System;
using UnityEngine;

namespace UVNF.Core.Entities.ScriptLines
{
    [AttributeUsage(AttributeTargets.Field)]
    public class FloatParameter : ScriptLineParameterAttribute
    {
        public FloatParameter(string label, bool optional = true) : base(label, optional) { }

        public override object DefaultValue => default(float);

        public override object ParseParameterValue(string parameter)
        {
            if (float.TryParse(parameter, out float parsedValue))
                return parsedValue;
            Debug.LogWarning("Tried to parse parameter '" + parameter + "' to a float.");
            return default(float);
        }
    }
}
