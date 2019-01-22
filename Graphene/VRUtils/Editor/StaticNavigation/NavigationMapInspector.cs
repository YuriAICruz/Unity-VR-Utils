using System.Collections.Generic;
using System.Linq;
using Graphene.VRUtils.Presentation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

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
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(i + ":", GUILayout.MaxWidth(36));

                EditorGUI.BeginChangeCheck();
                var nd = EditorGUILayout.Toggle("Norm", _self.RoomCustomSettings[i].NormalizeDistances, GUILayout.MaxWidth(48));
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, "Mod Room NormalizeDistances");
                    _self.RoomCustomSettings[i].NormalizeDistances = nd;
                    EditorUtility.SetDirty(_self);
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                    UpdateRoomPoints();
                }


                if (_self.RoomCustomSettings[i].NormalizeDistances)
                {
                    var lw = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 46;
                    EditorGUI.BeginChangeCheck();
                    var spd = EditorGUILayout.Toggle("Spread", _self.RoomCustomSettings[i].Spread, GUILayout.MaxWidth(62));
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(target, "Mod Room Spread");
                        _self.RoomCustomSettings[i].Spread = spd;
                        EditorUtility.SetDirty(_self);
                        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                        UpdateRoomPoints();
                    }
                    EditorGUIUtility.labelWidth = lw;
                }
                else
                {
                    _self.RoomCustomSettings[i].Spread = false;
                }
                
                EditorGUI.BeginChangeCheck();
                
