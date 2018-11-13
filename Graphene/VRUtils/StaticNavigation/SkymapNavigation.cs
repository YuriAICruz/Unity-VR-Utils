using System.Collections.Generic;
using UnityEngine;

namespace Graphene.VRUtils.StaticNavigation
{
    public class SkymapNavigation : BaseNavigation
    {
        void Awake()
        {
            GetMaterial();
        }

        protected override void GetMaterial()
        {
            base.GetMaterial();
            _material = RenderSettings.skybox;
        }
    }
}