﻿using System;
using Graphene.Shader.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace Graphene.VRUtils
{
    [RequireComponent(typeof(Rigidbody))]
    public class PhysicsInteractible : HandInteractible
    {
        private float _throwForce = 64;

        public bool CanWorldReset = true;

        public Rigidbody _rigidbody;

        [SerializeField]
        private Vector3 _velocity;

        private Vector3 _lastPos;

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
            _collider.enabled = false;

            _lastPos = transform.position;

            return base.OnGrab(parent);
        }

        protected void SetRigidbodyDynamic()
        {
            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = true;
            
            _collider.enabled = true;
            
            transform.parent = _parent;
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
            
            SetRigidbodyDynamic();
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
        }

        protected virtual void FixedUpdate()
        {
            if (!_grabbed)
            {
                _velocity = Vector3.zero;
                _angularVelocity = Vector3.zero;
                return;
            }

            var t = transform;
            var pos = t.position;
            var rot = t.eulerAngles;
            
            var newV = (pos - _lastPos);

            _velocity += (newV - _velocity) * Time.deltaTime * 4.6f;
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