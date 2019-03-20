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

        public LineRenderer[] LineRenderers;
        private float _dist;
        private float _angle, _lastAngle;
        private float _delta;

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
                LineRenderers[0].SetPosition(0, pos[i]);
                LineRenderers[0].SetPosition(1, pos[(i + 1) % n]);

                LineRenderers[1].SetPosition(0, pos[i]);
                LineRenderers[1].SetPosition(1, _lastPositions[i]);

                _dist = (pos[i] - pos[(i + 1) % n]).magnitude;
                var cross = Vector3.Cross(pos[i] - pos[(i + 1) % n], pos[i] - _manager.Head.transform.position);

                GetAngle(pos, i, n);

                LineRenderers[2].SetPosition(0, pos[i]);
                LineRenderers[2].SetPosition(1, pos[i] + cross * 10);
            }

            LineRenderers[3].SetPosition(0, _manager.Head.transform.position + Vector3.down * 0.5f);
            LineRenderers[3].SetPosition(1, _manager.Head.transform.position + _manager.Head.transform.forward * 10 + Vector3.down * 0.5f);

            LineRenderers[3].startColor = Color.Lerp(Color.red, Color.blue, Mathf.Abs(_angle - _lastAngle));

            _delta = _angle - _lastAngle;
            _lastAngle = _angle;// - _lastAngle * Time.deltaTime;
            
            _lastPositions = GetHandsPositions();
        }

        private void GetAngle(List<Vector3> pos, int i, int n)
        {
            var head = _manager.Head.transform;
            var a = pos[i] - head.position;
            var b = pos[(i + 1) % n] - head.position;

            a = head.InverseTransformDirection(a);
            b = head.InverseTransformDirection(b);

            a.x = 0;
            b.x = 0;
            
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

        public float GetHandsAngleDelta()
        {
            return _delta;
        }
    }
}