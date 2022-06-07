using System;
using System.Text;
using UVNF.Core.Entities.ScriptLines;
using System.Reflection;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace UVNF.Core.Entities
{
    [Serializable]
    public abstract class UVNFScriptLine
    {
        public abstract char Literal { get; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        protected UVNFScriptLine() { }

#if UNITY_EDITOR
        public virtual string ToCommandString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(Literal);

            ScriptLineAliasAttribute alias = GetType().GetCustomAttribute<ScriptLineAliasAttribute>();
            builder.Append(alias != null ? alias.GetAlias() : GetType().Name.Replace("ScriptLine", ""));

            FieldInfo[] fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (FieldInfo t in fields)
            {
                var test = t.GetValue(this);
                if (Attribute.GetCustomAttribute(t, typeof(ScriptLineParameterAttribute)) is ScriptLineParameterAttribute attribute
                    && !string.IsNullOrEmpty(t.GetValue(this).ToString())
                    && !(t.GetValue(this).Equals(attribute.DefaultValue)))
                {
                    builder.Append(" " + (attribute.Optional ? attribute.Label + ":" : ""));
                    builder.Append(attribute.ValueToString(t.GetValue(this)));
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Processes the incoming parameters and values
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="values"></param>
        public void ProcessParameters(string defaultParameter, string[] parameters, string[] values)
        {
            FieldInfo[] fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            FieldInfo defaultField = null;
            ScriptLineParameterAttribute defaultAttribute = null;

            foreach (string parameter in parameters.Append(defaultParameter))
            {
                foreach (FieldInfo t in fields)
                {
                    if (Attribute.GetCustomAttribute(t, typeof(ScriptLineParameterAttribute)) is ScriptLineParameterAttribute attribute)
                    {
                        if (attribute.Label == parameter)
                        {
                            t.SetValue(this, attribute.ParseParameterValue(values[Array.IndexOf(parameters, parameter)]));
                            break;
                        }
                        else if (!attribute.Optional)
                            t.SetValue(this, attribute.ParseParameterValue(defaultParameter));
                    }
                }
            }
        }
#endif
    }
}