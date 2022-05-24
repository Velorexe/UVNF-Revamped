using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UVNF.Core.Entities.Characters;
using UVNF.Editor.Extensions;
using System;

namespace UVNF.Editor.Windows
{
    public class CharacterPartEditorWindow : EditorWindow
    {
        private CharacterPose _focusedPose;

        private Vector2 _scrollPosition = new Vector2();
        private int _focusedCharacterPart = 0;

        private float _zoomAmount = 1f;
        private Vector2 _imageOffset = new Vector2();

        // Dragging the canvas
        private bool _canvasIsDragging = false;
        private Vector2 _canvasDragPoint = new Vector2();

        // Dragging individual part
        private bool _partIsDragging = false;
        private Vector2 _partDragPoint = new Vector2();

        // Dragging resize handle
        private bool _resizeIsDragging = false;
        private Vector2 _resizeDragPoint = new Vector2();

        private Rect _drawRect = new Rect();

        private bool _partTypesFoldout = true;
        private int _selectedSprite = 0;

        private bool _gizmosVisible = true;

        public void Init(CharacterPose focusedCharacter)
        {
            CharacterPartEditorWindow window = GetWindow<CharacterPartEditorWindow>();
            window.titleContent = new GUIContent("UVNF Part Editor");

            window.Show();

            _focusedPose = focusedCharacter;
        }

        private void OnGUI()
        {
            if (_focusedPose == null) Close();

            #region Canvas Dragging
            if (_focusedPose.PoseSprite != null && _drawRect.Contains(Event.current.mousePosition))
            {
                if (Event.current.type == EventType.ScrollWheel)
                {
                    if (Event.current.delta.y < 0f && _zoomAmount < 10f)
                        _zoomAmount += 0.25f;
                    else if (Event.current.delta.y > 0f && _zoomAmount > 1f)
                        _zoomAmount -= 0.25f;

                    Repaint();
                }

                if (Event.current.type == EventType.MouseDown && Event.current.button == 2)
                {
                    _canvasIsDragging = true;
                    _canvasDragPoint = Event.current.mousePosition;
                }
                else if (Event.current.type == EventType.MouseUp && Event.current.button == 2)
                {
                    _canvasIsDragging = false;
                    _canvasDragPoint = Vector2.zero;
                }

                if (_canvasIsDragging)
                {
                    _imageOffset += (Event.current.mousePosition - _canvasDragPoint);
                    _canvasDragPoint = Event.current.mousePosition;

                    //if (_imageOffset.x > 0f) _imageOffset.x = 0f;
                    //if (_imageOffset.y > 0f) _imageOffset.y = 0f;

                    //if (Mathf.Abs(_imageOffset.x) > _focusedPose.PoseSprite.rect.width) _imageOffset.x = -_focusedPose.PoseSprite.rect.width;
                    //if (Mathf.Abs(_imageOffset.y) > _focusedPose.PoseSprite.rect.height) _imageOffset.y = -_focusedPose.PoseSprite.rect.height;

                    Repaint();
                }
            }
            #endregion

            #region Character Display
            if (_focusedPose.PoseSprite != null)
            {
                Rect drawRect = _focusedPose.PoseSprite.rect;

                drawRect.x += 200f;

                _drawRect = drawRect;

                drawRect.x += _imageOffset.x;
                drawRect.y += _imageOffset.y;

                drawRect.width *= _zoomAmount;
                drawRect.height *= _zoomAmount;

                DrawBounds(drawRect, Color.white, 1f * _zoomAmount);
                GUI.DrawTexture(drawRect, _focusedPose.PoseSprite.texture, ScaleMode.ScaleToFit);

                if (_focusedPose.CharacterParts.Length > 0)
                {
                    foreach (CharacterPart part in _focusedPose.CharacterParts)
                    {
                        if (part.PoseSprites.Count > 0 && part.PoseSprites[part.SpriteToShow] != null && part.IsVisible)
                        {
                            Rect partRect = part.SpriteRect;

                            partRect.x *= _zoomAmount;
                            partRect.y *= _zoomAmount;

                            partRect.x += drawRect.x;
                            partRect.y += drawRect.y;

                            partRect.width *= _zoomAmount;
                            partRect.height *= _zoomAmount;

                            GUI.DrawTexture(partRect, part.PoseSprites[_selectedSprite].texture, ScaleMode.StretchToFill);

                            #region Part Moving & Resizing
                            if (_focusedCharacterPart != -1 && part == _focusedPose.CharacterParts[_focusedCharacterPart] && _gizmosVisible)
                            {
                                float handleSize = 5f * _zoomAmount;
                                Vector2 resize = DrawResizeHandle(new Vector2(partRect.x + partRect.width - handleSize, partRect.y + partRect.height - handleSize), Color.red, handleSize * 2f);

                                DrawBounds(partRect, Color.red, 1f * _zoomAmount);

                                //Prevent overlap of resizing and dragging
                                partRect.xMin += handleSize;
                                partRect.xMax -= handleSize;
                                partRect.yMin += handleSize;
                                partRect.yMax -= handleSize;

                                #region Dragging
                                if (partRect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown && Event.current.button == 0)
                                {
                                    _partIsDragging = true;
                                    _partDragPoint = Event.current.mousePosition;
                                }
                                else if (_partIsDragging && Event.current.type == EventType.MouseUp && Event.current.button == 0)
                                {
                                    _partIsDragging = false;
                                    _partDragPoint = Vector2.zero;
                                }

                                if (_partIsDragging)
                                {
                                    Rect newPosition = part.SpriteRect;

                                    newPosition.x += (Event.current.mousePosition - _partDragPoint).x / _zoomAmount;
                                    newPosition.y += (Event.current.mousePosition - _partDragPoint).y / _zoomAmount;

                                    part.SetSpriteRect(newPosition);

                                    _partDragPoint = Event.current.mousePosition;

                                    Repaint();
                                }
                                #endregion

                                #region Resizing
                                if (_resizeIsDragging)
                                {
                                    Rect newSize = part.SpriteRect;

                                    newSize.width += resize.x;
                                    newSize.height += resize.y;

                                    part.SetSpriteRect(newSize);

                                    Repaint();
                                }
                                #endregion
                            }
                            else if(partRect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown && Event.current.button == 0)
                            {
                                _focusedCharacterPart = Array.IndexOf(_focusedPose.CharacterParts, part);

                                GUI.FocusControl(null);

                                Repaint();
                            }
                            #endregion
                        }
                    }
                }
            }
            else
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        GUILayout.Label("The Pose doesn't contain a Sprite.", EditorStyles.boldLabel);
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndVertical();
            }
            #endregion

