using System;
using UnityEngine;

namespace UVNF.Core.Entities.ScriptLines
{
    [AttributeUsage(AttributeTargets.Field)]
    public class FloatParameter : ScriptLineParameterAttribute
    {
        public FloatParameter(string label, float defaultValue = 0f, bool optional = true) : base(label, defaultValue, optional) { }

        public override object ParseParameterValue(string parameter)
        {
            if (float.TryParse(parameter, out float parsedValue))
                return parsedValue;
            Debug.LogWarning("Tried to parse parameter '" + parameter + "' to a float.");
            return default(float);
        }
    }
}
