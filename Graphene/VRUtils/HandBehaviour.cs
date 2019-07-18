using System;
using UnityEngine;
using UnityEngine.XR;

namespace Graphene.VRUtils
{
    public enum ControllerInputIndex
    {
        LeftHand = 0,
        RightHand = 1
    }

    public class HandBehaviour : MonoBehaviour
    {
        public float GrabRange;

        private HandInteractible _interactible;

        private Collider _collider;

        public GameObject[] Controller;

        private Vector3 _movementVelocity;
        private Vector3 _lastPosition;

        public bool isFoot;
        private BaseManager _vrManager;
        public ControllerInputIndex _index;

        private XRNode _xrNode;
        private InputDevice _device;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _collider.enabled = true;

            switch (_index)
            {
                case ControllerInputIndex.LeftHand:
                    _xrNode = XRNode.LeftHand;
                    break;
                case ControllerInputIndex.RightHand:
                    _xrNode = XRNode.RightHand;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _vrManager = FindObjectOfType<BaseManager>();
            _vrManager.Grab += Grab;
            _vrManager.Trigger += Trigger;
        }

        public void Grab(int index, bool grab)
        {
            if ((int) _index != index) return;

            GrabFeedback(grab);
            if (_interactible != null)
            {
                if (!grab)
                {
                    Release();
                }

                return;
            }

            if (!grab)
            {
                DisableCollider();
                return;
            }

            var hits = Physics.SphereCastAll(transform.position, GrabRange, transform.forward, GrabRange);
            foreach (var hit in hits)
            {
                if (hit.transform == transform) continue;

                _interactible = hit.transform.GetComponent<HandInteractible>();

                if (_interactible == null) continue;

                if (FitGrabbed())
                {
                    return;
                }
            }

            EnableCollider();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!isFoot) return;

            var phyin = other.transform.GetComponent<PhysicsInteractible>();

            if (phyin == null) return;
            phyin._rigidbody.isKinematic = false;
            phyin._rigidbody.AddForce(-_movementVelocity * 2000);
        }

        public void Vibrate()
        {
            if(isFoot) return;
            
            _device = InputDevices.GetDeviceAtXRNode(_xrNode);
            
            if (_device.isValid)
            {
                HapticCapabilities hapcap = new HapticCapabilities();
                _device.TryGetHapticCapabilities(out hapcap);

                if (hapcap.supportsImpulse)
                {
                    _device.SendHapticImpulse(0, 0.5f);
                }
            }
        }

        private void FixedUpdate()
        {
            _movementVelocity = _lastPosition - transform.position;
            _lastPosition = transform.position;
        }

        private void OnCollisionExit(Collision other)
        {
//            _interactible = other.transform.GetComponent<HandInteractible>();
//
//            if (_interactible == null) return;
//
//            _interactible.OnCollisionExit();
        }

        private bool FitGrabbed()
        {
            Vibrate();
            
            if (_interactible.OnGrab(transform))
            {
                HideController();
                DisableCollider();
                return true;
            }

            _interactible = null;
            return false;
        }

        void EnableCollider()
        {
            if (_collider == null) return;

            //_collider.enabled = true;
        }

        void DisableCollider()
        {
            if (_collider == null) return;

            //_collider.enabled = false;
        }

        private void Release()
        {
            Vibrate();
            
            if (_interactible.Release())
            {
                ShowController();
                _interactible = null;
            }
        }

        void HideController()
        {
            foreach (var ctrl in Controller)
            {
                ctrl.SetActive(false);
            }
        }

        void ShowController()
        {
            foreach (var ctrl in Controller)
            {
                ctrl.SetActive(true);
            }
        }


        private void GrabFeedback(bool grab)
        {
        }

        private void TriggerFeedback(bool trigger)
        {
        }

        public void Trigger(int index, bool trigger)
        {
            if ((int) _index != index) return;
            TriggerFeedback(trigger);
            if (_interactible == null) return;

            _interactible.Trigger();
        }
    }
}