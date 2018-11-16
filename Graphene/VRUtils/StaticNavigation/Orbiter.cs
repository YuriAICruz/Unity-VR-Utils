using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graphene.VRUtils.StaticNavigation
{
    public class Orbiter : MonoBehaviour
    {
        private Vector3 _rotation;

        public void SetRotation(Vector3 rotation)
        {
            _rotation = rotation;
        }

        public float GetYRotation()
        {
            return _rotation.y;
        }
    }
}