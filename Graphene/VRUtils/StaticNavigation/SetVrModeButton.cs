using Graphene.UiGenerics;
using UnityEngine;

namespace Graphene.VRUtils.StaticNavigation
{
    public class SetVrModeButton : ButtonView
    {
        private SetVideoMode _cam;

        public bool VR;

        void Setup()
        {
            _cam = Camera.main.GetComponent<SetVideoMode>();
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