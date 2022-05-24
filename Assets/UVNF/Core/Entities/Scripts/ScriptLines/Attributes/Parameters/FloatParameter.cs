using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UVNF.Core.Entities.ScriptLines
{
    [AttributeUsage(AttributeTargets.Field)]
    public class FloatParameter : ScriptLineParameterAttribute
    {
        public FloatParameter(string label) : base(label) { }

        public override object ParseParameterValue(string parameter)
        {
            if (float.TryParse(parameter, out float parsedValue))
                return parsedValue;
            Debug.LogWarning("Tried to parse parameter '" + parameter + "' to a float.");
            return default(float);
        }
    }
}
