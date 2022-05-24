using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UVNF.Core.Entities;

namespace UVNF.Editor.Importer
{
    [UnityEditor.AssetImporters.ScriptedImporter(1, ".uvnf")]
    public class UVNFScriptScriptedImporter : UnityEditor.AssetImporters.ScriptedImporter
    {
        public override void OnImportAsset(UnityEditor.AssetImporters.AssetImportContext ctx)
        {
            //Add itself to the UVNF Script Manager
        }
    }
}