using Graphene.UiGenerics;
using Graphene.VRUtils;
using UnityEngine;

namespace Graphene.VRUtils.Presentation
{
    public class EyeGizmo : ImageView
    {
        private HeadDirection _head;

        Vector3 nofwd = new Vector3(1, 1, 0);

        void Setup()
        {
            _head = FindObjectOfType<HeadDirection>();
            _head.Counter += UpdateFill;
        }

        void UpdateFill(float v)
        {
            Image.fillAmount = v;
        }

        void Update()
        {
            transform.position = _head.GizmoPos; //Vector3.Scale(_head.GizmoPos, nofwd) + Vector3.forward * transform.position.z;
        }
    }
}