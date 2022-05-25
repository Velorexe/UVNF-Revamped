using System;
using UnityEngine;

namespace UVNF.Core.Entities.ScriptLines
{
    public class EnumParameterAttribute : ScriptLineParameterAttribute
    {
        public EnumParameterAttribute(string label, Type enumType, bool optional = true) : base(label, optional)
        {
            if (enumType.IsEnum)
                EnumType = enumType;
            else
                Debug.LogError($"A Type that isn't an enum has been given to an EnumParameter with label '{label}'.");
        }

        public readonly Type EnumType;

        public override object DefaultValue => Activator.CreateInstance(EnumType);

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