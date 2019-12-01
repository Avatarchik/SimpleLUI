//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;

namespace JEM.UnityEngine.Audio.Interface
{
    /// <inheritdoc />
    /// <summary>
    ///     Base class of every audio controller used by user interface controls.
    /// </summary>
    public abstract class JEMAudioForControl : MonoBehaviour
    {
        /// <summary>
        ///     Reference to the <see cref="JEMAudioInterfaceDatabase.Instance"/>.
        /// </summary>
        public JEMAudioInterfaceDatabase Database => JEMAudioInterfaceDatabase.Instance;
    }
}
