using System.Collections.Generic;
using UnityEngine;

namespace Graphene.VRUtils.StaticNavigation
{
    [RequireComponent(typeof(SphereTextureManager))]
    public class NavigationMap : MonoBehaviour
    {
        private BaseNavigation[] _sphereTextureManager;

        [HideInInspector] public List<Vector3> Rooms;

        private GameObject _canvas;
        public float ButtonRadiusDistance = 10;

        private void Awake()
        {
            SetupTextureManagers();

            _canvas = GameObject.Find("3DCanvas");
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