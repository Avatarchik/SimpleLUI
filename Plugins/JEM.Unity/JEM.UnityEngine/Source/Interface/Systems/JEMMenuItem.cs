//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JetBrains.Annotations;
using System;

namespace JEM.UnityEngine.Interface.Systems
{
    /// <summary>
    ///     A JEM MenuItem system for unity UI.
    ///     This static class just have references to <see cref="JEMMenuItemManager"/> class for shorter syntax.
    ///     For more properties or methods that are not implemented here, see <see cref="JEMMenuItemManager"/> or <see cref="JEMMenuItemElement"/>.
    /// </summary>
    public static class JEMMenuItem
    {
        /// <summary>
        ///     Register new item in to the menu.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void RegisterItem([NotNull] string path, [NotNull] Action onClick) => JEMMenuItemManager.RegisterItem(path, onClick);
    }
}
