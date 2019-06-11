using UnityEngine;

namespace Graphene.VRUtils
{
    [RequireComponent(typeof(Rigidbody))]
    public class PhysicsInteractible : HandInteractible
    {
        public float Force = 5000;

        public bool CanWorldReset = true;

        protected Rigidbody _rigidbody;

        private Vector3 _velocity;

        private Vector3 _lastPos;

        private float _lastDelta;
        protected float _lastResetTime;

        protected override void Awake()
        {
            base.Awake();

            _rigidbody = GetComponent<Rigidbody>();
        }

        public override bool OnGrab(Transform parent)
        {
            _rigidbody.isKinematic = true;

            _lastPos = transform.position;

            return base.OnGrab(parent);
        }

        public override void Release()
        {
            _rigidbody.isKinematic = false;
            _rigidbody.velocity = _velocity * Force;

            base.Release();
        }

        private void Update()
        {
            if (!_rigidbody.isKinematic)
            {
                if (CanWorldReset && Manager.WorldReset && transform.position.y < Manager.WorldResetHeight)
                    ResetPosition();
                else
                    _lastResetTime = Time.timeSinceLevelLoad;

                return;
            }

            var pos = transform.position;
            var delta = Time.deltaTime;

            _velocity = (transform.position - _lastPos) * _lastDelta;

            _lastPos = pos;
            _lastDelta = delta;
        }

        protected override void ResetPosition()
        {
            if (Time.timeSinceLevelLoad - _lastResetTime <= 1) return;

            base.ResetPosition();
            _lastResetTime = Time.timeSinceLevelLoad;
        }
    }
}