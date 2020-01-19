using System.Collections.Generic;
using Graphene.VRUtils.Oculus;
using UnityEngine;
using UnityEngine.XR;

namespace Graphene.VRUtils
{
    public enum HardwareTracker
    {
        LeftFoot = 1,
        RightFoot = 2,
        Waist = 3
    }

    public class XrDevicePosition : MonoBehaviour
    {
        public XRNode Point;

        private bool _tracking = true;
        private List<XRNodeState> _nodes;
        private Vector3 _position;
        public Vector3 Offset;
        private Quaternion _rotation;

        [SerializeField] private int _index;

        private BaseManager _manager;
        
#if OCULUS_QUEST
        [Header("Oculus Quest")]
        public bool isFinger;
        //public OVRPlugin.HandFinger finger;
        public OVRPlugin.BoneId bone;
        
        private OVRPlugin.HandState _currentHandState;

        public HandTracker handReference;
#endif

        private void Start()
        {
#if OCULUS_QUEST
            _currentHandState = new OVRPlugin.HandState();
#endif

            InputTracking.nodeAdded += NodeAdd;
            InputTracking.nodeRemoved += NodeRemove;

            _manager = FindObjectOfType<BaseManager>();
            if (Point == XRNode.HardwareTracker)
                _manager.MapHardwareTrackers += MapHardwareTrackers;

            _nodes = new List<XRNodeState>();
        }

        private void MapHardwareTrackers()
        {
            return;

//            if (Point != XRNode.HardwareTracker) return;
//
//
//            foreach (var node in _hardwareTrackers)
//            {
//                if (!node.TryGetPosition(out _position)) continue;
//
//                var d = _position - Camera.main.transform.position;
//
//                switch (_hardwareId)
//                {
//                    case HardwareTracker.LeftFoot:
//                        if ((d.x + d.z) / 2f > 0)
//                        {
//                            break;
//                        }
//
//                        continue;
//                    case HardwareTracker.RightFoot:
//                        if ((d.x + d.z) / 2f < 0)
//                        {
//                            break;
//                        }
//
//                        continue;
//                    case HardwareTracker.Waist:
//                        var pos = new Vector3[_hardwareTrackers.Count];
//
//                        var maxY = 0f;
//                        var canContinue = true;
//                        for (int i = 0; i < _hardwareTrackers.Count; i++)
//                        {
//                            if (!node.TryGetPosition(out pos[i]))
//                            {
//                                canContinue = false;
//                                break;
//                            }
//
//                            if (maxY < pos[i].y)
//                            {
//                                maxY = pos[i].y;
//                            }
//                        }
//
//                        if (!canContinue)
//                            continue;
//
//                        if (Math.Abs(d.y - maxY) < 1)
//                        {
//                            break;
//                        }
//
//                        continue;
//                    default:
//                        continue;
//                }
//
//                _hardwareuniqueID = node.uniqueID;
//            }
        }

        private void NodeRemove(XRNodeState node)
        {
            if (Point == XRNode.HardwareTracker)
            {
                return;
            }

            if (node.nodeType == Point)
                _tracking = false;
        }

        private void NodeAdd(XRNodeState node)
        {
            if (Point == XRNode.HardwareTracker)
            {
                _tracking = true;
                //return;
            }

            if (node.nodeType == Point)
            {
                _tracking = true;
//                if (Point == XRNode.HardwareTracker)
//                {
//                    _hardwareTrackers.Add(node);
//                    Debug.Log($"{node.tracked} - {node.nodeType} - {node.uniqueID} - {gameObject}");
//                }
            }
        }

        private void Update()
        {
            if (!_tracking || Point == XRNode.GameController || Point == XRNode.TrackingReference || (isFinger && (!handReference || !handReference.IsTracked)))
                return; // || Point == XRNode.HardwareTracker

            InputTracking.GetNodeStates(_nodes);

            var index = 0;
            foreach (var node in _nodes)
            {
                if (node.nodeType != Point) continue;

#if OCULUS_QUEST
                if (isFinger && (node.nodeType == XRNode.RightHand || node.nodeType == XRNode.LeftHand))
                {
                    UpdateHandPose();
                    return;
                }
#endif


//                if (Point == XRNode.HardwareTracker)
//                    Debug.Log(gameObject);

                if (_index != index)
                {
                    index++;
                    continue;
                }

                index++;

                if (node.TryGetPosition(out _position))
                {
                    transform.localPosition = _position + Offset;
                }

                if (node.TryGetRotation(out _rotation))
                {
                    transform.localRotation = _rotation;
                }
            }
        }

#if OCULUS_QUEST
        private void UpdateHandPose()
        {
            var point = handReference.Bones[(int)bone];
            
            transform.position = point.position;
            transform.rotation = point.rotation;
        }
#endif
    }
}