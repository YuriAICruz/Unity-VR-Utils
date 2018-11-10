using Graphene.UiGenerics;
using UnityEngine;

namespace Graphene.VRUtils.Presentation
{
    public class RoomInteractionButton : ButtonView
    {
        protected override void OnClick()
        {
            base.OnClick();
            
            Debug.Log("Clicked");
        }
    }
}