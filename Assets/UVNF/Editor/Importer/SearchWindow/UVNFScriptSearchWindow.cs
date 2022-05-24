using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UVNF.Assembly;
using UVNF.Editor.Extensions;
using UVNF.Core.Entities;

namespace UVNF.Editor.Importer
{
    public class UVNFScriptSearchWindow : EditorWindow
    {
        private string _searchQuery = string.Empty;

        private Vector2 _scrollPosition = Vector2.zero;

        private static UVNFScriptLine[] _options;

        private static GUISkin _uvnfSkin;

        private static UVNFScript _script;
        private static UnityEditor.Editor _repaintParent;

        private bool _setFocus = false;

        public static UVNFScriptSearchWindow Init(Vector2 mousePosition, GUISkin uvnfSkin, UVNFScript script, UnityEditor.Editor parent)
        {
            UVNFScriptSearchWindow window = CreateInstance<UVNFScriptSearchWindow>();

            window.position = new Rect(mousePosition, new Vector2(200f, 200f));
            window.ShowPopup();

            _options = AssemblyHelpers.GetEnumerableOfType<UVNFScriptLine>();
            _uvnfSkin = uvnfSkin;

            _script = script;
            _repaintParent = parent;

            return window;
        }

        private void OnGUI()
        {
            if (_uvnfSkin == null) Close();

            GUILayout.BeginVertical();
            {
                GUILayout.BeginHorizontal();
                {
                    if (!_setFocus)
                        GUI.SetNextControlName("SearchQuery");

                    _searchQuery = GUILayout.TextField(_searchQuery);
                    GUILayout.Button(EditorGUIUtility.IconContent("d_Search Icon"), GUILayout.MaxWidth(20f), GUILayout.MaxHeight(20f));

                    if (!_setFocus)
                    {
                        GUI.FocusControl("SearchQuery");
                        _setFocus = true;
                    }
                }
                GUILayout.EndHorizontal();

                _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
                {
                    for (int i = 0; i < _options.Length; i++)
                    {
                        string name = _options[i].GetType().Name.Replace("ScriptLine", "").AddSpaceToString(true);
                        if (name.ToLower().Contains(_searchQuery.ToLower()))
                        {
                            if(GUILayout.Button(name, _uvnfSkin.button))
                            {
                                _script.AddLine(_options[i]);

                                _repaintParent.Repaint();
                                Close();
                            }
                        }
                    }
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndVertical();
        }
    }
}