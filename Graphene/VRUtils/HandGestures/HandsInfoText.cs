using Graphene.UiGenerics;

namespace Graphene.VRUtils
{
    public class HandsInfoText : TextView
    {
        private HandDataListener _handDataListener;

        private void Setup()
        {
            _handDataListener = FindObjectOfType<HandDataListener>();
        }

        void Update()
        {
            if(_handDataListener == null) return;
            
            Text.text = $"dist: {_handDataListener.GetHandDistance()}\n" +
                        $"angle: {_handDataListener.GetHandsAngle()}\n" +
                        $"angle waist: {_handDataListener.GetHandsAngleFromWaist()}\n" +
                        $"delta: {_handDataListener.GetHandsAngleDelta()}";
        }
    }
}