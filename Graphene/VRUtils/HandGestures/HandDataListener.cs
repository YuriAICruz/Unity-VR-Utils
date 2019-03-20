using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Graphene.VRUtils
{
    [RequireComponent(typeof(Manager))]
    public class HandDataListener : MonoBehaviour
    {
        private Manager _manager;
        private List<Vector3> _lastPositions;

        private float _dist;
        private float _angle, _lastAngle;
        private float _delta;
        private float _angleWaist;

        private void Awake()
        {
            _manager = GetComponent<Manager>();
        }

        private void Start()
        {
            _lastPositions = GetHandsPositions();
        }

        private void Update()
        {
            var pos = GetHandsPositions();
            for (int i = 0, n = _lastPositions.Count; i < n - 1; i++)
            {
                _dist = (pos[i] - pos[(i + 1) % n]).magnitude;

                GetAngle(pos, i, n);
            }

            _delta = _angle - _lastAngle;
            _lastAngle = _angle;// - _lastAngle * Time.deltaTime;
            
            _lastPositions = GetHandsPositions();
        }

        private void GetAngle(List<Vector3> pos, int i, int n)
        {
            var head = _manager.Head.transform.parent;
            var a = pos[i] - head.position;
            var b = pos[(i + 1) % n] - head.position;

            a = head.InverseTransformDirection(a);
            b = head.InverseTransformDirection(b);

            var c = a;
            var d = b;

            c.y = 0;
            d.y = 0;

            a.x = 0;
            b.x = 0;
            
            _angleWaist = Vector3.Angle(c, d);
            
            _angle = Vector3.Angle(a, b);
            var cross = Vector3.Cross(a, b);
            if (cross.x < 0) _angle = -_angle;
        }

        private List<Vector3> GetHandsPositions()
        {
            return _manager.Hands.Select(x => x.transform.position).ToList();
        }

        public float GetHandDistance()
        {
            return _dist;
        }

        public float GetHandsAngle()
        {
            return _angle;
        }
        public float GetHandsAngleFromWaist()
        {
            return _angleWaist;
        }

        public float GetHandsAngleDelta()
        {
            return _delta;
        }
    }
}