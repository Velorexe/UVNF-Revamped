using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

namespace UVNF.Core.Entities.ScriptLines
{
    [AttributeUsage(AttributeTargets.Field)]
    public class TextParameterAttribute : ScriptLineParameterAttribute
    {
        public TextParameterAttribute(string label) : base(label) { }

        public override object ParseParameterValue(string parameter) => parameter;
    }
}