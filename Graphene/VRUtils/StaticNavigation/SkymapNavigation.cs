using System.Collections.Generic;
using UnityEngine;

namespace Graphene.VRUtils.StaticNavigation
{
    public class SkymapNavigation : BaseNavigation
    {
        public List<Texture2D> Textures;
        private int _actualTexture;
        private Renderer _renderer;
        private Material _material;

        public float TransitionTime = 0.4f;
        
        void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _material = RenderSettings.skybox;

            _actualTexture = 0;
        }

        protected override void GetMaterial()
        {
            base.GetMaterial();
            _renderer = GetComponent<Renderer>();
            _material = _renderer.material;
        }
    }
}