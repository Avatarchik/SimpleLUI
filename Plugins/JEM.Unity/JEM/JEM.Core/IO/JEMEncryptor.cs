//
// JEM Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

//
// NOTE: This class is Obsolete and may be removed in the future.
//

using System;
using System.IO;
using System.Security.Cryptography;

namespace JEM.Core.IO
{
    /// <summary>
    ///     JEM encryptor class is a pseudo encryption utility.
    /// </summary>
    [Obsolete("This class is Obsolete and may be removed in the future.")]
    public static class JEMEncryptor
    {
        /// <summary>
        ///     A default encryptor password.
        /// </summary>
        public const string DefaultEncryptorPassword = "Ramaja is the king!";

        /// <summary>
        ///     Pseudo encrypts received bytes from object with password and writes them to file at given path.
        /// </summary>
        public static void Write(string fileName, object obj) => Write(DefaultEncryptorPassword, fileName, obj);
        
        /// <summary>
        ///     Pseudo encrypts received bytes from object with password and writes them to file at given path.
        /// </summary>
        public static void Write(string password, string fileName, object obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            Write(password, fileName, obj.SerializeToBytes());
        }

        /// <summary>
        ///     Pseudo encrypts received bytes with password and writes them to file at given path.
        /// </summary>
        public static void Write(string fileName, byte[] bytes) => Write(DefaultEncryptorPassword, fileName, bytes);
        
        /// <summary>
        ///     Pseudo encrypts received bytes with password and writes them to file at given path.
        /// </summary>
        public static void Write(string password, string fileName, byte[] bytes)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));

            var directory = Path.GetDirectoryName(fileName);
            if (Directory.Exists(directory)) Directory.CreateDirectory(directory);

            File.WriteAllBytes(fileName, EncryptByteArray(password, bytes));
        }

        /// <summary>
        ///     Loads file at given path to given type of object.
        /// </summary>
        public static T Load<T>(string password, string fileName) => (T) Load(password, fileName);
        
        /// <summary>
        ///     Loads file at given path to given type of object.
        /// </summary>
        public static object Load(string password, string fileName)
        {
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            if (!File.Exists(fileName))
                throw new FileNotFoundException("Target file not exists", fileName);

            var directory = Path.GetDirectoryName(fileName);
            if (Directory.Exists(directory)) Directory.CreateDirectory(directory);
            var bytes = File.ReadAllBytes(fileName);

            return Load(password, bytes);
        }

        /// <summary>
        ///     Loads file at given path to given type of object.
        ///     Removes pseudo encryption from received array of bytes and converts it to object of serialized type.
        /// </summary>
        public static T Load<T>(string password, byte[] bytes) => (T) Load(password, bytes);

        /// <summary>
        ///     Loads file at given path to given type of object.
        ///     Removes pseudo encryption from received array of bytes and converts it to object of serialized type.
        /// </summary>
        public static object Load(string password, byte[] bytes)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));
            return DecryptByteArray(password, bytes).DeserializeFromBytes();
        }

        // yes, those methods are in fact from stackoverflow :)

        /// <summary>
        ///     Encrypts given byte array.
        /// </summary>
        public static byte[] EncryptByteArray(byte[] bytes) => EncryptByteArray(DefaultEncryptorPassword, bytes);
        
        /// <summary>
        ///     Encrypts given byte array.
        /// </summary>
        public static byte[] EncryptByteArray(string password, byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));
            var a = CreateOperator(password);
            var e = a.CreateEncryptor();
            using (var ms = new MemoryStream())
            using (var cs = new CryptoStream(ms, e, CryptoStreamMode.Write))
            {
                cs.Write(bytes, 0, bytes.Length);
                cs.Close();
                return ms.ToArray();
            }
        }

        /// <summary>
        ///     Decrypts given byte array.
        /// </summary>
        public static byte[] DecryptByteArray(byte[] bytes) => DecryptByteArray(DefaultEncryptorPassword, bytes);
        
        /// <summary>
        ///     Decrypts given byte array.
        /// </summary>
        public static byte[] DecryptByteArray(string password, byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));
            var a = CreateOperator(password);
            var d = a.CreateDecryptor();
            using (var ms = new MemoryStream())
            using (var cs = new CryptoStream(ms, d, CryptoStreamMode.Write))
            {
                cs.Write(bytes, 0, bytes.Length);
                cs.Close();
                return ms.ToArray();
            }
        }

        private static SymmetricAlgorithm CreateOperator(string password)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            var a = Rijndael.Create();
            var rdb = new Rfc2898DeriveBytes(password, new byte[]
            {
                0x53, 0x6f, 0x64, 0x69, 0x75, 0x6d, 0x20, // salty goodness
                0x43, 0x68, 0x6c, 0x6f, 0x72, 0x69, 0x64, 0x65
            });
            a.Padding = PaddingMode.ISO10126;
            a.Key = rdb.GetBytes(32);
            a.IV = rdb.GetBytes(16);
            return a;
        }

    }
}