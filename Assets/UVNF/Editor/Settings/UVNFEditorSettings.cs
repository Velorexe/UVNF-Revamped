using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UVNF.Assembly;
using UVNF.Editor.Importer.Parser;
using UVNF.Utilities;

public class UVNFEditorSettings : ScriptableObject
{
    public const string k_SettingsPath = "Assets/UVNF/Editor/Settings/UVNFEditorSettings.asset";

    public Type ScriptParserType { get { return m_UVNFScriptParser.SystemType; } }
    [SerializeReference]
    private SerializableType m_UVNFScriptParser;

    public GUISkin EditorSkin { get { return m_GUISkin; } }
    [SerializeField]
    private GUISkin m_GUISkin;

    internal static UVNFEditorSettings GetOrCreateSettings()
    {
        UVNFEditorSettings settings = AssetDatabase.LoadAssetAtPath<UVNFEditorSettings>(k_SettingsPath);
        if (settings == null)
        {
            settings = CreateInstance<UVNFEditorSettings>();

            settings.m_UVNFScriptParser = new SerializableType(typeof(UVNFDefaultScriptParser));
            settings.m_GUISkin = EditorGUIUtility.Load("Assets/UVNF/Editor/GUISKIN/UVNFSkin.guiskin") as GUISkin;

            AssetDatabase.CreateAsset(settings, k_SettingsPath);
            AssetDatabase.SaveAssets();
        }
        return settings;
    }

    internal static SerializedObject GetSerializedSettings()
    {
        return new SerializedObject(GetOrCreateSettings());
    }

    public IUVNFScriptParser GetParser() => (IUVNFScriptParser)Activator.CreateInstance(m_UVNFScriptParser.SystemType);
}

static class UVNFEditorSettingsEditor
{
    private static SerializableType[] _UVNFScriptParsers = new SerializableType[0];
    private static GUIContent[] _UVNFScriptParserLabels = new GUIContent[0];

    private static int _UVNFScriptParserIndex = 0;

    private static void Setup()
    {
        if (_UVNFScriptParsers.Length == 0)
        {
            _UVNFScriptParsers = AssemblyHelpers.GetEnumerableOfInterfaceType<IUVNFScriptParser>().Select(x => new SerializableType(x.GetType())).ToArray();
            _UVNFScriptParserLabels = _UVNFScriptParsers.Select(x => new GUIContent(x.Name)).ToArray();

            Type parserType = UVNFEditorSettings.GetOrCreateSettings().ScriptParserType;
            if (parserType != null)
                _UVNFScriptParserIndex = Array.FindIndex(_UVNFScriptParsers, (x) => x.SystemType == parserType);
        }
    }

    [SettingsProvider]
    public static SettingsProvider CreateUVNFEditorSettingsProvider()
    {
        SettingsProvider provider = new SettingsProvider("Project/UVNF/Editor", SettingsScope.Project)
        {
            label = "Editor",
            guiHandler = (searchContext) =>
            {
                Setup();

                SerializedObject settings = UVNFEditorSettings.GetSerializedSettings();

                _UVNFScriptParserIndex = EditorGUILayout.Popup(new GUIContent("Script Parser"), _UVNFScriptParserIndex, _UVNFScriptParserLabels);
                settings.FindProperty("m_UVNFScriptParser").managedReferenceValue = _UVNFScriptParsers[_UVNFScriptParserIndex];

                EditorGUILayout.PropertyField(settings.FindProperty("m_GUISkin"), new GUIContent("GUI Skin"));

                settings.ApplyModifiedProperties();
            },
            keywords = new HashSet<string>(new[] { "Parser", "Skin" })
        };

        return provider;
    }
}