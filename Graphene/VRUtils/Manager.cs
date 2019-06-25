using System.Linq.Expressions;
using Graphene.VRUtils;
using UnityEngine;

namespace Graphene.VRUtils
{
    public class Manager : MonoBehaviour
    {
        public static bool WorldReset { get; private set; }
        public static float WorldResetHeight { get; private set; }

        [SerializeField] private bool _worldReset = true;
        [SerializeField] private float _worldResetHeight = 0.5f;

        public bool CanResetOntrigger;

        public InputDemo Input;

        public XrDevicePosition Head;

        public HandBehaviour[] Hands;

        public Transform HeadHolder;
        public Transform InitialPosition;

        protected virtual void Awake()
        {
            WorldReset = _worldReset;

            WorldResetHeight = _worldResetHeight;
        }

        protected virtual void Start()
        {
            Input = GetComponent<InputDemo>();
            Input.Init();

            Input.GrabL += (b) => Grab(0, b);
            Input.GrabR += (b) => Grab(1, b);

            Input.TriggerL += (b) => Trigger(0, b);
            Input.TriggerR += (b) => Trigger(1, b);

            Reset();
        }

        protected void Reset()
        {
            Debug.Log("Reset");

            if (transform.parent == null) return;
            
            var reseter = transform.parent.GetComponent<ResetHeadPosition>();

            if (reseter)
                reseter.ResetPosition();
            else
                HeadHolder.position = new Vector3(InitialPosition.position.x, HeadHolder.position.y, InitialPosition.position.z);
        }

        protected void Trigger(int i, bool trigger)
        {
            if (i >= Hands.Length) return;
            Hands[i].Trigger(trigger);
        }

        protected void Grab(int i, bool grab)
        {
            if (i >= Hands.Length) return;
            
            Hands[i].Grab(grab);

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