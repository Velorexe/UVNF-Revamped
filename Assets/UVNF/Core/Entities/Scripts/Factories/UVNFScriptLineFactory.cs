using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UVNF.Assembly;
using System.Reflection;
using System;
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

        /// <summary>
        /// Creates a new UVNFScriptLine based of the given tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public UVNFScriptLine FromTag(string tag)
        {
            char literal = tag[0];

            //If the literal is a Comment
            if (literal == ';') return null;
            //Else the literal is most likely a Command
            else
            {
                string alias = tag.Split(' ')[0];
                alias = alias.Substring(1, alias.Length - 1);

                for (int i = 0; i < _scriptLineTypes.Length; i++)
                {
                    if (literal == _scriptLineTypes[i].Literal)
                    {
                        ScriptLineAliasAttribute aliasAttribute = _scriptLineTypes[i].GetType().GetCustomAttribute<ScriptLineAliasAttribute>();
                        string filteredLine = tag.Remove(0, alias.Length + 1);

                        //If the ScriptLine has the same alias
                        if ((alias != null && aliasAttribute.HasAlias(alias)) || (alias == null && _scriptLineTypes[i].GetType().Name.Replace("ScriptLine", "").ToLower() == alias))
                        {
                            Tuple<string[], string[]> parameterAndValues = ParseParametersAndValues(filteredLine);

                            UVNFScriptLine scriptLine = Activator.CreateInstance(_scriptLineTypes[i].GetType()) as UVNFScriptLine;
                            scriptLine.ProcessParameters(parameterAndValues.Item1, parameterAndValues.Item2);

                            return scriptLine;
                        }
                    }
                }
            }

            //Something went wrong, none of the characters are recognized
            return null;
        }

        /// <summary>
        /// Returns parameters and values from the given sentence
        /// </summary>
        /// <param name="filteredLine"></param>
        /// <returns></returns>
        private Tuple<string[], string[]> ParseParametersAndValues(string filteredLine)
        {
            string[] splitLine = filteredLine.Split(':');
            Tuple<string[], string[]> result = new Tuple<string[], string[]>(new string[splitLine.Length - 1], new string[splitLine.Length - 1]);

            //Go up to the amount of parameters
            for (int i = 0; i < splitLine.Length - 1; i++)
            {
                string[] splitParameter = splitLine[i].Split(' ');

                //Set the parameter name
                result.Item1[i] = splitParameter[splitParameter.Length - 1];
                
                splitParameter = splitLine[i + 1].Split(' ');
                result.Item2[i] = string.Join(" ", splitParameter, 0, (splitParameter.Length == 1 ? 1 : splitParameter.Length - 1));

                if (i == splitLine.Length - 2) result.Item2[i] = string.Join(" ", splitParameter, 0, splitParameter.Length);
            }

            return result;
        }
    }
}