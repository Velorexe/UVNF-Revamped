using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UVNF.Core.Entities;
using UVNF.Core.Entities.ScriptLines;
using UVNF.Editor.Extensions;
using UVNF.Editor.Importer.Parser;

namespace UVNF.Editor.Importer
{
    [CustomEditor(typeof(UVNFScriptScriptedImporter))]
    public class UVNFScriptScriptedImporterEditor : UnityEditor.AssetImporters.ScriptedImporterEditor
    {
        private const float _rightSideSafeSpace = 25f;

        private UVNFScript _script;
        private int _focusedLine = -1;

        private Vector2 _scrollPosition = new Vector2();

        private GUISkin _uvnfSkin;

        private Texture2D _guiSkinBuffer;

        private FileSystemWatcher _watcher;

        private UVNFScriptSearchWindow _searchWindow;

        private bool _draggingScriptLine = false;
        private int _draggingScriptLineIndex = 0;

        // true = firm, false = flexible
        private bool _parameterType = true;

        private string[] _stringLines = new string[0];
        private string _suggestAppend = string.Empty;

        private IUVNFScriptParser _scriptParser = new UVNFScriptParser();

        public override void OnInspectorGUI()
        {
            if (_uvnfSkin == null)
                _uvnfSkin = EditorGUIUtility.Load("Assets/UVNF/Editor/GUISKIN/UVNFSkin.guiskin") as GUISkin;

            if (Event.current.type == EventType.MouseDrag)
                Repaint();

            if (_scriptParser == null)
                _scriptParser = new UVNFScriptParser();

            // true if an individual element is clicked
            bool onElementClicked = false;

            GUILayout.BeginHorizontal();
            {
                GUI.enabled = !_parameterType;
                if (GUILayout.Button("Parameter Based"))
                {
                    _parameterType = !_parameterType;
                }
                GUI.enabled = true;

                GUI.enabled = _parameterType;
                if (GUILayout.Button("Text Based"))
                {
                    _parameterType = !_parameterType;
                }
                GUI.enabled = true;

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Compile"))
                {
                    string assetPath = Path.GetDirectoryName(_script.AssetPath);
                    _script.name = Path.GetFileNameWithoutExtension(_script.AssetPath);

                    AssetDatabase.CreateAsset(_script, assetPath + "//" + _script.name + ".asset");
                }
            }
            GUILayout.EndHorizontal();

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, _uvnfSkin.scrollView, GUILayout.MaxHeight(Screen.height - 300f));
            {
                for (int i = 0; i < _script.Lines.Count; i++)
                {
                    bool isVisible = i > _script.Lines.Count * ((_scrollPosition.y) / (_script.Lines.Count * 32f)) - 22f && i < _script.Lines.Count * ((_scrollPosition.y) / (_script.Lines.Count * 32f)) + 22f;
                    if (isVisible)
                    {
                        ColorTint tint = _script.Lines[i].GetType().GetCustomAttribute<ColorTint>();

                        if (tint != null)
                            _uvnfSkin.box.normal.background = GUIColorTexture(tint.Tint.ChangeColorBrightness(0.5f));
                        else
                            _uvnfSkin.box.normal.background = GUIColorTexture(Color.grey);

                        if (_draggingScriptLine && _draggingScriptLineIndex == i)
                        {
                            GUI.DrawTexture(new Rect(Event.current.mousePosition, new Vector2(30f, 29f)), _uvnfSkin.box.normal.background, ScaleMode.StretchToFill);
                        }
                        else
                        {
                            // Dragging
                            GUILayout.BeginHorizontal();
                            {
                                // Line indicator
                                GUILayout.Box((i + 1).ToString(), _uvnfSkin.box, GUILayout.Width(30f), GUILayout.Height(30f));

                                if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown)
                                {
                                    Event.current.Use();

                                    _draggingScriptLine = true;
                                    _draggingScriptLineIndex = i;
                                }

                                Rect lastRect = GUILayoutUtility.GetLastRect();

                                // Parameters
                                GUILayout.BeginVertical(_uvnfSkin.box, GUILayout.Height(30f), GUILayout.Width(EditorGUIUtility.currentViewWidth - 30f));
                                {
                                    if (_parameterType)
                                    {
                                        #region Draw Firm Parameters
                                        // Deduct the width of the Number box and the standard Unity indenting
                                        // Also deduct a standard safe space
                                        float maxWidth = EditorGUIUtility.currentViewWidth - 170f - _rightSideSafeSpace;
                                        float currentWidth = 0f;

                                        GUI.enabled = i == _focusedLine;

                                        GUILayout.BeginHorizontal(GUILayout.MaxWidth(maxWidth + 120f));
                                        {
                                            // ScriptLine title
                                            string scriptLineLabel = _script.Lines[i].GetType().Name.Replace("ScriptLine", "").AddSpaceToString(true) + ": ";
                                            GUILayout.Label(scriptLineLabel, _uvnfSkin.label, GUILayout.ExpandHeight(false));

                                            maxWidth -= _uvnfSkin.label.CalcSize(new GUIContent(scriptLineLabel)).x;

                                            SerializedObject obj = new SerializedObject(_script);
                                            SerializedProperty line = obj.FindProperty("_lines").GetArrayElementAtIndex(i);

                                            // Drawing the actual attribute
                                            FieldInfo[] fields = _script.Lines[i].GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                                            for (int j = 0; j < fields.Length; j++)
                                            {
                                                if (fields[j].FieldType.IsValueType)
                                                {
                                                    var test = Activator.CreateInstance(fields[j].FieldType);
                                                    var test2 = fields[j].GetValue(_script.Lines[i]);

                                                    bool test3 = test != test2;
                                                }

                                                // Add Hidden tag
                                                if (Attribute.GetCustomAttribute(fields[j], typeof(ScriptLineParameterAttribute)) is ScriptLineParameterAttribute field
                                                    && (!field.Optional || (field.Optional && !fields[j].GetValue(_script.Lines[i]).Equals(field.DefaultValue)) || _focusedLine == i))

                                                {
                                                    if (currentWidth + 5f > maxWidth)
                                                    {
                                                        currentWidth = 0f;

                                                        GUILayout.FlexibleSpace();
                                                        GUILayout.EndHorizontal();

                                                        // Space between different parameter lines
                                                        GUILayout.BeginVertical();
                                                        {
                                                            GUILayout.Space(5f);
                                                        }
                                                        GUILayout.EndVertical();

                                                        GUILayout.BeginHorizontal();
                                                    }

                                                    SerializedProperty property = line.FindPropertyRelative(fields[j].Name);
                                                    string label = CleanParamaterName(fields[j].Name);

                                                    float typeSpace = fields[j].FieldType.IsValueType ? 30f : 15f;

                                                    Vector2 propertySize = _uvnfSkin.label.CalcSize(
                                                        new GUIContent(field.ValueToString(fields[j].GetValue(_script.Lines[i]))));
                                                    Vector2 labelSize = _uvnfSkin.label.CalcSize(new GUIContent(label));

                                                    float expectedWidth = currentWidth + propertySize.x + labelSize.x + typeSpace;

                                                    bool textWrap = _uvnfSkin.textArea.wordWrap;

                                                    EditorGUILayout.BeginHorizontal();
                                                    {
                                                        EditorGUILayout.LabelField(label, _uvnfSkin.label, GUILayout.MaxWidth(labelSize.x + 5f));

                                                        if (property.propertyType == SerializedPropertyType.String)
                                                        {
                                                            _uvnfSkin.textArea.wordWrap = true;
                                                            property.stringValue = EditorGUILayout.TextArea(property.stringValue,
                                                                _uvnfSkin.textArea,
                                                                GUILayout.MaxWidth(propertySize.x + 10f),
                                                                GUILayout.Height(EditorGUIUtility.singleLineHeight * (1f + Mathf.Floor(expectedWidth / maxWidth))));
                                                            _uvnfSkin.textArea.wordWrap = textWrap;
                                                        }
                                                        else
                                                            EditorGUILayout.PropertyField(property, new GUIContent(""), GUILayout.MaxWidth(propertySize.x + typeSpace));

                                                        EditorGUILayout.Space(10f);
                                                    }
                                                    EditorGUILayout.EndHorizontal();

                                                    currentWidth += propertySize.x + labelSize.x + 35f;
                                                }
                                            }

                                            obj.ApplyModifiedProperties();

                                            GUILayout.FlexibleSpace();
                                        }
                                        GUILayout.EndHorizontal();

                                        GUI.enabled = true;
                                        #endregion
                                    }
                                    else
                                    {
                                        #region Draw Flexible Parameters
                                        GUI.enabled = i == _focusedLine;

                                        GUI.SetNextControlName("flex-param");
                                        _stringLines[i] = GUI.TextArea(new Rect(lastRect.x + lastRect.width + 5f, i * 33f + 4f, EditorGUIUtility.currentViewWidth, lastRect.height), _stringLines[i]);

                                        //GUI.SetNextControlName("flex-param");
                                        //_stringLines[i] = GUILayout.TextArea(_stringLines[i], _uvnfSkin.textArea, GUILayout.ExpandWidth(true));
                                        GUI.FocusControl("flex-param");

                                        TextEditor txt = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                                        txt.cursorIndex = _stringLines[i].Length;

                                        if (Event.current.keyCode == KeyCode.Tab)
                                        {
                                            //GUI.FocusControl("flex-param");
                                            if (!string.IsNullOrEmpty(_suggestAppend))
                                            {
                                                _stringLines[i] += _suggestAppend;
                                                _suggestAppend = string.Empty;

                                            }

                                            Event.current.Use();
                                        }

                                        string lastWord = _stringLines[i].Split(' ').Last();

                                        FieldInfo[] fields = _script.Lines[i].GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                                        string[] suggestions = fields.Select(x => ((ScriptLineParameterAttribute)Attribute.GetCustomAttribute(x, typeof(ScriptLineParameterAttribute))).Label).ToArray();

                                        if (suggestions.Length > 0 && !string.IsNullOrWhiteSpace(lastWord))
                                        {
                                            suggestions = suggestions.Where(x => x.StartsWith(lastWord)).ToArray();
                                            if (suggestions.Length > 0)
                                            {
                                                Rect textFieldRect = GUILayoutUtility.GetLastRect();
                                                textFieldRect.x += _uvnfSkin.textArea.CalcSize(new GUIContent(_stringLines[i])).x - 2f;

                                                _suggestAppend = suggestions[0].Substring(lastWord.Length, suggestions[0].Length - lastWord.Length) + ": ";
                                                GUI.Label(textFieldRect, _suggestAppend, _uvnfSkin.GetStyle("suggestionlabel"));
                                            }
                                        }

                                        GUI.enabled = true;
                                        #endregion
                                    }
                                }
                                GUILayout.EndVertical();
                            }
                            GUILayout.EndHorizontal();
                        }

                        if (_draggingScriptLine && Event.current.type == EventType.MouseUp)
                        {
                            Event.current.Use();

                            _draggingScriptLine = false;
                            _draggingScriptLineIndex = -1;
                        }

                        if (Event.current.type == EventType.MouseUp && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                        {
                            onElementClicked = true;

                            CloseSearchWindow();

                            _focusedLine = i;
                            Repaint();

                            // Right mouse button
                            if (Event.current.button == 1)
                            {
                                CreateScriptLineMenu(i);
                            }
                        }
                    }
                    else
                    {
                        GUILayout.Box("", GUILayout.MinHeight(30f));
                    }
                }
            }
            GUILayout.EndScrollView();

            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseUp && !onElementClicked)
            {
                _focusedLine = -1;

                CloseSearchWindow();
                Repaint();
            }

            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseUp && (Event.current.button == 1 || Event.current.keyCode == KeyCode.Space) && !onElementClicked)
            {
                CloseSearchWindow();
                _searchWindow = UVNFScriptSearchWindow.Init(GUIUtility.GUIToScreenPoint(Event.current.mousePosition), _uvnfSkin, _script, this);
            }

            base.ApplyRevertGUI();
        }

        private IEnumerator ForceRepaint()
        {
            Repaint();
            yield return new WaitForSeconds(0.1f);
        }

        private void CloseSearchWindow()
        {
            if (_searchWindow != null)
                _searchWindow.Close();
        }

        private void OnDestroy()
        {
            CloseSearchWindow();
        }

        private void CreateScriptLineMenu(int scriptLineIndex)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Duplicate"), false, DuplicateScriptLineMenuCallback, scriptLineIndex);
            menu.AddItem(new GUIContent("Delete"), false, RemoveScriptLineMenuCallback, scriptLineIndex);

            menu.ShowAsContext();

            Event.current.Use();
        }

        private void RemoveScriptLineMenuCallback(object scriptLineIndex)
        {
            int index = (int)scriptLineIndex;
            _script.RemoveAt(index);
        }

        private void DuplicateScriptLineMenuCallback(object scriptLineIndex)
        {
            UVNFScriptLine duplicate = _script.Lines[(int)scriptLineIndex];
            _script.InsertLine(duplicate, (int)scriptLineIndex + 1);
        }

        private Texture2D GUIColorTexture(Color color)
        {
            if (_guiSkinBuffer == null)
                _guiSkinBuffer = new Texture2D(1, 1);

            _guiSkinBuffer.SetPixel(0, 0, color);
            _guiSkinBuffer.Apply();

            return _guiSkinBuffer;
        }

        public override void OnEnable()
        {
            base.OnEnable();

            string path = AssetDatabase.GetAssetPath(target);

            ParseLinesToScript(path);

            _watcher = new FileSystemWatcher(Path.Combine(Application.dataPath, Path.GetPathRoot(path)));
            _watcher.Changed += ParseLinesToScript;
        }

        private void ParseLinesToScript(string path)
        {
            string fullPath = Path.Combine(Application.dataPath, path.Replace("Assets/", ""));
            string[] lines = File.ReadAllLines(fullPath);

            _script = _scriptParser.CompileLines(CreateInstance<UVNFScript>(), lines);
            _script.AssetPath = path;

            _stringLines = lines;
        }

        private void ParseLinesToScript(object source, FileSystemEventArgs e)
        {
            string[] lines = File.ReadAllLines(e.FullPath);

            _script = _scriptParser.CompileLines(CreateInstance<UVNFScript>(), lines);
            _script.AssetPath = e.FullPath;

            _stringLines = lines;
        }

        public override void OnDisable()
        {
            base.OnDisable();

            File.WriteAllText(_script.AssetPath, _scriptParser.ExportLines(_script));
        }

        private static string CleanParamaterName(string name)
        {
            name = name.Replace("_", "");
            name = char.ToUpper(name[0]) + name.Substring(1);

            return string.Concat(name.Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
        }
    }
}