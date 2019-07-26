using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR;
using Random = UnityEngine.Random;

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

        public LayerMask HittableLayer;

        private bool _grabbing;

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

            _grabbing = grab;

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

            if (phyin == null)
            {
                Vibrate(1, 0.06f);
                return;
            }

            phyin._rigidbody.isKinematic = false;
            phyin._rigidbody.AddForce(-_movementVelocity * 2000);

            Vibrate(2, 0.12f);
        }

        public void Vibrate(float intensity, float duration = 0)
        {
            if (isFoot) return;

            if (duration > 0)
            {
                StartCoroutine(Vibration(intensity, duration));
                return;
            }

            Vibration(intensity);
        }

        private void Vibration(float intensity)
        {
            _device = InputDevices.GetDeviceAtXRNode(_xrNode);

            if (_device.isValid)
            {
                HapticCapabilities hapcap = new HapticCapabilities();
                _device.TryGetHapticCapabilities(out hapcap);

                if (hapcap.supportsImpulse)
                {
                    _device.SendHapticImpulse(0, intensity);
                }
            }
        }

        IEnumerator Vibration(float intensity, float duration)
        {
            var t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;

                Vibration(intensity);

                yield return null;
            }
        }

        private void FixedUpdate()
        {
            _movementVelocity = _lastPosition - transform.position;
            _lastPosition = transform.position;

            if (!_grabbing) return;

            var hits = Physics.OverlapSphere(transform.position, Mathf.Max(_collider.bounds.size.x, _collider.bounds.size.z), HittableLayer);
            if (hits.Length == 0) return;

            var pos = transform.position;

            var vibrated = false;
            foreach (var hit in hits)
            {
                if (_interactible != null && hit.gameObject == _interactible.gameObject) continue;

                var rb = hit.GetComponent<Rigidbody>();

                if (!rb)
                {
                    if (!vibrated)
                        Vibrate(1, 0.06f);
                    vibrated = true;
                    continue;
                }

                rb.isKinematic = false;
                rb.useGravity = true;
                rb.AddForce((hit.transform.position - transform.position).normalized * 800);
                rb.angularVelocity += Random.insideUnitSphere * 40;

                if (!vibrated)
                    Vibrate(2, 0.12f);
                
                vibrated = true;
            }
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
            Vibrate(1f, 0.12f);

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
            Vibrate(0.5f, 0.08f);

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