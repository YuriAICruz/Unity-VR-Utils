using UnityEngine;

namespace Graphene.VRUtils
{
    public class HandInteractible : MonoBehaviour
    {
        public Vector3 PositionOffset;
        public Vector3 RotationOffset = new Vector3(45, 0, 0);

        private Transform _parent;

        private bool parentSet;
        protected bool _grabbed;

        protected Quaternion _initialRotation;
        protected Vector3 _initialPosition;

        protected virtual void Awake()
        {
            _initialPosition = transform.position;
            _initialRotation = transform.rotation;
        }

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

        public virtual void Release()
        {
            _grabbed = false;
            transform.parent = _parent;
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