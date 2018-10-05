using UnityEngine;

namespace Graphene.VRUtils
{
    public class HandGun : HandInteractible
    {
        public int MaxBullets;
        private int _bullets;

        private Bullet _particle;

        public Transform Tip;

        private void Awake()
        {
            _bullets = MaxBullets;
            _particle = Resources.Load<Bullet>("Particles/Bullet");
        }

        public override void Trigger()
        {
            Shoot();
        }

        private void Shoot()
        {
            if (_bullets <= 0) return;
            
            _bullets--;

            var bl = Instantiate(_particle);

            bl.transform.position = Tip.position;
            bl.SetDir(transform.forward);
        }
    }
}