using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UVNF.Core.Entities.ScriptLines
{
    [AttributeUsage(AttributeTargets.Field)]
    public abstract class ScriptLineAttribute : Attribute
    {
        public readonly string Label;

        public ScriptLineAttribute(string label)
        {
            Label = label;
        }
    }
}