using System.Linq;
using UnityEngine;
using UnityEngine.Video;

namespace Graphene.VRUtils.StaticNavigation
{
    [RequireComponent(typeof(VideoPlayer))]
    public class VideoUrlNavigation : BaseNavigation
    {
        public string[] ResPaths = new string[]
        {
            "4k/",
            "FHD/",
            "HD/",
            "SD/"
        };

        private int _selectedResolution;
        public string BaseUrl;
        public string[] VideoUrls;
        private VideoPlayer _player;

        private void Awake()
        {
            Textures = new Texture[VideoUrls.Length].ToList();

            _player = GetComponent<VideoPlayer>();

            if (Screen.currentResolution.width > 1920)
            {
                _selectedResolution = 0;
            }
            else if (Screen.currentResolution.width <= 1920)
            {
                _selectedResolution = 1;
            }
            else if (Screen.currentResolution.width <= 1080)
            {
                _selectedResolution = 2;
            }
            else
            {
                _selectedResolution = 3;
            }
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
            _player.url = BaseUrl + ResPaths[_selectedResolution] + VideoUrls[_currentTexture];
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