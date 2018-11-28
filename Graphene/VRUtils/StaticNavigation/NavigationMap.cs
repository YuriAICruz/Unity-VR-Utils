using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace Graphene.VRUtils.StaticNavigation
{
    [Serializable]
    public class ListInt
    {
        public ListInt()
        {
            hotspot = new List<HotspotReference>();
        }

        public List<HotspotReference> hotspot;
    }

    [Serializable]
    public class CustomSettings
    {
        public bool NormalizeDistances;
        public bool Spread;
        public bool IsPopupVideo;
        public VideoClip Clip;
        public string ClipName;
        public Vector3 CamRotation;
    }
    

    [Serializable]
    public class HotspotReference
    {
        public int pointer;
        public Vector3 offset;
    }

    public class NavigationMap : MonoBehaviour
    {
        private BaseNavigation[] _sphereTextureManager;

        [HideInInspector] public List<Vector3> Rooms;
        
        [HideInInspector] public List<Vector3> RoomRotationOffset;

        [HideInInspector] public List<ListInt> RoomShow;
        
        [HideInInspector] public List<string> RoomName;
        
        [HideInInspector] public List<CustomSettings> RoomCustomSettings;

        private GameObject _canvas;
        public float ButtonRadiusDistance = 10;

        private Orbiter _orbiter;

        private static int currentId;

        private void Awake()
        {
            SetupTextureManagers();

            _canvas = GameObject.Find("3DCanvas");

            _orbiter = FindObjectOfType<Orbiter>();
        }

        private void SetupTextureManagers()
        {
            _sphereTextureManager = FindObjectsOfType<BaseNavigation>();

            var sm = GetComponent<BaseNavigation>();

            foreach (var textureManager in _sphereTextureManager)
            {
                textureManager.Textures = sm.Textures;
                textureManager.TransitionTime = sm.TransitionTime;
            }
        }

        private void Start()
        {
            MoveToRoom(0);
            currentId = 0;
        }

        public void MoveToRoom(int id)
        {
            if (_canvas == null)
                _canvas = GameObject.Find("3DCanvas");
            if (_sphereTextureManager == null)
                SetupTextureManagers();
            if (_orbiter == null)
                _orbiter = FindObjectOfType<Orbiter>();

            _orbiter.SetRotation(RoomRotationOffset[id].y, RoomCustomSettings[id].CamRotation);

            for (int i = 0; i < Rooms.Count; i++)
            {
                _canvas.transform.GetChild(i).gameObject.SetActive(i == id);
            }
            foreach (var textureManager in _sphereTextureManager)
            {
                textureManager.ChangeTexture(id);
            }

            currentId = id;
        }

        public int GetCurrentId()
        {
            return currentId;
        }
    }
}