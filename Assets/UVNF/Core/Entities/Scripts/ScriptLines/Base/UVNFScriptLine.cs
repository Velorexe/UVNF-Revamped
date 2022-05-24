using System;
using System.Text;
using UVNF.Core.Entities.ScriptLines;
using System.Reflection;
using Cysharp.Threading.Tasks;
using System.Threading;

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

        /// <summary>
        /// Processes the incoming parameters and values
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="values"></param>
        public void ProcessParameters(string[] parameters, string[] values)
        {
            FieldInfo[] fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (string parameter in parameters)
            {
                foreach (FieldInfo t in fields)
                {
                    if (Attribute.GetCustomAttribute(t, typeof(ScriptLineParameterAttribute)) is ScriptLineParameterAttribute attribute && attribute.Label == parameter)
                    {
                        t.SetValue(this, attribute.ParseParameterValue(values[Array.IndexOf(parameters, parameter)]));
                        break;
                    }
                }
            }
        }

        public virtual async UniTask Execute(UVNFGameManager callback, CancellationToken token) { await UniTask.Yield(); }

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
                if (Attribute.GetCustomAttribute(t, typeof(ScriptLineParameterAttribute)) is ScriptLineParameterAttribute attribute && !string.IsNullOrEmpty(t.GetValue(this).ToString()))
                {
                    builder.Append(" " + attribute.Label + ":");
                    builder.Append(t.GetValue(this));
                }
            }

            return builder.ToString();
        }
#endif
    }
}