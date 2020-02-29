using System;
using System.Linq.Expressions;
using Graphene.VRUtils;
using UnityEngine;

namespace Graphene.VRUtils
{
    public class Manager : BaseManager
    {
        public InputDemo Input;

        protected override void Awake()
        {
            WorldReset = _worldReset;

            WorldResetHeight = _worldResetHeight;
        }

        protected virtual void Start()
        {
            Input = GetComponent<InputDemo>();
            Input.Init();

            Input.GrabL += (b) => OnGrabHit(0, b);
            Input.GrabR += (b) => OnGrabHit(1, b);

            Input.TriggerL += (b) => OnTriggerHit(0, b);
            Input.TriggerR += (b) => OnTriggerHit(1, b);

            Reset();
        }

        protected void Reset()
        {
#if DEV_MODE
            Debug.Log("Reset");
#endif

            if (transform.parent == null) return;
            
            var reseter = transform.parent.GetComponent<ResetHeadPosition>();

            if (reseter)
                reseter.ResetPosition();
            else
                HeadHolder.position = new Vector3(InitialPosition.position.x, HeadHolder.position.y, InitialPosition.position.z);
        }

        protected virtual void OnTriggerHit(int i, bool trigger)
        {
            Grab?.Invoke(i, trigger);
        }

        protected virtual void OnGrabHit(int i, bool grab)
        {
            Grab?.Invoke(i, grab);
            
            if (CanResetOntrigger && Input.GrabLState && Input.GrabRState)
                Reset();
        }

        private void PrintTrigger(float axis)
        {
            if (axis <= 0) return;

            Debug.Log(":" + axis);
        }

        private void PrintAxis(Vector2 axis)
        {
            if (axis.magnitude <= 0) return;

            Debug.Log(":" + axis);
        }
    }
}