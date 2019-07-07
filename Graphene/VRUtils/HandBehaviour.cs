using System.Linq;
using UnityEngine;

namespace Graphene.VRUtils
{
    public class HandBehaviour : MonoBehaviour
    {
        public float GrabRange;

        private HandInteractible _interactible;

        private Collider _collider;

        public GameObject[] Controller;
        public float GlowDistance = 10;

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

            var hits = Physics.OverlapSphere(transform.position, GrabRange); //TODO
            hits = hits.ToList().OrderBy(x => (transform.position - x.transform.position).magnitude).ToArray();

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
//            _interactible = other.transform.GetComponent<HandInteractible>();
//
//            if (_interactible == null) return;

//            _interactible.OnCollisionEnter();
        }

        private void OnCollisionExit(Collision other)
        {
//            TODO: redudant methods OnCollisionExit and OnCollisionEnter are called by Unity internal physics system
//            _interactible = other.transform.GetComponent<HandInteractible>();
//
//            if (_interactible == null) return;

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