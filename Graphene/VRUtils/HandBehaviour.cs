using UnityEngine;

namespace Graphene.VRUtils
{
    public class HandBehaviour : MonoBehaviour
    {
        public float GrabRange;

        private Transform _grabbed;
        private HandInteractible _interactible;

        private Renderer _renderer;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
        }

        public void Grab(bool grab)
        {
            if (_grabbed != null)
            {
                if (!grab)
                {
                    Realease();
                }
                
                return;
            }
            
            if (!grab) return;

            var hits = Physics.SphereCastAll(transform.position, GrabRange, transform.forward, GrabRange);
            foreach (var hit in hits)
            {
                if(hit.transform == transform) continue;
                
                _grabbed = hit.transform;
                _interactible = _grabbed.GetComponent<HandInteractible>();
                FitGrabbed();
                return;
            }
        }

        private void FitGrabbed()
        {
            _renderer.enabled = false;
            _grabbed.SetParent(transform);
            _grabbed.localPosition = Vector3.zero;
            _grabbed.localRotation = Quaternion.Euler(new Vector3(45,0,0));
        }

        private void Realease()
        {
            _renderer.enabled = true;
            _grabbed.SetParent(null);
            _grabbed = null;
        }

        public void Trigger(bool trigger)
        {
            if(_interactible == null) return;
            
            _interactible.Trigger();
        }
    }
}