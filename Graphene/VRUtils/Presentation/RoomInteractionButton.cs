using System;
using Graphene.UiGenerics;
using Graphene.VRUtils.StaticNavigation;
using UnityEngine;

namespace Graphene.VRUtils.Presentation
{
    public class RoomInteractionButton : ButtonView
    {
        public event Action<int> OnClickId;

        public NavigationMap NavigationMap;
        
        public int Id;
        
        protected override void OnClick()
        {
            base.OnClick();
            
            NavigationMap.MoveToRoom(Id);
        }
    }
}