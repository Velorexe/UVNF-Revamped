using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UVNF.Core.Entities;
using UVNF.Editor.Importer.Parser;

namespace UVNF.Editor.Exporter
{
    public class UVNFDefaultScriptExporter : IPreprocessBuildWithReport
    {
        public int callbackOrder => 1;

        public async void OnPreprocessBuild(BuildReport report)
        {
            await ProcessScriptFiles();
        }

        [MenuItem("UVNF/Pre-process UVNF Scripts")]
        public static async UniTask ProcessScriptFiles()
        {
            string[] scriptFiles = AssetDatabase.FindAssets("l:UVNFScript");

            List<string> scriptPaths = scriptFiles.Select(x => AssetDatabase.GUIDToAssetPath(x)).ToList();
            scriptFiles = AssetDatabase.FindAssets($"t:{typeof(UVNFScript)}");

            List<UVNFScript> compiledScripts = scriptFiles.Select(x => AssetDatabase.GUIDToAssetPath(x)).Select(x => AssetDatabase.LoadAssetAtPath<UVNFScript>(x)).ToList();

            IUVNFScriptParser parser = UVNFEditorSettings.GetOrCreateSettings().GetParser();

            foreach (UVNFScript script in compiledScripts)
            {
                string fullPath = Path.Combine(Application.dataPath, script.AssetPath.Replace("Assets/", ""));
                parser.CompileLines(script, File.ReadAllLines(fullPath));

                AssetDatabase.SaveAssets();

                scriptPaths.Remove(script.AssetPath);
            }

            foreach (string scriptPath in scriptPaths)
            {
                UVNFScript script = ScriptableObject.CreateInstance<UVNFScript>();
                script.AssetPath = scriptPath;

                string fullPath = Path.Combine(Application.dataPath, scriptPath.Replace("Assets/", ""));
                parser.CompileLines(script, File.ReadAllLines(fullPath));

                string assetPath = Path.GetDirectoryName(scriptPath) + "/" + Path.GetFileNameWithoutExtension(scriptPath) + "-Compiled.asset";

                AssetDatabase.CreateAsset(script, assetPath);
                AssetDatabase.SaveAssets();

                compiledScripts.Add(script);
            }

            foreach (UVNFScript script in compiledScripts)
            {
                if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(script, out string guid, out long localId) &&
                    !await UVNFEditorResources.ResourceExists<UVNFScript>("Scripts/" + script.Name))
                {
                    UVNFEditorResources.AddResource(guid, "Scripts/" + script.Name, "UVNF-Script");
                }
            }
        }
    }
}
