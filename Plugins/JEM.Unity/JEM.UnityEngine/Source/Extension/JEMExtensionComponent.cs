//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JetBrains.Annotations;
using System;
using UnityEngine;

namespace JEM.UnityEngine.Extension
{
    /// <summary>
    ///     Set of utility extensions to Component class.
    /// </summary>
    public static class JEMExtensionComponent
    {
        /// <summary>
        ///     Sets the active state of GameObject of this component.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void SetActive([NotNull] this Component component, bool activeState)
        {
            if (component == null) throw new ArgumentNullException(nameof(component));
            component.gameObject.SetActive(activeState);
        }
    }
}
