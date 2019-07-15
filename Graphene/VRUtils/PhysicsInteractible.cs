using System;
using Graphene.Shader.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace Graphene.VRUtils
{
    [RequireComponent(typeof(Rigidbody))]
    public class PhysicsInteractible : HandInteractible
    {
        private float _throwForce = 36;

        public bool CanWorldReset = true;

        public Rigidbody _rigidbody;

        [SerializeField]
        private Vector3 _velocity;

        private Vector3 _lastPos;

        private float _lastDelta;
        protected float _lastResetTime;
        protected TransitionOutlineMaterialManager _outline;
        protected Collider _collider;
        private Vector3 _angularVelocity;
        private Vector3 _lastRot;

        protected override void Awake()
        {
            base.Awake();

            _outline = GetComponent<TransitionOutlineMaterialManager>();
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
        }

        public override bool OnGrab(Transform parent)
        {
            if (_outline)
                _outline.HideOutline();

            _rigidbody.isKinematic = true;

            _lastPos = transform.position;

            return base.OnGrab(parent);
        }

        protected override void HandClose()
        {
            if(!_collider.enabled) return;
            
            base.HandClose();

            if (_outline)
                _outline.ShowOutline();
        }

        protected override void HandFar()
        {
            if(!_collider.enabled) return;
            
            base.HandFar();

            if (_outline)
                _outline.HideOutline();
        }

        public override bool Release()
        {
            if (_outline)
                _outline.HideOutline();
            
            _rigidbody.isKinematic = false;
            _rigidbody.velocity = _velocity * _throwForce;
            _rigidbody.angularVelocity = _angularVelocity * 0.10f;

            return base.Release();
        }

        protected override void Update()
        {
            base.Update();

            if (!_rigidbody.isKinematic)
            {
                if (CanWorldReset && Manager.WorldReset && transform.position.y < Manager.WorldResetHeight)
                    ResetPosition();
                else
                    _lastResetTime = Time.timeSinceLevelLoad;

                return;
            }

            var delta = Time.deltaTime;

            //_velocity = (transform.position - _lastPos) * _lastDelta;

            _lastDelta = delta;
        }

        protected virtual void FixedUpdate()
        {
            var pos = transform.position;
            var rot = transform.eulerAngles;
            
            _velocity = (pos - _lastPos);
            _angularVelocity = (rot - _lastRot);
            
            _lastPos = pos;
            _lastRot = rot;
        }

        protected override void ResetPosition()
        {
            if (Time.timeSinceLevelLoad - _lastResetTime <= 1) return;

            base.ResetPosition();
            _lastResetTime = Time.timeSinceLevelLoad;
        }
    }
}