            GUILayout.BeginHorizontal();
            {
                #region Leftside Menu
                GUILayout.BeginVertical("box", GUILayout.MaxWidth(200f), GUILayout.MinHeight(position.height));
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("Parts", EditorStyles.boldLabel);

                        GUIContent addButton = EditorGUIUtility.IconContent("d_Toolbar Plus");
                        if (GUILayout.Button(addButton, GUILayout.MaxWidth(30f)))
                        {
                            //TODO: Get folder from EditorSettings
                            CharacterPart newPart = CreateInstance<CharacterPart>();
                            AssetDatabase.AddObjectToAsset(newPart, _focusedPose);

                            _focusedPose.AddCharacterPart(newPart);
                            newPart.name = newPart.PartName;

                            AssetDatabase.SaveAssets();
                        }
                    }
                    GUILayout.EndHorizontal();

                    if (_focusedPose != null && _focusedPose.CharacterParts.Length > 0)
                    {
                        //_focusedCharacterPart = this.CreateScrollView<CharacterPart>(ref _scrollPosition, _focusedCharacterPart, _focusedPose.CharacterParts, false, true);
                        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true);
                        for (int i = 0; i < _focusedPose.CharacterParts.Length; i++)
                        {
                            GUIStyle style = i == _focusedCharacterPart ? this.SelectedStyle() : GUIStyle.none;
                            GUILayout.BeginHorizontal();
                            {
                                if (GUILayout.Button($"{i + 1}: " + _focusedPose.CharacterParts[i].PartName, style))
                                {
                                    _focusedCharacterPart = i;
                                }

                                GUIContent visibleContent = _focusedPose.CharacterParts[i].IsVisible ? EditorGUIUtility.IconContent("d_scenevis_visible_hover") : EditorGUIUtility.IconContent("d_scenevis_hidden_hover");
                                if (GUILayout.Button(visibleContent, GUILayout.MaxWidth(30f)))
                                {
                                    _focusedPose.CharacterParts[i].IsVisible = !_focusedPose.CharacterParts[i].IsVisible;
                                }
                            }
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndScrollView();
                    }
                }
                GUILayout.EndVertical();
                #endregion

                #region Topbar
                GUILayout.FlexibleSpace();

