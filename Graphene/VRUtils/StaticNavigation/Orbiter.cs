using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graphene.VRUtils.StaticNavigation
{
    public class Orbiter : MonoBehaviour
    {
        private float _rotation;

        private Vector3 _selfRotation;

        public void SetRotation(float rotation, Vector3 selfRotation)
        {
            _rotation = rotation;
            _selfRotation = selfRotation;
        }

        public float GetYRotation()
        {
            return _rotation;
        }

        public void Rotate()
        {
            transform.eulerAngles = _selfRotation;
        }
    }
}