using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Graphene.VRUtils
{
    public class HandPosition : MonoBehaviour
    {
        public XRNode Point;

        private bool _tracking;
        private List<XRNodeState> _nodes;
        private Vector3 _position;
        public Vector3 Offset;
        private Quaternion _rotation;

        private void Start()
        {
//            transform.SetParent(Camera.main.transform);

            InputTracking.nodeAdded += NodeAdd;
            InputTracking.nodeRemoved += NodeRemove;
            _nodes = new List<XRNodeState>();
        }

        private void NodeRemove(XRNodeState node)
        {
            if (node.nodeType == Point)
                _tracking = false;
        }

        private void NodeAdd(XRNodeState node)
        {
            if (node.nodeType == Point)
                _tracking = true;
        }

        private void Update()
        {
            if (!_tracking || Point == XRNode.GameController || Point == XRNode.TrackingReference || Point == XRNode.HardwareTracker) return;

            InputTracking.GetNodeStates(_nodes);

            foreach (var node in _nodes)
            {
                if (node.nodeType != Point) continue;

                if (node.TryGetPosition(out _position))
                {
                    transform.localPosition = _position+Offset;
                }
                if (node.TryGetRotation(out _rotation))
                {
                    transform.localRotation = _rotation;
                }
            }
        }
    }
}