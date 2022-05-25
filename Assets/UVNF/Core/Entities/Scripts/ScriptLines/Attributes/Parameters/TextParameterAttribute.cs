using System;

namespace UVNF.Core.Entities.ScriptLines
{
    [AttributeUsage(AttributeTargets.Field)]
    public class TextParameterAttribute : ScriptLineParameterAttribute
    {
        public TextParameterAttribute(string label, bool optional = true) : base(label, optional) { }

        public override object DefaultValue => string.Empty;

        public override object ParseParameterValue(string parameter) => parameter;
    }
}