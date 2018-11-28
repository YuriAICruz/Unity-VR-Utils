using System.Linq;
using System.Collections;
using Graphene.UiGenerics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Graphene.VRUtils.StaticNavigation
{
    [RequireComponent(typeof(VideoPlayer))]
    public class VideoUrlNavigation : BaseNavigation
    {
        public string BaseUrl;
        public string[] VideoUrls;
        
        public VideoUrlSourceManage _player;

        private bool _fromURL = VideoPlayerView.fromURL;

        private VideoLoadingText _infoText;

        private void Awake()
        {
            Textures = new Texture[VideoUrls.Length].ToList();

            _player.Setup(GetComponent<VideoPlayer>());

            _player.errorReceived += OnError;
            _infoText = FindObjectOfType<VideoLoadingText>();
        }

        private void OnError(VideoPlayer arg1, string arg2)
        {
            StartCoroutine(Transit(_currentTexture));
        }

        protected override void SetMainTexture()
        {
        }

        protected override void OnFullBlack()
        {
            if (_player == null)
            {
                _player = new VideoUrlSourceManage();
                _player.Setup(GetComponent<VideoPlayer>());
            }
            
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
            {
                _player = new VideoUrlSourceManage();
                _player.Setup(GetComponent<VideoPlayer>());
            }
            
            if (string.IsNullOrEmpty(VideoUrls[_currentTexture]) || _player == null)
            {
                if (_player != null)
                    _player.Stop();

                return;
            }

            _player.Stop();
            _player.SetUrl(BaseUrl + _player.GetResUrlPath(), VideoUrls[_currentTexture]);
            _player.errorReceived += ErrorReceived;
            _holdFade = true;
            StartCoroutine(PrepareVideo());
        }

        protected IEnumerator PrepareVideo()
        {
            _player.Prepare();

            if (_fromURL)
            {
                _infoText.SetText("Baixando o vídeo...");
            }

            while (!_player.IsPrepared())
            {
                yield return null;
            }

            _infoText.SetText("");
            
            yield return null;
            
            _holdFade = false;
        }

        protected void ErrorReceived (VideoPlayer source, string msg)
        {
            _infoText.SetText("Não foi possível exibir o vídeo");
            _player.errorReceived -= ErrorReceived;
            StartCoroutine(CleanInfoText());
        }

        protected IEnumerator CleanInfoText()
        {
            yield return new WaitForSeconds(3);
            _infoText.SetText("");
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