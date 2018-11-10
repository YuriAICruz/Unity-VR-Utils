using System.Collections.Generic;
using Graphene.VRUtils.Presentation;
using UnityEditor;
using UnityEngine;

namespace Graphene.VRUtils.StaticNavigation
{
    [CustomEditor(typeof(NavigationMap))]
    public class NavigationMapInspector : Editor
    {
        private NavigationMap _self;
        private SphereTextureManager _textureManager;

        private bool _viewMap;

        private int _selectedRoom;
        private GameObject _canvas;
        private RoomInteractionButton _navButtonAsset;

        private void Awake()
        {
            _self = target as NavigationMap;
            
            if(_self.Rooms == null)
                _self.Rooms = new List<Vector3>();
            
            _textureManager = _self.GetComponent<SphereTextureManager>() ?? _self.gameObject.AddComponent<SphereTextureManager>();

            _canvas = GameObject.Find("3DCanvas");

            _navButtonAsset = Resources.Load<RoomInteractionButton>("UI/NavButton");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

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
                if (i == 0)
                {
                    EditorGUILayout.LabelField(i + ":", GUILayout.MaxWidth(36));
                    EditorGUILayout.LabelField("X " + _self.Rooms[i].x, GUILayout.MinWidth(60));
                    EditorGUILayout.LabelField("Y " + _self.Rooms[i].y, GUILayout.MinWidth(60));
                    EditorGUILayout.LabelField("Z " + _self.Rooms[i].z, GUILayout.MinWidth(60));
                }
                else
                {
                    _self.Rooms[i] = EditorGUILayout.Vector3Field(i + ":", _self.Rooms[i]);
                }
                if (GUILayout.Button("View Gizmos", GUILayout.MaxWidth(120), GUILayout.MinWidth(120)))
                {
                    ViewGizmos(i);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUIUtility.labelWidth = labelWidth;
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
            
            _self.MoveToRoom(i);
        }

        private void OnSceneGUI()
        {
            if (_viewMap)
                DrawMap();
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
                }
                
                if(i == 0) continue;
                
                if(_selectedRoom != i) continue;
                
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
                    bt = Instantiate(_navButtonAsset, room);
                    ch = bt.transform;
                }
                else
                {    
                    ch = room.GetChild(j);
                    bt = ch.GetComponent<RoomInteractionButton>();
                }
                ch.name = "Connection (" + i + " -> " + j + ")";

                ch.gameObject.SetActive(i!=j);
                
                var dir = _self.Rooms[j] - _self.Rooms[i];

                ch.position = dir * _self.ButtonRadiusDistance;
                ch.LookAt(Vector3.zero);

                bt.NavigationMap = _self;
                bt.Id = j;
            }
        }
    }
}