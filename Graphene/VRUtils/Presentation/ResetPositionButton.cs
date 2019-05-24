using Graphene.UiGenerics;

namespace Graphene.VRUtils.Presentation
{
    public class ResetPositionButton : ButtonView
    {
        private ResetHeadPosition _rh;

        private void Setup()
        {
            _rh = FindObjectOfType<ResetHeadPosition>();
        }

        protected override void OnClick()
        {
            base.OnClick();

            _rh.ResetPosition();
        }
    }
}