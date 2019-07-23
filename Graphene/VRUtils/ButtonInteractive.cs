using System.Collections;
using Graphene.Shader.Scripts;
using UnityEngine;

namespace Graphene.VRUtils
{
    [RequireComponent(typeof(AudioSource))]
    public class ButtonInteractive : HandInteractible
    {
        public float ClickDuration = 0.2f;
        public float Height = 0.008f;
        private Coroutine _clickAnimation;

        private AudioSource _audioSource;
        public AudioClip Click;
        
        protected TransitionOutlineMaterialManager _outline;

        private bool _interactible;

        protected override void Awake()
        {
            base.Awake();

            _outline = GetComponent<TransitionOutlineMaterialManager>();
            if (!_outline)
            {
                _outline = transform.GetComponentInChildren<TransitionOutlineMaterialManager>();
            }
            
            _audioSource = GetComponent<AudioSource>();
            
            _audioSource.loop = false;
            _audioSource.playOnAwake = false;
            _audioSource.clip = Click;

            ChangeInteractible(true);
        }

        protected virtual void ChangeInteractible(bool value)
        {
            _interactible = value;
        }

        public override bool OnGrab(Transform parent)
        {
            return false;
        }

        public override bool Release()
        {
            return true;
        }

        public override void OnCollisionEnter()
        {
            base.OnCollisionEnter();
            
            OnClick();    
        }

        public override void OnCollisionExit()
        {
            base.OnCollisionExit();
        }

        protected virtual void OnClick()
        {
            if(!_interactible) return;
            
            if(_clickAnimation!=null)
                StopCoroutine(_clickAnimation);
            
            _audioSource.Play();
            
            ChangeInteractible(false);
            _clickAnimation = StartCoroutine(AnimateClick());
            
            
            AnalyticsSaver.SaveData("on_interact", gameObject.name, "on_click");
            
            _outline.HideOutline();
        }

        IEnumerator AnimateClick()
        {
            var t = 0f;

            var i = transform.position;

            var d = _initialPosition - Vector3.up*Height;

            while (t < ClickDuration)
            {
                transform.position = Vector3.Lerp(i, d, t/ClickDuration);

                t += Time.deltaTime;
                
                yield return null;
            }
            
            t = 0f;
            while (t < ClickDuration)
            {
                transform.position = Vector3.Lerp(d, _initialPosition, t/ClickDuration);

                t += Time.deltaTime;
                
                yield return null;
            }
            
            ChangeInteractible(true);
        }

    }
}