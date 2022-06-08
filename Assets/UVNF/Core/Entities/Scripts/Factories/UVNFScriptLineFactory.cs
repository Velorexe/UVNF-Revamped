using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UVNF.Assembly;
using UVNF.Core.Entities.ScriptLines;

namespace UVNF.Core.Entities.Factories
{
    public class UVNFScriptLineFactory
    {
        private UVNFScriptLine[] _scriptLineTypes = new UVNFScriptLine[0];

        public UVNFScriptLineFactory()
        {
            _scriptLineTypes = AssemblyHelpers.GetEnumerableOfType<UVNFScriptLine>();
        }

        public UVNFScriptLine CreateScriptLine(Type scriptLineType, (string, string)[] parameterValueData, string defaultParameter)
        {
            if (!scriptLineType.IsSubclassOf(typeof(UVNFScriptLine)))
                Debug.LogWarning($"Type '{scriptLineType.Name}' isn't a subclass of Type UVNFScriptLine.");
            else
            {
                UVNFScriptLine scriptLine = Activator.CreateInstance(scriptLineType) as UVNFScriptLine;

                FieldInfo defaultField = null;
                ScriptLineParameterAttribute defaultAttribute = null;

                FieldInfo[] fields = scriptLineType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                ScriptLineParameterAttribute[] attributes = new ScriptLineParameterAttribute[fields.Length];

                fields = fields.Where((x, i) =>
                {
                    if (Attribute.GetCustomAttribute(x, typeof(ScriptLineParameterAttribute)) is ScriptLineParameterAttribute attribute)
                    {
                        if (!attribute.Optional)
                        {
                            defaultField = x;
                            defaultAttribute = attribute;
                        }

                        attributes[i] = attribute;

                        return true;
                    }
                    return false;
                }).ToArray();

                if (defaultField != null && defaultAttribute != null && !string.IsNullOrEmpty(defaultParameter))
                    defaultField.SetValue(scriptLine, defaultAttribute.ParseParameterValue(defaultParameter));

                foreach ((string, string) parameter in parameterValueData)
                {
                    int index = Array.FindIndex(attributes, x => x.Label.Equals(parameter.Item1));
                    if (index > -1)
                        fields[index].SetValue(scriptLine, attributes[index].ParseParameterValue(parameter.Item2));
                }

                return scriptLine;
            }

            return null;
        }
    }
}