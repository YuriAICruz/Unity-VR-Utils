using UnityEngine;

namespace Graphene.VRUtils
{
    public class HandInteractible : MonoBehaviour
    {
        public float GlowDistance;
        
        public Vector3 PositionOffset;
        public Vector3 RotationOffset = new Vector3(45, 0, 0);

        protected Transform _parent;

        protected bool parentSet;
        protected bool _grabbed;

        private bool _close;
        
        public bool IsGrabbed()
        {
            return _grabbed;
        }

        protected Quaternion _initialRotation;
        protected Vector3 _initialPosition;
        private BaseManager _vrManager;

        protected virtual void Awake()
        {
            _initialPosition = transform.position;
            _initialRotation = transform.rotation;

            _vrManager = FindObjectOfType<BaseManager>();
            GlowDistance = _vrManager.Hands[0].GetComponent<SphereCollider>().radius * _vrManager.Hands[0].transform.localScale.x * 2;
        }

        protected virtual void Update()
        {
            CheckHandsDistance();
        }

        private void CheckHandsDistance()
        {
            var count = 0;
            for (int i = 0; i < _vrManager.Hands.Length; i++)
            {
                var dist = (_vrManager.Hands[i].transform.position - transform.position).magnitude;
                
                if (!_close && dist <= GlowDistance)
                {
                    _close = true;
                    HandClose();
                    return;
                }
                
                if (_close && dist > GlowDistance)
                {
                    count++;
                }
            }
            
            if (_close && count == _vrManager.Hands.Length)
            {
                _close = false;
                HandFar();
            }
        }
        
        protected virtual void HandClose(){}
        protected virtual void HandFar(){}

        public virtual void Trigger()
        {
        }

        public virtual bool OnGrab(Transform parent)
        {
            _grabbed = true;
            if (!parentSet)
            {
                parentSet = true;
                _parent = transform.parent;
            }

            transform.SetParent(parent);

            transform.localPosition = PositionOffset;
            transform.localRotation = Quaternion.Euler(RotationOffset);

            return true;
        }

        public virtual bool Release()
        {
            _grabbed = false;
            transform.parent = _parent;
            return true;
            //transform.localPosition = Vector3.zero;
        }


        protected virtual void ResetPosition()
        {
            transform.position = _initialPosition;
            transform.rotation = _initialRotation;
        }

        public virtual void OnCollisionEnter()
        {
        }

        public virtual void OnCollisionExit()
        {
        }
    }
}