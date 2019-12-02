//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Text;
using Object = UnityEngine.Object;

namespace SimpleLUI.API
{
    public abstract class SLUIBuilderObject
    {
        /// <summary>
        ///     Called to resolve type of the object.
        /// </summary>
        public abstract Type ResolveObjectType();

#if UNITY_EDITOR
        public StringBuilder String { get; } = new StringBuilder();
        public bool PrettyPrint { get; set; }
        public string ResourcesPath { get; set; }
        public string ResourcesPathFull { get; set; }

        public abstract void CollectObjectDefinition(Object obj);
        public virtual void CollectObjectDefinitionExtras(Object obj) { }
        public abstract void CollectObjectProperty(Object obj);
        public virtual void CollectExtras(Object obj) { }

        public void Label(string label)
        {
            if (!PrettyPrint)
                return;

            if (!string.IsNullOrEmpty(label))
                String.AppendLine($"-- {label}");
        }

        public void Space(string label = null)
        {
            if (!PrettyPrint)
                return;

            String.AppendLine();
            Label(label);
        }
#endif
    }
}
