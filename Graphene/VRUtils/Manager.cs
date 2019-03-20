using Graphene.VRUtils;
using UnityEngine;

namespace Graphene.VRUtils
{
    public class Manager : MonoBehaviour
    {
        public InputDemo Input;

        public XrDevicePosition Head;

        public HandBehaviour[] Hands;

        private void Start()
        {
            Input = new InputDemo();
            Input.Init();

            Input.GrabL += (b) => Grab(0, b);
            Input.GrabR += (b) => Grab(1, b);
            
            Input.TriggerL += (b) => Trigger(0, b);
            Input.TriggerR += (b) => Trigger(1, b);
        }

        private void Trigger(int i, bool trigger)
        {
            if (i >= Hands.Length) return;
            Hands[i].Trigger(trigger);
        }

        private void Grab(int i, bool grab)
        {
            if (i >= Hands.Length) return;
            Hands[i].Grab(grab);
        }

        private void PrintTrigger(float axis)
        {
            if(axis <= 0) return;
            
            Debug.Log(":" + axis);
        }

        private void PrintAxis(Vector2 axis)
        {
            if(axis.magnitude <= 0) return;
            
            Debug.Log(":" + axis);
        }
    }
}