using System;
using Graphene.Shader.Scripts;
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
        protected TransitionOutlineMaterialManager _outline;
        protected Collider _collider;

        protected override void Awake()
        {
            base.Awake();

            _outline = GetComponent<TransitionOutlineMaterialManager>();
            if (!_outline)
            {
                _outline = transform.GetComponentInChildren<TransitionOutlineMaterialManager>();
            }
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
        }

        public override bool OnGrab(Transform parent)
        {
            if (_outline)
                _outline.HideOutline();

            _rigidbody.isKinematic = true;

            _lastPos = transform.position;

            AnalyticsSaver.SaveData("on_interact", gameObject.name, "on_grab");
            
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
            _rigidbody.velocity = _velocity * Force;

            AnalyticsSaver.SaveData("on_interact", gameObject.name, "on_release");
            
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

            var pos = transform.position;
            var delta = Time.deltaTime;

            _velocity = (transform.position - _lastPos) * _lastDelta;

            _lastPos = pos;
            _lastDelta = delta;
        }

        public override void ResetPosition()
        {
            if (Time.timeSinceLevelLoad - _lastResetTime <= 1) return;

            base.ResetPosition();
            _lastResetTime = Time.timeSinceLevelLoad;
        }
    }
}