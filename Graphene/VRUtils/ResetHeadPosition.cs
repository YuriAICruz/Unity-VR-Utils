using UnityEngine;

namespace Graphene.VRUtils
{
    public class ResetHeadPosition : MonoBehaviour
    {
        public Transform Target;

        private void Start()
        {
            ResetPosition();
        }

        void Update()
        {
//            if(Input.GetKeyDown(KeyCode.Space))
//            {
//               ResetPosition(); 
//            }
        }
        
        public void ResetPosition()
        {
            transform.position = Target.position - transform.GetComponentInChildren<Camera>().transform.localPosition;
        }
    }
}