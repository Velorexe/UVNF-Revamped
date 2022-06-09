using UnityEngine;
using System;

namespace UVNF.Core.Entities.ScriptLines
{
    [AttributeUsage(AttributeTargets.Field)]
    public class IntParameterAttribute : ScriptLineParameterAttribute
    {
        public IntParameterAttribute(string label, int defaultValue = 0, bool optional = true) : base(label, defaultValue, optional) { }

        public override object ParseParameterValue(string parameter)
        {
            if (int.TryParse(parameter, out int parsedValue))
                return parsedValue;
            Debug.LogWarning("Tried to parse parameter '" + parameter + "' to an int.");
            return default(int);
        }
    }
}