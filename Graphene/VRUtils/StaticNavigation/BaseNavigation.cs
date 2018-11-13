using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graphene.VRUtils.StaticNavigation
{
    public abstract class BaseNavigation : MonoBehaviour, INavigator
    {
        public List<Texture> Textures;
        protected Material _material;
        protected int _currentTexture;

        public float TransitionTime = 0.4f;

        protected void NextTexture()
        {
            _currentTexture++;
            _currentTexture = _currentTexture % Textures.Count;
        }

        public void ChangeTexture()
        {
            StartCoroutine(Transit());
        }

        IEnumerator Transit(int index = -1)
        {
#if UNITY_EDITOR
            GetMaterial();
#endif

            if (index < 0)
            {
                NextTexture();
            }
            else
            {
                _currentTexture = index;
            }

            SetSecodaryTexture();

            var t = 0f;
            while (t < TransitionTime)
            {
                SetUpdateBlend(t / TransitionTime);

                yield return null;
                t += Time.deltaTime;
            }

            SetMainTexture();
        }

        protected virtual void SetSecodaryTexture()
        {
            _material.SetTexture("_Texture2", Textures[_currentTexture]);
        }

        protected virtual void SetUpdateBlend(float t)
        {
            _material.SetFloat("_Blend", t);
        }

        protected virtual void SetMainTexture()
        {
            _material.SetFloat("_Blend", 0);
            _material.mainTexture = Textures[_currentTexture];
        }

        protected virtual void GetMaterial()
        {
        }

        public void ChangeTexture(int index)
        {
            if (index >= Textures.Count) return;

            StartCoroutine(Transit(index));
        }
    }
}