//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.UnityEngine.Interface;
using SimpleLUI.Editor.API;
using Object = UnityEngine.Object;

namespace SimpleLUI.Editor.JEM
{
    [SLUIBuilderObject]
    public class SLUIFadeElement : SLUIBuilderObject
    {
        public SLUIFadeElement() : base(typeof(InterfaceFadeElement)) { }

        public override void CollectObjectDefinition(Object obj)
        {
            // String.AppendLine("some stuff from fade element!");
        }

        public override void CollectObjectProperty(Object obj)
        {
            
        }
    }
}
