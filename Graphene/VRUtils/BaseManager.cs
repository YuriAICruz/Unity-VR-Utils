using System;
using UnityEngine;

namespace Graphene.VRUtils
{
    public abstract class BaseManager : MonoBehaviour
    {
        public Action<int, bool> Grab;
        public Action<int, bool> Trigger;
        public Action MapHardwareTrackers;
        
        public static bool WorldReset { get; protected set; }
        public static float WorldResetHeight { get; protected set; }

        [SerializeField] protected bool _worldReset = true;
        [SerializeField] protected float _worldResetHeight = 0.5f;

        public bool CanResetOntrigger;

        public XrDevicePosition Head;

        [HideInInspector]
        public HandBehaviour[] Hands;

        public Transform HeadHolder;
        public Transform InitialPosition;

        protected virtual void Awake()
        {
            Hands = FindObjectsOfType<HandBehaviour>();
        }
    }
}