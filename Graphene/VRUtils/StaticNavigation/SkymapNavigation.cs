using System.Collections.Generic;
using UnityEngine;

namespace Graphene.VRUtils.StaticNavigation
{
    public class SkymapNavigation : BaseNavigation
    {
        private Orbiter _orbiter;

        void Awake()
        {
            GetMaterial();

            if (Textures[_currentTexture] == null) return;
            
            _orbiter = FindObjectOfType<Orbiter>();
        }

        protected override void OnFullBlack()
        {
            if (_orbiter == null)
                _orbiter = FindObjectOfType<Orbiter>();
         
            _material.SetFloat("_Rotation", _orbiter.GetYRotation() % 360);

            if (Textures[_currentTexture] == null) return;
            _material.SetTexture("_Tex", Textures[_currentTexture]);
        }

        protected override void GetMaterial()
        {
            base.GetMaterial();
            _material = RenderSettings.skybox;
        }

        protected override void SetMainTexture()
        {
            _material.SetFloat("_Blend", 0);
            if (Textures[_currentTexture] == null) return;
            _material.SetTexture("_Tex", Textures[_currentTexture]);
        }

        protected override void SetSecodaryTexture()
        {
            _material.SetFloat("_Blend", 0);
        }
    }
}