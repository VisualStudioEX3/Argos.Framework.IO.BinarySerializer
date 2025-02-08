using Argos.Framework.IO.BinarySerializers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace Argos.Framework.IO
{
    /// <summary>
    /// Utility class to serialize objects to raw binary format.
    /// </summary>
    /// <remarks>
    /// This serializer converts to primitive type values all public non static serializable 
    /// members (fields and properties) from a serializable object, without storing any 
    /// metadata refered to the serialized types or data model structure, only serializes the 
    /// field values sequentially in his binary format.
    /// <para/>
    /// The serializer supports any of the following types for the object members:
    /// <list type="bullet">
    /// <item>Primitive types: <see cref="bool"/>, <see cref="byte"/>, <see cref="sbyte"/>, 
    /// <see cref="char"/>, <see cref="short"/>, <see cref="ushort"/>, <see cref="int"/>, 
    /// <see cref="uint"/>, <see cref="long"/>, <see cref="ulong"/>, <see cref="float"/> and 
    /// <see cref="double"/>.</item>
    /// <item>Non primitive types: <see cref="decimal"/>, <see cref="string"/>, 
    /// <see cref="DateTime"/>, <see cref="TimeSpan"/> and <see cref="Guid"/>.</item>
    /// <item>Typed enumerations: <see cref="byte"/>, <see cref="sbyte"/>, 
    /// <see cref="short"/>, <see cref="ushort"/>, <see cref="int"/>, 
    /// <see cref="uint"/>, <see cref="long"/> and <see cref="ulong"/>.</item>
    /// <item>Arrays of one dimension based on the types from this list, including arrays.</item>
    /// <item><see cref="List{T}"/> and <see cref="IEnumerable"/> types based on 
    /// <see cref="IList"/> or <see cref="IDictionary"/>.</item>
    /// <item>Serializable objects: classes marked with <see cref="SerializableAttribute"/> or
    /// <see cref="DataContractAttribute"/> attributes.</item>
    /// </list>
    /// The <see cref="IEnumerable"/> types only will be serialized if the generic type key or value 
    /// is one of the supported types described above.
    /// <para/>
    /// You can use the <see cref="NonSerializedAttribute"/> and 
    /// <see cref="BinaryIgnoreAttribute"/> to select which members must be ignored
    /// for serialization.
    /// <para/>
    /// The <see cref="BinarySerializer"/> also supports <see cref="DataContractAttribute"/> and 
    /// <see cref="DataMemberAttribute"/> attributes to define which classes and members, private or 
    /// public, are serializables. The attribute values (Name, Namespace, Order, IsRequired, etc...)
    /// are ignored. <see cref="NonSerializedAttribute"/> and <see cref="BinaryIgnoreAttribute"/>
    /// attributes are ignored when the <see cref="DataContractAttribute"/> attribute is present. The
    /// <see cref="DataContractAttribute"/> behavior is applied only for the class where the attribute
    /// is present. The types of the class members doesn't inherit this behavior when are serialized or
    /// deserialized.
    /// <para/>
    /// The <see cref="BinarySerializer"/> serializes all members sorted by name to ensure always 
    /// the same serialziation and deserialization member order. The 
    /// <see cref="DataMemberAttribute.Order"/> attribute value doesn't affect this behavior. 
    /// <para/>
    /// All serializable members are required for serialization and deserialization. The 
    /// <see cref="DataMemberAttribute.IsRequired"/> attribute value doesn't affect this behavior.
    /// </remarks>
    public static class BinarySerializer
    {
        #region Methods & Functions
        /// <summary>
        /// Serializes an object to binary format.
        /// </summary>
        /// <param name="obj">Object to serialize.</param>
        /// <param name="settings">Optional settings for serialization.</param>
        /// <returns>Returns a <see cref="byte"/> array with all serializable data from 
        /// the source object.</returns>
        /// <exception cref="BinarySerializerException">Serializer exception that encapsulates the 
        /// source exception.</exception>
        public static byte[] Serialize(object obj, BinarySerializerSettings? settings = null)
        {
            try
            {
                if (obj is null)
                    throw new ArgumentNullException(nameof(obj));

                using (var buffer = new MemoryStream())
                {
                    using (var writer = new BinaryWriter(buffer, GetTextEncodingFromSettings(settings)))
                        if (!BinaryDataSerializer.instance.Serialize(obj, writer))
                            throw new ArgumentException(
                                message: $"The object of type \"{obj.GetType().Name}\" " +
                                    $"is not serializable.", 
                                paramName: nameof(obj));

                    return buffer.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw new BinarySerializerException(
                    message: "An error occurred during binary serialization.",
                    sourceException: ex,
                    serializableType: obj?.GetType(),
                    objToSerialize: obj);
            }
        }

        /// <summary>
        /// Deserializes a <see cref="byte"/> array to a typed object.
        /// </summary>
        /// <typeparam name="T">Type of the object to deserialize.</typeparam>
        /// <param name="data">Data to deserialize.</param>
        /// <param name="settings">Optional settings for serialization.</param>
        /// <returns>Returns an instance of the generic type defined by T with all data deserialized 
        /// from the source <see cref="byte"/> array.</returns>
        /// <exception cref="BinarySerializerException">Serializer exception that encapsulates the 
        /// source exception.</exception>
        public static T Deserialize<T>(byte[] data, BinarySerializerSettings? settings = null) =>
            (T)Deserialize(data, typeof(T), settings);

        /// <summary>
        /// Deserializes a <see cref="byte"/> array to a typed object.
        /// </summary>
        /// <param name="type">Type of the object to deserialize.</param>
        /// <param name="data">Data to deserialize.</param>
        /// <param name="settings">Optional settings for deserialization.</param>
        /// <returns>Returns an instance of type defined in <paramref name="type"/> 
        /// parameter with all data deserialized from the source <see cref="byte"/> array.</returns>
        /// <exception cref="BinarySerializerException">Serializer exception that encapsulates the 
        /// source exception.</exception>
        public static object Deserialize(byte[] data, Type type, BinarySerializerSettings? settings = null)
        {
            try
            {
                if (data is null)
                    throw new ArgumentNullException(nameof(data));

                if (type is null)
                    throw new ArgumentNullException(nameof(type));

                using (var buffer = new MemoryStream(data))
                using (var reader = new BinaryReader(buffer, GetTextEncodingFromSettings(settings)))
                {
                    object obj = BinaryDataSerializer.instance.Deserialize(type, reader) ??
                        throw new ArgumentException(
                            message: $"The type \"{type.Name}\" is not serializable.",
                            paramName: nameof(type));
                    
                    return obj;
                }
            }
            catch (Exception ex)
            {
                throw new BinarySerializerException(
                    message: "An error occurred during binary deserialization.",
                    sourceException: ex,
                    serializableType: type,
                    serializedData: data);
            }
        }

        private static Encoding GetTextEncodingFromSettings(BinarySerializerSettings? settings) =>
            settings is null
                ? BinarySerializerSettings.DEFAULT_SETTINGS.TextEncoding
                : settings.TextEncoding;
        #endregion
    }
}
