using System;
using Graphene.UiGenerics;
using Graphene.VRUtils.StaticNavigation;
using UnityEngine;
using UnityEngine.UI;

namespace Graphene.VRUtils.Presentation
{
    public class RoomInteractionButton : ButtonView
    {
        public event Action<int> OnClickId;

        public NavigationMap NavigationMap;
        
        public int Id;
        public string Name;

        public void SetName(string name)
        {
            Name = name;

            var tx = transform.GetComponentInChildren<Text>();
            tx.text = Name;
        }

        protected override void OnClick()
        {
            base.OnClick();
            
            NavigationMap.MoveToRoom(Id);
        }
    }
}