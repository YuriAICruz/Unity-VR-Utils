using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graphene.VRUtils.StaticNavigation
{
    [RequireComponent(typeof(Renderer))]
    public class SphereTextureManager : MonoBehaviour
    {
        // Uses Blend2Textures Shader

        public List<Texture2D> Textures;
        private int _actualTexture;
        private Renderer _renderer;
        private Material _material;

        public float TransitionTime = 0.4f;

        void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _material = _renderer.material;

            _actualTexture = 0;
        }

        void NextTexture()
        {
            _actualTexture++;
            _actualTexture = _actualTexture % Textures.Count;
        }

        public void ChangeTexture()
        {
            StartCoroutine(Transit());
        }

        IEnumerator Transit(int index = -1)
        {
#if UNITY_EDITOR
            if (_renderer == null)
                _renderer = GetComponent<Renderer>();
            if (_material == null)
                _material = _renderer.sharedMaterial;
#endif

            if (index < 0)
            {
                NextTexture();
            }
            else
            {
                _actualTexture = index;
            }

            _material.SetTexture("_Texture2", Textures[_actualTexture]);

            var t = 0f;
            while (t < TransitionTime)
            {
                _material.SetFloat("_Blend", t / TransitionTime);

                yield return null;
                t += Time.deltaTime;
            }

            _material.SetFloat("_Blend", 0);
            _material.mainTexture = Textures[_actualTexture];
        }

        public void ChangeTexture(int index)
        {
            if (index >= Textures.Count) return;

            StartCoroutine(Transit(index));
        }
    }
}