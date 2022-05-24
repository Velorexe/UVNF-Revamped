using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using UVNF.Assembly;

namespace UVNF.Core.Entities.ScriptLines
{
    [AttributeUsage(AttributeTargets.Field)]
    public abstract class ScriptLineParameterAttribute : ScriptLineAttribute
    {
        public ScriptLineParameterAttribute(string label) : base(label) { }

        public abstract object ParseParameterValue(string parameter);
    }
}