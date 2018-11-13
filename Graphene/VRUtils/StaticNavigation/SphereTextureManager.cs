using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graphene.VRUtils.StaticNavigation
{
    [RequireComponent(typeof(Renderer))]
    public class SphereTextureManager : BaseNavigation
    {
        // Uses Blend2Textures Shader

        private Renderer _renderer;


        void Awake()
        {
            GetMaterial();
            _currentTexture = 0;
        }

        protected override void GetMaterial()
        {
            base.GetMaterial();
            _renderer = GetComponent<Renderer>();
            _material = _renderer.material;
        }
    }
}