using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace UVNF.Core.Entities.ScriptLines
{
    [AttributeUsage(AttributeTargets.Field)]
    public class IntParameterAttribute : ScriptLineParameterAttribute
    {
        public IntParameterAttribute(string label) : base(label) { }

        public override object ParseParameterValue(string parameter)
        {
            if (int.TryParse(parameter, out int parsedValue))
                return parsedValue;
            Debug.LogWarning("Tried to parse parameter '" + parameter + "' to an int.");
            return default(int);
        }
    }
}