using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UVNF.Core.Entities.Characters;
using UVNF.Assembly;
using UVNF.Editor.Extensions;
using UVNF.Core.Canvas;

namespace UVNF.Editor.Windows
{
    public class CharacterEditorWindow : EditorWindow
    {
        [MenuItem("UVNF/Edit Characters")]
        public static void Init()
        {
            CharacterEditorWindow window = GetWindow<CharacterEditorWindow>();
            window.titleContent = new GUIContent("UVNF Character Editor");

            window.Show();
        }

        private Character[] _characters;

        private int _selectedCharacter = 0;

        private Vector2 _scrollPosition = new Vector2();
        private Vector2 _poseScrollPosition = new Vector2();

        private string _filter = string.Empty;

        private static GUIStyle _selectedStyle;

        private static string _characterPath = "Assets/UVNF/Assets/Characters/";

        private void OnGUI()
        {
            if (_characters == null)
                _characters = AssemblyHelpers.FindAssetsByType<Character>();

            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginVertical("box", GUILayout.MaxWidth(150f));
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("Characters", EditorStyles.boldLabel);

                        GUIContent refreshIcon = EditorGUIUtility.IconContent("d_Refresh");
                        if (GUILayout.Button(refreshIcon, GUILayout.MaxWidth(30f)))
                        {
                            AssetDatabase.Refresh();
                            _characters = AssemblyHelpers.FindAssetsByType<Character>();
                        }

                        GUIContent addButton = EditorGUIUtility.IconContent("d_Toolbar Plus");
                        if (GUILayout.Button(addButton, GUILayout.MaxWidth(30f)))
                        {
                            //TODO: Get folder from EditorSettings
                            Character newCharacter = CreateInstance<Character>();
                            newCharacter.name = newCharacter.CharacterName;

                            AssetDatabase.CreateAsset(newCharacter, _characterPath + "NewCharacter.asset");
                            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(newCharacter), ImportAssetOptions.ForceUpdate);

                            AssetDatabase.SaveAssets();

                            _characters = AssemblyHelpers.FindAssetsByType<Character>();
                        }
                    }
                    GUILayout.EndHorizontal();
                    _selectedCharacter = this.CreateScrollView<Character>(ref _scrollPosition, _selectedCharacter, _characters, false, true, GUILayout.MaxWidth(150f));
                }
                GUILayout.EndVertical();

                if (_characters.Length > 0)
                {
                    Character character = _characters[_selectedCharacter];

                    GUILayout.BeginVertical();
                    {
                        GUILayout.BeginHorizontal();
                        {
                            character.SetCharacterName(EditorGUILayout.TextField(character.CharacterName, GUILayout.MinWidth(position.width - 350f)));
                            character.name = character.CharacterName;

                            if (GUILayout.Button("Compile"))
                            {
                                CompileCharacter(character);
                            }

                            if (GUILayout.Button("Remove"))
                            {
                                DestroyImmediate(character, true);
                                _characters = AssemblyHelpers.FindAssetsByType<Character>();

                                return;
                            }

                            if (GUILayout.Button("Save", GUILayout.MaxWidth(50f)))
                            {
                                if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(character.PrecompiledCharacter, out string guid, out long localId))
                                    UVNFResources.Instance.AddResource(guid, "Characters/" + character.CharacterName);

                                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(character), character.CharacterName);
                                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(character), ImportAssetOptions.ForceUpdate);

                                AssetDatabase.SaveAssets();
                                AssetDatabase.Refresh();
                            }
                            GUILayout.FlexibleSpace();
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.BeginVertical("box", GUILayout.MinWidth(position.width - 150f));
                        {
                            if (GUILayout.Button("Add Pose"))
                            {
                                CharacterPose newPose = CreateInstance<CharacterPose>();

                                AssetDatabase.AddObjectToAsset(newPose, AssetDatabase.GetAssetPath(character));
                                AssetDatabase.SaveAssets();

                                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(newPose));

                                Undo.RegisterCreatedObjectUndo(newPose, AssetDatabase.GetAssetPath(newPose));

                                character.AddPose(newPose);

                                return;
                            }

                            if (character.Poses.Length > 0)
                            {
                                _poseScrollPosition = GUILayout.BeginScrollView(_poseScrollPosition, "horizontalscrollbar", GUIStyle.none, GUILayout.MaxHeight(350f), GUILayout.ExpandHeight(false));
                                {
                                    GUILayout.BeginHorizontal();
                                    {
                                        for (int i = 0; i < character.Poses.Length; i++)
                                        {
                                            GUILayout.BeginVertical("box", GUILayout.MinWidth(150f));
                                            {
                                                character.Poses[i].SetPoseName(GUILayout.TextField(character.Poses[i].PoseName, "label"));
                                                character.Poses[i].name = character.Poses[i].PoseName;

                                                character.Poses[i].SetPoseSprite((Sprite)EditorGUILayout.ObjectField(character.Poses[i].PoseSprite, typeof(Sprite), false));

                                                if (character.Poses[i].PoseSprite != null)
                                                {
                                                    Rect lastRect = GUILayoutUtility.GetLastRect();

                                                    lastRect.y += EditorGUIUtility.singleLineHeight;

                                                    lastRect.width = 150f;
                                                    lastRect.height = 250f;

                                                    GUI.DrawTexture(lastRect, character.Poses[i].PoseSprite.texture, ScaleMode.ScaleToFit);

                                                    GUILayout.Space(lastRect.height);
                                                }

                                                if (GUILayout.Button("Edit"))
                                                {
                                                    CharacterPartEditorWindow window = GetWindow<CharacterPartEditorWindow>();
                                                    window.Init(character.Poses[i]);
                                                }

                                                if (GUILayout.Button("Remove"))
                                                {
                                                    for (int j = 0; j < character.Poses[i].CharacterParts.Length; j++)
                                                        DestroyImmediate(character.Poses[i].CharacterParts[j], true);

                                                    AssetDatabase.RemoveObjectFromAsset(character.Poses[i]);
                                                    character.RemovePoseAt(i);

                                                    DestroyImmediate(character.Poses[i], true);

                                                    AssetDatabase.Refresh();
                                                    AssetDatabase.SaveAssets();

                                                    break;
                                                }
                                            }
                                            GUILayout.EndVertical();
                                        }
                                    }
                                    GUILayout.EndHorizontal();
                                }
                                GUILayout.EndScrollView();
                            }
                        }
                        GUILayout.EndVertical();
                    }
                    GUILayout.EndVertical();
                }
            }
            GUILayout.EndHorizontal();
        }

        protected static GUIStyle SelectedStyle()
        {
            if (_selectedStyle == null)
            {
                _selectedStyle = new GUIStyle();

                var grayTexture = new Texture2D(1, 1);
                var whiteTexture = new Texture2D(1, 1);

                grayTexture.SetPixel(0, 0, Color.gray);
                whiteTexture.SetPixel(0, 0, Color.white);

                _selectedStyle.normal.background = whiteTexture;
                _selectedStyle.active.background = grayTexture;
            }
            return _selectedStyle;
        }

        private void CompileCharacter(Character character)
        {
            GameObject newCharacter = new GameObject(character.CharacterName, typeof(RectTransform));
            OnScreenCharacter onScreenCharacter = newCharacter.AddComponent<OnScreenCharacter>();

            onScreenCharacter.Compile(character);

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(newCharacter, _characterPath + "Compiled/" + character.CharacterName + ".prefab");
            character.PrecompiledCharacter = prefab;

            AssetDatabase.SaveAssets();

            DestroyImmediate(newCharacter);
        }
    }
}