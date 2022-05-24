using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace UVNF.Core.Entities.ScriptLines
{
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumParameterAttribute : ScriptLineParameterAttribute
    {
        public EnumParameterAttribute(string label, Type enumType) : base(label)
        {
            if (enumType.IsEnum)
                EnumType = enumType;
            else
                Debug.LogError("An Type that isn't an enum has been given to an EnumParameter");
        }

        public readonly Type EnumType;

        public override object ParseParameterValue(string parameter)
        {
            if (EnumType != null)
            {
                object value = Enum.Parse(EnumType, parameter);
                if (value != null)
                    return value;
            }
            Debug.LogWarning("Tried to parse a parameter to an enum.");
            return null;
        }
    }
}