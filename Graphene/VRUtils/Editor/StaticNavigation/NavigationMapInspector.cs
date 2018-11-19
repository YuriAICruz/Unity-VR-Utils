using System.Collections.Generic;
using Graphene.VRUtils.Presentation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Graphene.VRUtils.StaticNavigation
{
    [CustomEditor(typeof(NavigationMap))]
    public class NavigationMapInspector : Editor
    {
        private NavigationMap _self;
        private BaseNavigation _textureManager;

        private bool _viewMap;

        private int _selectedRoom;
        private GameObject _canvas;
        private RoomInteractionButton _navButtonAsset;
        private bool _editiList;

        private void Awake()
        {
            _self = target as NavigationMap;

            if (_self.Rooms == null)
                _self.Rooms = new List<Vector3>();

            _textureManager = _self.GetComponent<BaseNavigation>() ?? _self.gameObject.AddComponent<SphereTextureManager>();

            _canvas = GameObject.Find("3DCanvas");

            _navButtonAsset = Resources.Load<RoomInteractionButton>("UI/NavButton");
        }


        private void OnSceneGUI()
        {
            if (_viewMap)
            {
                CheckRoomRadius();

                DrawMap();
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            CheckRoomRadius();

            _viewMap = EditorGUILayout.Toggle("View Map", _viewMap);

            if (_viewMap)
            {
                if (_canvas == null)
                {
                    Debug.LogError("Create a Canvas Object Called '3DCanvas'");
                    _viewMap = false;
                    return;
                }

                MapGUI();
            }
        }


        private void MapGUI()
        {
            EditorGUILayout.LabelField("Rooms");

            if (GUILayout.Button("Add Room"))
            {
                Undo.RecordObject(target, "Add Room");
                _self.Rooms.Add(Vector3.zero);
                _selectedRoom = _self.Rooms.Count - 1;
                UpdateRoomPoints();
            }

            var labelWidth = EditorGUIUtility.labelWidth;

            EditorGUIUtility.labelWidth = 36;

            for (int i = 0; i < _self.Rooms.Count; i++)
            {
                EditorGUI.BeginChangeCheck();
                var str = EditorGUILayout.TextField(_self.RoomName[i]);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, "Mod Room Name");
                    _self.RoomName[i] = str;
                    UpdateRoomPoints();
                }
                
                EditorGUILayout.BeginHorizontal();
                if (i == 0)
                {
                    EditorGUILayout.LabelField(i + ":", GUILayout.MaxWidth(36));
                    EditorGUILayout.LabelField("X " + _self.Rooms[i].x, GUILayout.MinWidth(60));
                    EditorGUILayout.LabelField("Y " + _self.Rooms[i].y, GUILayout.MinWidth(60));
                    EditorGUILayout.LabelField("Z " + _self.Rooms[i].z, GUILayout.MinWidth(60));
                }
                else
                {
                    EditorGUI.BeginChangeCheck();
                    var pos = EditorGUILayout.Vector3Field(i + ":", _self.Rooms[i]);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(target, "Mod Room Rotation");

                        _self.Rooms[i] = pos;

                        UpdateRoomPoints();
                    }
                }

                if (_selectedRoom == i)
                {
                    EditorGUI.BeginChangeCheck();
                    var radius = EditorGUILayout.FloatField("Radius", _self.RoomViewRadius[i]);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(target, "Mod Room Radius");

                        _self.RoomViewRadius[i] = radius;

                        UpdateRoomPoints();
                    }
                }

                if (GUILayout.Button("View Gizmos", GUILayout.MaxWidth(120), GUILayout.MinWidth(120)))
                {
                    ViewGizmos(i);
                    return;
                }

                EditorGUILayout.EndHorizontal();

                if (_selectedRoom == i)
                {
                    EditorGUI.BeginChangeCheck();
                    var rotation = EditorGUILayout.Vector3Field("Rotation", _self.RoomRotationOffset[i]);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(target, "Mod Room Rotation");

                        _self.RoomRotationOffset[i] = rotation;
                        
                        ViewGizmos(i);
                    }
                }

                if (_selectedRoom == i)
                {
                    var rect = EditorGUILayout.BeginVertical();

                    if (!_editiList)
                    {
                        if (GUILayout.Button("\\/"))
                        {
                            _editiList = true;
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("/\\"))
                        {
                            _editiList = false;
                        }

                        if (GUILayout.Button("Add"))
                        {
                            _self.RoomHide[i].pointer.Add(0);
                            
                            EditorUtility.SetDirty(_self);
                            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                        }
                        
                        if(_self.RoomHide[i].pointer == null)
                            _self.RoomHide[i].pointer = new List<int>();

                        for (int j = 0; j < _self.RoomHide[i].pointer.Count; j++)
                        {
                            EditorGUILayout.BeginHorizontal();
                            
                            if (GUILayout.Button("X", GUILayout.MaxWidth(60), GUILayout.MinWidth(60)))
                            {
                                _self.RoomHide[i].pointer.RemoveAt(j);
                                
                                UpdateRoomPoints();
                                EditorUtility.SetDirty(_self);
                                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                                return;
                            }
                            
                            EditorGUI.BeginChangeCheck();
                            
                            var hide = EditorGUILayout.IntField(j.ToString(), _self.RoomHide[i].pointer[j]);

                            if (EditorGUI.EndChangeCheck())
                            {
                                Undo.RecordObject(target, "Mod Room Hide");

                                UpdateRoomPoints();
                                _self.RoomHide[i].pointer[j] = hide;
                            }
                            
                            EditorGUILayout.EndHorizontal();
                        }

                    }
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.Space();
                }
            }
            EditorGUIUtility.labelWidth = labelWidth;
        }

        private void CheckRoomRadius()
        {
            if (_self.RoomViewRadius == null)
                _self.RoomViewRadius = new List<float>();

            if (_self.RoomRotationOffset == null)
                _self.RoomRotationOffset = new List<Vector3>();

            if (_self.RoomHide == null)
                _self.RoomHide = new List<ListInt>();
            
            if (_self.RoomName == null)
                _self.RoomName = new List<string>();

            for (var i = _self.RoomViewRadius.Count; i < _self.Rooms.Count; i++)
            {
                _self.RoomViewRadius.Add(0);
            }

            for (var i = _self.RoomRotationOffset.Count; i < _self.Rooms.Count; i++)
            {
                _self.RoomRotationOffset.Add(Vector3.zero);
            }

            for (var i = _self.RoomHide.Count; i < _self.Rooms.Count; i++)
            {
                _self.RoomHide.Add(new ListInt());
            }

            for (var i = _self.RoomName.Count; i < _self.Rooms.Count; i++)
            {
                _self.RoomName.Add("Hotspot");
            }
        }

        private void ViewGizmos(int i)
        {
            if (_textureManager.Textures.Count < _self.Rooms.Count)
            {
                for (int j = _textureManager.Textures.Count; j < _self.Rooms.Count; j++)
                {
                    _textureManager.Textures.Add(null);
                }
            }

            UpdateRoomPoints();
            _self.MoveToRoom(i);

            _selectedRoom = i;
            Repaint();
        }

        private void DrawMap()
        {
            if (_selectedRoom >= _self.Rooms.Count)
                _selectedRoom = 0;

            for (int i = 0; i < _self.Rooms.Count; i++)
            {
                if (Handles.Button(_self.Rooms[i], Quaternion.identity, 1, 1, Handles.SphereHandleCap))
                {
                    _selectedRoom = i;
                    Repaint();
                    return;
                }

                if (_selectedRoom != i) continue;

                Handles.DrawWireDisc(_self.Rooms[i], Vector3.up, _self.RoomViewRadius[i]);
                Handles.DrawWireDisc(_self.Rooms[i], Vector3.right, _self.RoomViewRadius[i]);
                Handles.DrawWireDisc(_self.Rooms[i], Vector3.forward, _self.RoomViewRadius[i]);

                if (i == 0) continue;

                EditorGUI.BeginChangeCheck();
                var point = Handles.DoPositionHandle(_self.Rooms[i], Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, "Mod Room");
                    _self.Rooms[i] = point;

                    UpdateRoomPoints();
                }
            }
        }

        private void UpdateRoomPoints()
        {
            if (_canvas.transform.childCount > _self.Rooms.Count)
            {
                for (int i = 0; i < _canvas.transform.childCount; i++)
                {
                    DestroyImmediate(_canvas.transform.GetChild(i).gameObject);
                }
            }

            for (int i = 0; i < _self.Rooms.Count; i++)
            {
                SetupRoomHierarchy(i);
            }
        }

        private void SetupRoomHierarchy(int i)
        {
            Transform room;

            if (i >= _canvas.transform.childCount)
            {
                room = new GameObject("Room - " + i).transform;
                room.SetParent(_canvas.transform);
            }
            else
            {
                room = _canvas.transform.GetChild(i);
            }
            room.name = "Room - " + i;

            SetupRoomConnections(room, i);
        }

        private void SetupRoomConnections(Transform room, int i)
        {
            if (Application.isPlaying) return;

            if (room.childCount >= _self.Rooms.Count)
            {
                for (int j = 0; j < room.childCount; j++)
                {
                    DestroyImmediate(room.GetChild(j).gameObject);
                }
            }

            for (int j = 0; j < _self.Rooms.Count; j++)
            {
                Transform ch;
                RoomInteractionButton bt;
                if (j >= room.childCount)
                {
                    bt = PrefabUtility.InstantiatePrefab(_navButtonAsset) as RoomInteractionButton;
                    bt.transform.SetParent(room);
                    ch = bt.transform;
                }
                else
                {
                    ch = room.GetChild(j);
                    bt = ch.GetComponent<RoomInteractionButton>();
                }
                
                ch.name = "Connection (" + i + " -> " + j + ")";

                var dir = _self.Rooms[j] - _self.Rooms[i];

                ch.position = dir * _self.ButtonRadiusDistance;
                ch.LookAt(Vector3.zero);

                bt.NavigationMap = _self;
                bt.Id = j;
                bt.SetName(_self.RoomName[j]);

                EditorUtility.SetDirty(bt);

                ch.gameObject.SetActive(i != j);

                if (dir.magnitude > _self.RoomViewRadius[i])
                {
                    ch.gameObject.SetActive(false);
                }
                if (_self.RoomHide[i].pointer.Contains(j))
                {
                    ch.gameObject.SetActive(false);
                }
            }

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
    }
}