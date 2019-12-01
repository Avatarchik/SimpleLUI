//
// JEM Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace JEM.Core.IO
{
    /// <summary>
    ///     MD5 hash calculation class.
    /// </summary>
    public static class JEMMD5
    {
        /// <summary>
        ///     Mode of md5 hash calculation.
        /// </summary>
        public enum HashMode
        {
            /// <summary>
            ///     MD5 hash will be calculated per character.
            /// </summary>
            PerChar,

            /// <summary>
            ///     MD5 hash will by calculated ny whole byte data.
            /// </summary>
            FromByte
        }

        /// <summary>
        ///     Calculates MD5 hash.
        /// </summary>
        /// <param name="stringToHash">String to hash.</param>
        /// <param name="mode">Mode of hash.</param>
        /// <param name="lowCase"></param>
        /// <returns>Generate string in low case?</returns>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public static string Hash(string stringToHash, HashMode mode = HashMode.PerChar, bool lowCase = true)
        {
            var format = lowCase ? "x2" : "X2";

            switch (mode)
            {
                case HashMode.PerChar:
                {
                    var md5 = MD5.Create();
                    var inputBytes = Encoding.ASCII.GetBytes(stringToHash);
                    var hash = md5.ComputeHash(inputBytes);
                    var sb = new StringBuilder();
                    foreach (var t in hash) sb.Append(t.ToString(format));

                    return sb.ToString();
                }
                case HashMode.FromByte:
                {
                    var bytes = Encoding.ASCII.GetBytes(stringToHash);
                    string hash;

                    using (var md5 = MD5.Create())
                    {
                        hash = string.Concat(md5.ComputeHash(bytes).Select(x => x.ToString(format)));
                    }

                    return hash;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        /// <summary>
        ///     Calculates MD5 hash.
        /// </summary>
        /// <param name="stream">File stream to hash.</param>
        /// <param name="mode">Mode of hash.</param>
        /// <param name="lowCase"></param>
        /// <returns>Generate string in low case?</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public static string Hash(FileStream stream, HashMode mode = HashMode.PerChar, bool lowCase = true)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            var format = lowCase ? "x2" : "X2";

            switch (mode)
            {
                case HashMode.PerChar:
                {
                    var md5 = MD5.Create();
                    var hash = md5.ComputeHash(stream);
                    var sb = new StringBuilder();
                    foreach (var t in hash) sb.Append(t.ToString(format));

                    return sb.ToString();
                }
                case HashMode.FromByte:
                {
                    string hash;

                    using (var md5 = MD5.Create())
                    {
                        hash = string.Concat(md5.ComputeHash(stream).Select(x => x.ToString(format)));
                    }

                    return hash;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }
    }
}