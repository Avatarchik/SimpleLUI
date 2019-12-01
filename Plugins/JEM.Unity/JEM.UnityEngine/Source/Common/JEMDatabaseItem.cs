//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.UnityEngine.Attribute;
using UnityEngine;

namespace JEM.UnityEngine.Common
{
    /// <inheritdoc />
    /// <summary>
    ///     A base class of simple 'Database' system.
    /// </summary>
    public abstract class JEMDatabaseItem : ScriptableObject
    {
        /// <summary>
        ///     Identity of this Database item.
        /// </summary>
        [JEMReadonly]
        public ushort Identity;
    }
}
