using UnityEngine;

namespace Graphene.VRUtils
{
    public class HandBehaviour : MonoBehaviour
    {
        public float GrabRange;

        private HandInteractible _interactible;

        private Renderer _renderer;
        private Material _material;

        private Collider _collider;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _material = _renderer.material;
            _collider = GetComponent<Collider>();
        }

        public void Grab(bool grab)
        {
            _material.color = grab ? Color.green : Color.white;
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
            _interactible = other.transform.GetComponent<HandInteractible>();

            if (_interactible == null) return;

            _interactible.OnCollisionEnter();
        }

        private void OnCollisionExit(Collision other)
        {
            _interactible = other.transform.GetComponent<HandInteractible>();

            if (_interactible == null) return;

            _interactible.OnCollisionExit();
        }

        private bool FitGrabbed()
        {
            if (_interactible.OnGrab(transform))
            {
                _renderer.enabled = false;
                DisableCollider();
                return true;
            }

            _interactible = null;
            return false;
        }

        void EnableCollider()
        {
            if (_collider == null) return;

            _collider.enabled = true;
        }

        void DisableCollider()
        {
            if (_collider == null) return;

            _collider.enabled = false;
        }

        private void Release()
        {
            _renderer.enabled = true;
            _interactible.Release();
            _interactible = null;
        }

        public void Trigger(bool trigger)
        {
            _material.color = trigger ? Color.red : Color.white;
            if (_interactible == null) return;

            _interactible.Trigger();
        }
    }
}