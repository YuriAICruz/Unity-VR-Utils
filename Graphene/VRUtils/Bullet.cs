using UnityEngine;

namespace Graphene.VRUtils
{
    public class Bullet : MonoBehaviour
    {
        private Vector3 _dir;
        public float Speed;

        private void Update()
        {
            transform.LookAt(_dir);
            transform.position += _dir * Speed * Time.deltaTime;
        }

        public void SetDir(Vector3 dir)
        {
            _dir = dir;
        }
    }
}