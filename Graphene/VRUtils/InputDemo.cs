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
                    break;
                case 1:
                    GrabL?.Invoke(false);
                    break;
                case 2:
                    GrabR?.Invoke(true);
                    break;
                case 3:
                    GrabR?.Invoke(false);
                    break;
                case 4:
                    TriggerL?.Invoke(true);
                    break;
                case 5:
                    TriggerL?.Invoke(false);
                    break;
                case 6:
                    TriggerR?.Invoke(true);
                    break;
                case 7:
                    TriggerR?.Invoke(false);
                    break;
            }
        }
    }
}