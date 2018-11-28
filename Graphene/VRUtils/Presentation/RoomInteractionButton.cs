using System;
using Graphene.UiGenerics;
using Graphene.VRUtils.StaticNavigation;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Graphene.VRUtils.Presentation
{
    public class RoomInteractionButton : ButtonView
    {
        public event Action<int> OnClickId;

        public NavigationMap NavigationMap;

        public bool IsPopupVideo;
        public VideoClip Clip;
        public string ClipName;

        public int Id;
        public string Name;
        private VideoWindow _videoWindow;
        private VideoPlayerView _player;
        private bool _videoShown;

        private bool _fromURL = VideoPlayerView.fromURL;

        private void Setup()
        {
            if (IsPopupVideo)
            {
                if (_videoWindow == null)
                    _videoWindow = transform.GetComponentInChildren<VideoWindow>();
                if (_player == null)
                    _player = FindObjectOfType<VideoPlayerView>();
                _player.OnStop += Reset;
                _player.OnEnd += Reset;

                _videoWindow.Hide();
            }
            
            Reset();

            SetName(Name);
        }

        private void OnEnable()
        {
            Reset();
        }

        private void OnDisable()
        {
        }

        private void Reset()
        {
            if (IsPopupVideo)
            {
                if (_videoWindow == null)
                    _videoWindow = transform.GetComponentInChildren<VideoWindow>();
                if (_player == null)
                    _player = FindObjectOfType<VideoPlayerView>();

                _videoWindow.Hide();
                
                _videoShown = false;
            }
        }

        public void SetName(string name)
        {
            Name = name;

            var tx = transform.GetComponentInChildren<Text>();
            tx.text = Name;
        }

        protected override void OnClick()
        {
            base.OnClick();

            if (IsPopupVideo && !_videoShown)
            {
                if (_fromURL)
                {
                    _player.Play(ClipName);
                }
                else
                {
                    _player.Play(Clip);
                }
                _videoWindow.Show();
                _videoShown = true;
            }
            else
            {
                NavigationMap.MoveToRoom(Id);
            }
        }
    }
}