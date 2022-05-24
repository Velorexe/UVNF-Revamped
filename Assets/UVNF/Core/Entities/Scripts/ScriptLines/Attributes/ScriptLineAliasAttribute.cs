using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UVNF.Core.Entities.ScriptLines
{
    [AttributeUsage(AttributeTargets.Class), Serializable]
    public class ScriptLineAliasAttribute : Attribute
    {
        public readonly string[] Alias;

        public int ChosenAlias;

        public ScriptLineAliasAttribute(params string[] aliases)
        {
            Alias = aliases;
        }

        public bool HasAlias(string alias)
        {
            int aliasIndex = Array.IndexOf(Alias, alias);
            if (aliasIndex != -1) ChosenAlias = aliasIndex;

            return aliasIndex != -1;
        }

        public string GetAlias() => Alias[ChosenAlias];
    }
}
