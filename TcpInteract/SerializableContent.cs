using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace TcpInteract
{
    /// <summary>
    /// Represents content that can be serialized and deserialized.
    /// </summary>
    [Serializable]
    public abstract class SerializableContent<T> : ISerializable where T : SerializableContent<T>
    {
        /// <summary>
        /// Serialize object using <see cref="BinaryFormatter"/>.
        /// </summary>
        public byte[] Serialize()
        {
            using (var stream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(stream, this);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Deserialize data using <see cref="BinaryFormatter"/>.
        /// </summary>
        public static T Deserialize(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            {
                return (T)(new BinaryFormatter().Deserialize(stream));
            }
        }
    }

    /// <summary>
    /// Implements functionality to serialize an instance.
    /// </summary>
    public interface ISerializable
    {
        /// <summary>
        /// Implements an instance serializer.
        /// </summary>
        byte[] Serialize();
    }
}
