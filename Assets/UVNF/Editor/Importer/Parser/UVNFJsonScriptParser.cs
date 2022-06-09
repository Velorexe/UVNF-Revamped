using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UVNF.Core.Entities;

namespace UVNF.Editor.Importer.Parser
{
    public class UVNFJsonScriptParser : IUVNFScriptParser
    {
        public UVNFScript CompileLines(UVNFScript script, string[] lines)
        {
            return script;
        }

        public string ExportLines(UVNFScript script)
        {
            return "";
        }
    }
}
