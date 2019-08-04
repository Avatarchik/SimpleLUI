//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JetBrains.Annotations;
using System;
using System.Collections.Generic;

namespace SimpleLUI.API.Core
{
    public class SLUIEventItem
    {
        public SLUIObject target;
        public string methodName;
        public List<object> args;

        public SLUIEventItem() { }
        public SLUIEventItem(SLUIObject target, string method)
        {
            this.target = target;
            this.methodName = method;
            args = new List<object>();
        }

        public void Add(int i)
        {
            args.Add(i);
        }

        public void Add(bool b)
        {
            args.Add(b);
        }

        public void Add(float f)
        {
            args.Add(f);
        }

        public void Add(string str)
        {
            args.Add(str);
        }
    }

    public class SLUIUnityEvent
    {
        public List<SLUIEventItem> items { get; } = new List<SLUIEventItem>();

        public SLUIUnityEvent() { }

        public void Add([NotNull] SLUIEventItem eventItem)
        {
            if (eventItem == null) throw new ArgumentNullException(nameof(eventItem));
            items.Add(eventItem);
        }
    }
}
