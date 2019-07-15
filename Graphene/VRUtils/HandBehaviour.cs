using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

namespace Graphene.VRUtils
{
    public class HandBehaviour : MonoBehaviour
    {
        public float GrabRange;

        private HandInteractible _interactible;

        private Collider _collider;

        public GameObject[] Controller;

        private Vector3 _movementVelocity;
        private Vector3 _lastPosition;

        public bool isFoot;
        

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _collider.enabled = true;
        }

        public void Grab(bool grab)
        {
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
            if(!isFoot) return;
            
            var phyin = other.transform.GetComponent<PhysicsInteractible>();
            
            if(phyin == null) return;
            phyin._rigidbody.isKinematic = false;
            phyin._rigidbody.AddForce(-_movementVelocity * 2000);
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

        public void Trigger(bool trigger)
        {
            TriggerFeedback(trigger);
            if (_interactible == null) return;

            _interactible.Trigger();
        }
    }
}