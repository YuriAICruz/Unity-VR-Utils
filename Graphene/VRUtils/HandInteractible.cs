using UnityEngine;

namespace Graphene.VRUtils
{
    public class HandInteractible : MonoBehaviour
    {
        private Transform _parent;
        private bool parentSet;

        public virtual void Trigger()
        {
        }

        public virtual void OnGrab(Transform parent)
        {
            if (!parentSet)
            {
                parentSet = true;
                _parent = transform.parent;
            }
            
            transform.SetParent(parent);

            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.Euler(new Vector3(45, 0, 0));
        }

        public virtual void Release()
        {
            transform.parent = _parent;
            //transform.localPosition = Vector3.zero;
        }
    }
}