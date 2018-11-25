using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
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

        private Text _infoText;

        private void Awake()
        {
            Textures = new Texture[VideoUrls.Length].ToList();

            _player = GetComponent<VideoPlayer>();
            _infoText = GameObject.Find("VideoLoadingText").GetComponent<Text>();

            if (Screen.currentResolution.width > 1920)
            {
                _selectedResolution = 0;
            }
            else if (Screen.currentResolution.width > 1280)
            {
                _selectedResolution = 2; // was 1, reduced because videos were not being played
            }
            else if (Screen.currentResolution.width > 1080)
            {
                _selectedResolution = 3; // was 2
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
            _player.errorReceived += ErrorReceived;
            StartCoroutine(PrepareVideo());
        }

        protected IEnumerator PrepareVideo()
        {
            _player.Prepare();
            _infoText.text = "Baixando o vídeo...";

            while (!_player.isPrepared)
            {
                yield return null;
            }

            _infoText.text = "";
        }

        protected void ErrorReceived (VideoPlayer source, string msg)
        {
            _infoText.text = "Não foi possível exibir o vídeo";
            _player.errorReceived -= ErrorReceived;
            StartCoroutine(CleanInfoText());
        }

        protected IEnumerator CleanInfoText()
        {
            yield return new WaitForSeconds(3);
            _infoText.text = "";
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