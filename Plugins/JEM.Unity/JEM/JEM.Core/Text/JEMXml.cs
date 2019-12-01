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
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace JEM.Core.Text
{
    /// <summary>
    ///     Xml string de/serialization class.
    /// </summary>
    public static class JEMXml
    {
        private static string Utf8ByteArrayToString(byte[] characters)
        {
            var encoding = new UTF8Encoding();
            var constructedString = encoding.GetString(characters);
            return constructedString;
        }

        private static byte[] StringToUtf8ByteArray(string pXmlString)
        {
            var encoding = new UTF8Encoding();
            var byteArray = encoding.GetBytes(pXmlString);
            return byteArray;
        }

        /// <summary>
        ///     Parse object to xml string.
        /// </summary>
        /// <param name="targetObject">Object to serialize.</param>
        /// <returns>Xml string of object.</returns>
        public static string ObjectToString(object targetObject)
        {
            if (targetObject == null)
                throw new ArgumentNullException(nameof(targetObject));

            var memoryStream = new MemoryStream();
            var xs = new XmlSerializer(targetObject.GetType());
            var xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
            xs.Serialize(xmlTextWriter, targetObject);
            memoryStream = (MemoryStream) xmlTextWriter.BaseStream;
            return Utf8ByteArrayToString(memoryStream.ToArray());
        }

        /// <summary>
        ///     Parse xml string to object.
        /// </summary>
        /// <typeparam name="T">Type of object to create from xml string.</typeparam>
        /// <param name="xmlString">Xml string of target.</param>
        /// <returns>Created object from xml string.</returns>
        public static T StringToObject<T>(string xmlString)
        {
            if (string.IsNullOrEmpty(xmlString))
                throw new ArgumentNullException(nameof(xmlString), $"{nameof(xmlString)} can be null or empty!");

            return (T) StringToObject(typeof(T), xmlString);
        }

        /// <summary>
        ///     Parse xml string to object.
        /// </summary>
        /// <param name="objectType">Type of object to create from xml string.</param>
        /// <param name="xmlString">Target xml string.</param>
        /// <returns>Created object from xml string.</returns>
        public static object StringToObject(Type objectType, string xmlString)
        {
            if (objectType == null)
                throw new ArgumentNullException(nameof(objectType));
            if (string.IsNullOrEmpty(xmlString))
                throw new ArgumentNullException(nameof(xmlString), $"{nameof(xmlString)} can be null or empty!");

            var xs = new XmlSerializer(objectType);
            var memoryStream = new MemoryStream(StringToUtf8ByteArray(xmlString));
            return xs.Deserialize(memoryStream);
        }
    }
}