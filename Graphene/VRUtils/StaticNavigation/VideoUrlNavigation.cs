using System.Linq;
using UnityEngine;
using UnityEngine.Video;

namespace Graphene.VRUtils.StaticNavigation
{
    [RequireComponent(typeof(VideoPlayer))]
    public class VideoUrlNavigation : BaseNavigation
    {
        public string[] VideoUrls;
        private VideoPlayer _player;

        private void Awake()
        {
            Textures = new Texture[VideoUrls.Length].ToList();

            _player = GetComponent<VideoPlayer>();
        }

        protected override void SetMainTexture()
        {
            if (_player == null)
                _player = GetComponent<VideoPlayer>();
            if (string.IsNullOrEmpty(VideoUrls[_currentTexture]) || _player == null)
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
            if (string.IsNullOrEmpty(VideoUrls[_currentTexture]) || _player == null)
            {
                if (_player != null)
                    _player.Stop();
                
                return;
            }

            _player.Stop();
            _player.url = VideoUrls[_currentTexture];
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