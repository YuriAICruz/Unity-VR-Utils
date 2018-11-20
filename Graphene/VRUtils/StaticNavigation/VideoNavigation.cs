using System.Linq;
using UnityEngine;
using UnityEngine.Video;

namespace Graphene.VRUtils.StaticNavigation
{
    [RequireComponent(typeof(VideoPlayer))]
    public class VideoNavigation : BaseNavigation
    {
        public VideoClip[] Videos;
        private VideoPlayer _player;

        private void Awake()
        {
            Textures = new Texture[Videos.Length].ToList();

            _player = GetComponent<VideoPlayer>();
        }

        protected override void SetMainTexture()
        {
            if (_player == null)
                _player = GetComponent<VideoPlayer>();
            if (Videos[_currentTexture] == null || _player == null)
            {
                if (_player != null)
                    _player.Stop();
                
                return;
            }

            _player.Play();
        }

        protected override void SetSecodaryTexture()
        {
            if (_player == null)
                _player = GetComponent<VideoPlayer>();
            if (Videos[_currentTexture] == null || _player == null)
            {
                if (_player != null)
                    _player.Stop();
                
                return;
            }

            _player.Stop();
            _player.clip = Videos[_currentTexture];
        }

        protected override void SetUpdateBlend(float t)
        {
        }

        protected override void GetMaterial()
        {
            base.GetMaterial();
        }
    }
}