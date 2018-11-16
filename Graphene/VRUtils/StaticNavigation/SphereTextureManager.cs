using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
            #if UNITY_EDITOR
            if (EditorApplication.isPlaying)
            {
                _material = _renderer.material;
            }else{
                _material = _renderer.sharedMaterial;}
            #else
                _material = _renderer.material;
            #endif
        }
    }
}