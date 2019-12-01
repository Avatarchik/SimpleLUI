//
// JEM Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;

namespace JEM.Core.Common
{
    /// <inheritdoc />
    /// <summary>
    ///     Interface that defines a <see cref="IJEMPoolManager" /> item.
    /// </summary>
    public interface IJEMPoolItem : IDisposable
    {
        /// <summary>
        ///     Defines if this item is pooled or not.
        /// </summary>
        bool IsPooled { get; set; }

        /// <summary>
        ///     Called when this item has been pooled by manager.
        /// </summary>
        void OnPooled();

        /// <summary>
        ///     Called when this item has been resolved by the active <see cref="IJEMPoolManager"/>.
        /// </summary>
        void OnResolved(object[] args);

        /// <summary>
        ///     Returns false if this item is not valid and should by deleted by pooling manager.
        /// </summary>
        bool IsValid();
    }
}
