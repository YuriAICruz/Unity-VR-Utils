using System;
using System.Collections;
using System.Collections.Generic;
using Graphene.InputManager;
using UnityEngine;

namespace Graphene.VRUtils
{
    [SerializeField]
    public class InputDemo : InputSystem
    {
        public bool GrabLState;
        public bool GrabRState;
        public bool TriggerLState;
        public bool TriggerRState;
        
        public event Action<bool> GrabL, GrabR;
        public event Action<bool> TriggerL, TriggerR;

        protected override void ExecuteCombo(int id)
        {
            if (debug)
                Debug.Log(id);
            
            switch (id)
            {
                case 0:
                    GrabL?.Invoke(true);
                    GrabLState = true;
                    break;
                case 1:
                    GrabL?.Invoke(false);
                    GrabLState = false;
                    break;
                case 2:
                    GrabR?.Invoke(true);
                    GrabRState = true;
                    break;
                case 3:
                    GrabR?.Invoke(false);
                    GrabRState = false;
                    break;
                case 4:
                    TriggerL?.Invoke(true);
                    TriggerLState = true;
                    break;
                case 5:
                    TriggerL?.Invoke(false);
                    TriggerLState = false;
                    break;
                case 6:
                    TriggerR?.Invoke(true);
                    TriggerRState = true;
                    break;
                case 7:
                    TriggerR?.Invoke(false);
                    TriggerRState = false;
                    break;
            }
        }
    }
}