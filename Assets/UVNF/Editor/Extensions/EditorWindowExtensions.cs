using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UVNF.Editor.Extensions
{
    public static class EditorWindowExtensions
    {
        private static GUIStyle _selectedStyle;

        public static int CreateScrollView<T>(this EditorWindow window, ref Vector2 scrollPosition, int selectedIndex, object[] elements, bool showHorizontal, bool showVertical, params GUILayoutOption[] options) where T : UnityEngine.Object
        {
            int newSelectedIndex = selectedIndex;

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, showHorizontal, showVertical, options);
            {
                for (int i = 0; i < elements.Length; i++)
                {
                    GUIStyle style = i == newSelectedIndex ? SelectedStyle(window) : GUIStyle.none;
                    GUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button($"{i + 1}: {(elements[i] as T).name}", style))
                        {
                            newSelectedIndex = i;
                        }
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndScrollView();

            return newSelectedIndex;
        }

        public static GUIStyle SelectedStyle(this EditorWindow window)
        {
            if (_selectedStyle == null)
            {
                _selectedStyle = new GUIStyle();

                var grayTexture = new Texture2D(1, 1);
                var whiteTexture = new Texture2D(1, 1);

                grayTexture.SetPixel(0, 0, Color.clear);
                whiteTexture.SetPixel(0, 0, Color.clear);

                _selectedStyle.normal.background = whiteTexture;
                _selectedStyle.active.background = grayTexture;

                _selectedStyle.normal.textColor = Color.white;
            }
            return _selectedStyle;
        }
    }
}