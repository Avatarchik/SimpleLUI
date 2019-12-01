//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

namespace JEM.UnityEngine.Extension
{
    /// <summary>
    ///     Rest of weird extension i don't know where to put lol.
    /// </summary>
    public static class JEMExtensionMath
    {
        /// <summary>
        ///     Converts <see cref="float"/> in to <see cref="sbyte"/>.
        /// </summary>
        public static sbyte ToSByte(this float f)
        {
            if (f > float.Epsilon)
                return 1;
            if (f < -float.Epsilon)
                return -1;
            return 0;
        }
    }
}