                GUILayout.BeginVertical();
                {
                    string toolTip = string.Empty;
                    GUILayout.BeginHorizontal("box");
                    {
                        GUIContent icon = EditorGUIUtility.IconContent("d_ToolHandleCenter@2x");
                        if (GUILayout.Button(icon, GUILayout.ExpandWidth(false)))
                        {
                            _imageOffset = Vector2.zero;
                        }

                        if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) toolTip = "Recenter";

                        icon = _gizmosVisible ? EditorGUIUtility.IconContent("d_scenevis_visible_hover@2x") : EditorGUIUtility.IconContent("d_scenevis_hidden_hover@2x");
                        if(GUILayout.Button(icon, GUILayout.ExpandWidth(false)))
                        {
                            _gizmosVisible = !_gizmosVisible;
                        }

                        if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) toolTip = "Hide Gizmos";
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Label(toolTip);
                }
                GUILayout.EndVertical();
                #endregion

                #region Rightside Menu
                GUILayout.FlexibleSpace();
                if (_focusedCharacterPart != -1 && _focusedPose.CharacterParts.Length > 0)
                {
                    CharacterPart part = _focusedPose.CharacterParts[_focusedCharacterPart];
                    GUILayout.BeginVertical("box", GUILayout.MinWidth(200f), GUILayout.MinHeight(position.height));
                    {
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.FlexibleSpace();
                            part.SetPartName(GUILayout.TextField(part.PartName, "label"));
                            part.name = part.PartName;
                            GUILayout.FlexibleSpace();
                        }
                        GUILayout.EndHorizontal();

                        if (GUILayout.Button("Remove"))
                        {
                            DestroyImmediate(part, true);
                            AssetDatabase.SaveAssets();

                            _focusedPose.RemoveCharacterPart(_focusedCharacterPart);
                            _focusedCharacterPart -= 1;
                            return;
                        }

                        GUILayout.BeginVertical("box");
                        {
                            Rect partRect = part.SpriteRect;

                            partRect.x = EditorGUILayout.FloatField("X", partRect.x);
                            partRect.y = EditorGUILayout.FloatField("Y", partRect.y);

                            partRect.width = EditorGUILayout.FloatField("Width", partRect.width);
                            partRect.height = EditorGUILayout.FloatField("Height", partRect.height);

                            part.SetSpriteRect(partRect);
                        }
                        GUILayout.EndVertical();

                        GUILayout.BeginVertical("box");
                        {
                            _partTypesFoldout = EditorGUILayout.Foldout(_partTypesFoldout, "Sprites");
                            if (_partTypesFoldout)
                            {
                                if (GUILayout.Button("Add New Sprite"))
                                {
                                    part.AddPose();
                                }

                                for (int i = 0; i < part.PoseNames.Count; i++)
                                {
                                    GUILayout.BeginVertical("box");
                                    {
                                        part.PoseNames[i] = GUILayout.TextField(part.PoseNames[i], "label");
                                        part.PoseSprites[i] = (Sprite)EditorGUILayout.ObjectField(part.PoseSprites[i], typeof(Sprite), false);

                                        if (part.PoseSprites[i] != null)
                                        {
                                            Rect lastRect = GUILayoutUtility.GetLastRect();

                                            lastRect.y += EditorGUIUtility.singleLineHeight;

                                            lastRect.width = 150f;
                                            lastRect.height = 250f;

                                            GUI.DrawTexture(lastRect, part.PoseSprites[i].texture, ScaleMode.ScaleToFit);

                                            GUILayout.Space(lastRect.height);
                                        }

                                        if (GUILayout.Button("Remove"))
                                        {
                                            part.PoseNames.RemoveAt(i);
                                            part.PoseSprites.RemoveAt(i);

                                            return;
                                        }
                                    }
                                    GUILayout.EndVertical();

                                    if (i == _selectedSprite)
                                    {
                                        DrawBounds(GUILayoutUtility.GetLastRect(), Color.white, 1f);
                                    }
                                }
                            }
                        }
                        GUILayout.EndVertical();
                    }
                    GUILayout.EndVertical();
                }
                else
                {
                    GUILayout.Space(200f);
                }
                #endregion
            }
            GUILayout.EndHorizontal();
        }

        private void DrawBounds(Rect bounds, Color color, float width)
        {
            //Upper
            Rect boundRect = new Rect(bounds.x, bounds.y, bounds.width, width);
            EditorGUI.DrawRect(boundRect, color);

            //Right
            boundRect = new Rect(bounds.width + bounds.x, bounds.y, width, bounds.height);
            EditorGUI.DrawRect(boundRect, color);

            //Bottom
            boundRect = new Rect(bounds.x, bounds.height + bounds.y, bounds.width, width);
            EditorGUI.DrawRect(boundRect, color);

            //Left
            boundRect = new Rect(bounds.x, bounds.y, width, bounds.height);
            EditorGUI.DrawRect(boundRect, color);
        }

        private Vector2 DrawResizeHandle(Vector2 position, Color color, float size)
        {
            Rect resizeHandle = new Rect(position, new Vector2(size, size));

            if (resizeHandle.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                _resizeIsDragging = true;
                _resizeDragPoint = Event.current.mousePosition;
            }

            if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                _resizeIsDragging = false;
                _resizeDragPoint = Vector2.zero;
            }

            Vector2 resized = Vector2.zero;
            if (_resizeIsDragging)
            {
                resized = Event.current.mousePosition - _resizeDragPoint;

                resizeHandle.x += resized.x;
                resizeHandle.y += resized.y;

                _resizeDragPoint = Event.current.mousePosition;
            }

            EditorGUI.DrawRect(resizeHandle, color);

            return resized;
        }
    }
}