//                _self.RoomNames[i].names[0] = _self.RoomName[i];
//                _self.RoomNames[i].names[1] = _self.RoomName[i] + "_ES";
                
                var strPt = EditorGUILayout.TextField(_self.RoomNames[i].names[0]);
                var strEs = EditorGUILayout.TextField(_self.RoomNames[i].names[1]);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, "Mod Room Names");
                    _self.RoomNames[i].names[1] = strEs;
                    _self.RoomNames[i].names[0] = strPt;
                    EditorUtility.SetDirty(_self);
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                }
                
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                if (i == 0)
                {
                    //EditorGUILayout.LabelField(i + ":", GUILayout.MaxWidth(36));
                    EditorGUILayout.LabelField("X " + _self.Rooms[i].x, GUILayout.MinWidth(60));
                    EditorGUILayout.LabelField("Y " + _self.Rooms[i].y, GUILayout.MinWidth(60));
                    EditorGUILayout.LabelField("Z " + _self.Rooms[i].z, GUILayout.MinWidth(60));
                }
                else
                {
                    EditorGUI.BeginChangeCheck();
                    var pos = EditorGUILayout.Vector3Field("", _self.Rooms[i]);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(target, "Mod Room Rotation");

                        _self.Rooms[i] = pos;
                        EditorUtility.SetDirty(_self);
                        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

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
                    EditorGUILayout.LabelField("Rotations");


                    EditorGUI.BeginChangeCheck();
                    var rotation = EditorGUILayout.Vector3Field("", _self.RoomRotationOffset[i]);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(target, "Mod Room Rotation");

                        _self.RoomRotationOffset[i] = rotation;
                        EditorUtility.SetDirty(_self);
                        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

                        ViewGizmos(i);
                    }

                    EditorGUI.BeginChangeCheck();
                    var rot = EditorGUILayout.Vector3Field("Cam ", _self.RoomCustomSettings[i].CamRotation);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(target, "Mod Room Cam Rotation");

                        _self.RoomCustomSettings[i].CamRotation = rot;
                        EditorUtility.SetDirty(_self);
                        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
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
                            _self.RoomShow[i].hotspot.Add(new HotspotReference() {pointer = -1, offset = Vector3.zero});

                            EditorUtility.SetDirty(_self);
                            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                        }

                        if (_self.RoomShow[i].hotspot == null)
                            _self.RoomShow[i].hotspot = new List<HotspotReference>();

                        for (int j = 0; j < _self.RoomShow[i].hotspot.Count; j++)
                        {
                            EditorGUILayout.BeginHorizontal();

                            if (GUILayout.Button("X", GUILayout.MaxWidth(60), GUILayout.MinWidth(60)))
                            {
                                _self.RoomShow[i].hotspot.RemoveAt(j);

                                UpdateRoomPoints();
                                EditorUtility.SetDirty(_self);
                                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                                return;
                            }

                            EditorGUI.BeginChangeCheck();

                            var hide = EditorGUILayout.IntField(j.ToString(), _self.RoomShow[i].hotspot[j].pointer);

                            if (!_self.RoomShow[i].hotspot.Exists(x => x.pointer == hide))
                            {
                                if (EditorGUI.EndChangeCheck())
                                {
                                    Undo.RecordObject(target, "Mod Room Reference");

                                    _self.RoomShow[i].hotspot[j].pointer = hide;
                                    UpdateRoomPoints();
                                }
                            }
                            else
                            {
                                EditorGUI.BeginChangeCheck();

                                var offs = EditorGUILayout.Vector3Field(j.ToString(), _self.RoomShow[i].hotspot[j].offset);

                                if (EditorGUI.EndChangeCheck())
                                {
                                    Undo.RecordObject(target, "Mod Room Reference Offset");

                                    _self.RoomShow[i].hotspot[j].offset = offs;
                                    UpdateRoomPoints();
                                }
                            }

                            EditorGUILayout.EndHorizontal();
                        }
                    }

                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.BeginHorizontal();

                EditorGUI.BeginChangeCheck();
                var tlw = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 108;
                var pvd = EditorGUILayout.Toggle("Has Popup Video", _self.RoomCustomSettings[i].IsPopupVideo, GUILayout.MaxWidth(128));
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, "Mod Room IsPopupVideo");
                    _self.RoomCustomSettings[i].IsPopupVideo = pvd;

                    EditorUtility.SetDirty(_self);
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                }
                EditorGUIUtility.labelWidth = tlw;
                if (pvd)
                {
                    EditorGUI.BeginChangeCheck();
                    var clip = EditorGUILayout.ObjectField("Clip", _self.RoomCustomSettings[i].Clip, typeof(VideoClip), false) as VideoClip;
                    var clipName = EditorGUILayout.TextField("ClipName", _self.RoomCustomSettings[i].ClipName);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(target, "Mod Room PopupVideo");
                        _self.RoomCustomSettings[i].Clip = clip;
                        _self.RoomCustomSettings[i].ClipName = clipName;

                        UpdateRoomPoints();

                        EditorUtility.SetDirty(_self);
                        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                    }
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();

                if (_selectedRoom == i)
                    EditorGUILayout.Space();
            }
            EditorGUIUtility.labelWidth = labelWidth;
        }

        private void CheckRoomRadius()
        {
            if (_self.RoomRotationOffset == null)
                _self.RoomRotationOffset = new List<Vector3>();

            if (_self.RoomShow == null)
                _self.RoomShow = new List<ListInt>();

            if (_self.RoomNames == null)
            {
                _self.RoomNames = new List<RoomNamesReference>();
                //_self.RoomCustomSettings = new List<CustomSettings>();
            }

            for (var i = _self.RoomRotationOffset.Count; i < _self.Rooms.Count; i++)
            {
                _self.RoomRotationOffset.Add(Vector3.zero);
            }

            for (var i = _self.RoomShow.Count; i < _self.Rooms.Count; i++)
            {
                _self.RoomShow.Add(new ListInt());
            }

            for (var i = _self.RoomNames.Count; i < _self.Rooms.Count; i++)
            {
                _self.RoomNames.Add(new RoomNamesReference());
            }

            for (var i = _self.RoomCustomSettings.Count; i < _self.Rooms.Count; i++)
            {
                _self.RoomCustomSettings.Add(new CustomSettings());
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
                if (_self.RoomCustomSettings[i].NormalizeDistances)
                    dir = dir.normalized * 10;

                ch.position = dir * _self.ButtonRadiusDistance;
                ch.LookAt(Vector3.zero);

                bt.NavigationMap = _self;
                bt.Id = j;
                bt.Names = _self.RoomNames[j].names.ToArray();
                bt.SetName(_self.RoomNames[j].names.ToArray());

                bt.IsPopupVideo = _self.RoomCustomSettings[j].IsPopupVideo;

                if (bt.IsPopupVideo)
                {
                    bt.Clip = _self.RoomCustomSettings[j].Clip;
                    bt.ClipName = _self.RoomCustomSettings[j].ClipName;
                }
                else
                {
                    bt.Clip = null;
                    bt.ClipName = "";
                }


                ch.gameObject.SetActive(i != j);

                ch.gameObject.SetActive(false);

                if (_self.RoomShow[i].hotspot.Exists(x => x.pointer == j))
                {
                    ch.gameObject.SetActive(true);

                    var pos = ch.position + _self.RoomShow[i].hotspot.Find(x => x.pointer == j).offset;
                    ch.position = pos;
                    ch.LookAt(Vector3.zero);

                    Handles.DrawLine(_self.Rooms[i], pos);
                }

                EditorUtility.SetDirty(bt);
            }

            if (_self.RoomCustomSettings[i].Spread)
            {
                SpreadConnections(room, i);
            }

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        private void SpreadConnections(Transform room, int i)
        {
            var chs = new List<GameObject>();

            for (int j = 0; j < room.childCount; j++)
            {
                chs.Add(room.GetChild(j).gameObject);
            }

            chs = chs.FindAll(g => g.activeSelf);

            for (var j = 0; j < chs.Count; j++)
            {
                chs[j].transform.position = Quaternion.AngleAxis(j / (float) chs.Count * 360, Vector3.up) * room.forward * _self.ButtonRadiusDistance * 10;
                chs[j].transform.LookAt(Vector3.zero);
            }
        }
    }
}