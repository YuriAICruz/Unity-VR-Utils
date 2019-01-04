using Graphene.UiGenerics;
using UnityEngine;
using UnityEngine.UI;

namespace Graphene.VRUtils.StaticNavigation
{
    public class SetVrModeButton : ButtonView
    {
        private SetVideoMode _cam;

        public Text title;
        public Text subtitle;

        public bool VR;

        void Setup()
        {
            _cam = Camera.main.GetComponent<SetVideoMode>();

            bool isPT = PlayerPrefs.GetString("language", "PT") == "PT";

            if (VR)
            {
                    title.text = "360° VR";
                    subtitle.text = "daydream\ncardboard\n...";
            }
            else
            {
                if (isPT)
                {
                    title.text = "Tela Normal";
                    subtitle.text = "sem óculos\nde VR";
                }
                else
                {
                    title.text = "Pantalla Normal";
                    subtitle.text = "sin gafas\nde VR";
                }
            }
        }

        protected override void OnClick()
        {
            base.OnClick();

            if (VR)
                _cam.SetStereoscopic();
            else
                _cam.SetMonoscopic();
        }
    }
}