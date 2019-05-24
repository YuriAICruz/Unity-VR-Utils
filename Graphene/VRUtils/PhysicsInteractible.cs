using UnityEngine;

namespace Graphene.VRUtils
{
    [RequireComponent(typeof(Rigidbody))]
    public class PhysicsInteractible : HandInteractible
    {
        public float Force = 100;
        private Rigidbody _rigidbody;
        
        [SerializeField]
        private Vector3 _velocity;
        
        private Vector3 _lastPos;

        private float _lastDelta;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public override void OnGrab(Transform parent)
        {
            _rigidbody.isKinematic = true;
            
            _lastPos = transform.position;
            
            base.OnGrab(parent);
        }

        public override void Release()
        {
            _rigidbody.isKinematic = false;
            _rigidbody.velocity = _velocity * Force;
            
            base.Release();
        }

        private void Update()
        {
            if (!_rigidbody.isKinematic) return;

            var pos = transform.position;
            var delta = Time.deltaTime;

            _velocity = (transform.position - _lastPos) * _lastDelta;
            
            _lastPos = pos;
            _lastDelta = delta;
        }
    }
}