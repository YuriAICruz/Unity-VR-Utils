using System.Collections.Generic;
using UnityEngine;

namespace Graphene.VRUtils.StaticNavigation
{
    [RequireComponent(typeof(SphereTextureManager))]
    public class NavigationMap : MonoBehaviour
    {
        private SphereTextureManager _sphereTextureManager;

        [HideInInspector]
        public List<Vector3> Rooms; 

        private void Awake()
        {
            _sphereTextureManager = GetComponent<SphereTextureManager>();
        }
    }
}