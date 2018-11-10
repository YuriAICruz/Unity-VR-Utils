using System.Collections.Generic;
using UnityEngine;

namespace Graphene.VRUtils.StaticNavigation
{
    [RequireComponent(typeof(SphereTextureManager))]
    public class NavigationMap : MonoBehaviour
    {
        private SphereTextureManager _sphereTextureManager;

        [HideInInspector] public List<Vector3> Rooms;

        private GameObject _canvas;
        public float ButtonRadiusDistance = 10;

        private void Awake()
        {
            _sphereTextureManager = GetComponent<SphereTextureManager>();

            _canvas = GameObject.Find("3DCanvas");
        }

        public void MoveToRoom(int id)
        {
            if (_canvas == null)
                _canvas = GameObject.Find("3DCanvas");
            if (_sphereTextureManager == null)
                _sphereTextureManager = GetComponent<SphereTextureManager>();

            for (int i = 0; i < Rooms.Count; i++)
            {
                _canvas.transform.GetChild(i).gameObject.SetActive(i == id);
            }
            _sphereTextureManager.ChangeTexture();
        }
    }
}