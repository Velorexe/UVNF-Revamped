using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UVNF.Core.Entities;
using System.Linq;

namespace UVNF.Editor.Importer
{
    [UnityEditor.AssetImporters.ScriptedImporter(1, ".uvnf")]
    public class UVNFScriptScriptedImporter : UnityEditor.AssetImporters.ScriptedImporter
    {
        public override void OnImportAsset(UnityEditor.AssetImporters.AssetImportContext ctx)
        {
            GUID guid = AssetDatabase.GUIDFromAssetPath(ctx.assetPath);
            List<string> labels = AssetDatabase.GetLabels(guid).ToList();

            if (!labels.Contains("UVNFScript"))
            {
                labels.Add("UVNFScript");
                AssetDatabase.SetLabels(ctx.mainObject, labels.ToArray());
            }
        }
    }
}