using System;
using System.Collections.Generic;
using UnityEngine;

namespace Graphene.VRUtils.StaticNavigation
{
    [Serializable]
    public class ListInt
    {
        public ListInt()
        {
            pointer = new List<int>();
        }

        public List<int> pointer;
    }

    public class NavigationMap : MonoBehaviour
    {
        private BaseNavigation[] _sphereTextureManager;

        [HideInInspector] public List<Vector3> Rooms;
        [HideInInspector] public List<float> RoomViewRadius;
        [HideInInspector] public List<Vector3> RoomRotationOffset;

        [HideInInspector] public List<ListInt> RoomHide;

        private GameObject _canvas;
        public float ButtonRadiusDistance = 10;

        private Orbiter _orbiter;

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
        }

        public void MoveToRoom(int id)
        {
            if (_canvas == null)
                _canvas = GameObject.Find("3DCanvas");
            if (_sphereTextureManager == null)
                SetupTextureManagers();
            if (_orbiter == null)
                _orbiter = FindObjectOfType<Orbiter>();

            _orbiter.SetRotation(RoomRotationOffset[id]);


            for (int i = 0; i < Rooms.Count; i++)
            {
                _canvas.transform.GetChild(i).gameObject.SetActive(i == id);
            }
            foreach (var textureManager in _sphereTextureManager)
            {
                textureManager.ChangeTexture(id);
            }
        }
    }
}