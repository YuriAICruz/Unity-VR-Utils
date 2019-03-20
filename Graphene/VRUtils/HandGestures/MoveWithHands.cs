using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Graphene.VRUtils
{
    public class MoveWithHands : MonoBehaviour
    {
        public float Speed;

        private HandDataListener _handDataListener;
        private Manager _manager;

        public float WaistLimit;
        
        private void Awake()
        {
            _handDataListener = FindObjectOfType<HandDataListener>();
            _manager = FindObjectOfType<Manager>();
        }

        private void Update()
        {
            if(_manager.Hands.Select(x=>x.transform.forward.y).Sum() / _manager.Hands.Length > WaistLimit) return;
            Move(_handDataListener.GetHandsAngleDelta());
        }


        void Move(float delta)
        {
            var dir = _manager.Head.transform.forward * Mathf.Abs(delta) * Time.deltaTime * Speed;

            transform.InverseTransformDirection(dir);

            dir.y = 0;
            
            transform.TransformDirection(dir);
            
            transform.position += dir;
        }
    }
}