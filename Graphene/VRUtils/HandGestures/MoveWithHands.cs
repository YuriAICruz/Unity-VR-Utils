using UnityEditor;
using UnityEngine;

namespace Graphene.VRUtils
{
    public class MoveWithHands : MonoBehaviour
    {
        public float Speed;

        private HandDataListener _handDataListener;
        private Manager _manager;

        private void Awake()
        {
            _handDataListener = FindObjectOfType<HandDataListener>();
            _manager = FindObjectOfType<Manager>();
        }

        private void Update()
        {
            Move(_handDataListener.GetHandsAngleDelta());
        }

        void Move(float delta)
        {
            transform.position += _manager.Head.transform.forward * Mathf.Abs(delta) * Time.deltaTime * Speed;
        }
    }
}