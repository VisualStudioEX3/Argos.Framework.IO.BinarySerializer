using System;
using System.Collections;
using System.IO;

namespace Argos.Framework.IO.BinarySerializers.IEnumerableSerializers
{
    /// <summary>
    /// Binary data serializer for objects which implements <see cref="IDictionary"/> interface.
    /// </summary>
    /// <remarks>The key type must be any of the supported types by the 
    /// <see cref="PrimitiveValueBinaryDataSerializer"/>, <see cref="NonPrimitiveValueBinaryDataSerializer"/>
    /// and <see cref="EnumValueBinaryDataSerializer"/> serializers.
    /// <para/>
    /// The value type must be any of the supported types by the <see cref="PrimitiveValueBinaryDataSerializer"/>, 
    /// <see cref="NonPrimitiveValueBinaryDataSerializer"/>, <see cref="EnumValueBinaryDataSerializer"/>, 
    /// <see cref="ArrayBinaryDataSerializer"/>, <see cref="IEnumerableBinaryDataSerializer"/> and 
    /// <see cref="ObjectBinaryDataSerializer"/> serializers.
    /// <para/>
    /// Any <see cref="IDictionary"/> object with an unsupported type for keys or values will be ignored by this serializer.
    /// <para/>
    /// This serializer is part of <see cref="IEnumerableBinaryDataSerializer"/>.</remarks>
    public sealed class IDictionaryBinaryDataSerializer
        : IBinaryDataSerializer
    {
        #region Public Members
        /// <summary>
        /// Singleton instance of this serializer.
        /// </summary>
        public static readonly IDictionaryBinaryDataSerializer instance =
            new IDictionaryBinaryDataSerializer();
        #endregion

        #region Methods & Functions
        public object? Deserialize(Type type, BinaryReader reader)
        {
            IDictionary? dict = null;

            if (IsExpectedType(type))
            {
                int count = reader.ReadInt32();
                IBinaryDataSerializer? keySerializer = FindSerializerForKey(type);
                IBinaryDataSerializer? valueSerializer = FindSerializerForValue(type);

                dict = BinaryDataSerializerHelper.CreateGenericTypeInstance(type) as IDictionary;

                for (int i = 0; i < count; i++)
                    dict!.Add(
                        keySerializer!.Deserialize(type.GenericTypeArguments[0], reader), 
                        valueSerializer!.Deserialize(type.GenericTypeArguments[1], reader));
            }

            return dict;
        }

        public bool IsExpectedType(Type? type) =>
            !(type is null) &&
            type.ImplementInterface<IDictionary>() &&
            type.GenericTypeArguments[0].HasBinarySerializer() &&
            type.GenericTypeArguments[1].HasBinarySerializer();

        public bool Serialize(object? obj, BinaryWriter writer)
        {
            Type? type = obj?.GetType();

            if (IsExpectedType(type))
            {
                var dict = obj as IDictionary;
                IBinaryDataSerializer? keySerializer = FindSerializerForKey(type!);
                IBinaryDataSerializer? valueSerializer = FindSerializerForValue(type!);

                writer.Write(dict!.Count);

                foreach (DictionaryEntry entry in dict)
                {
                    keySerializer!.Serialize(entry.Key, writer);
                    valueSerializer!.Serialize(entry.Value, writer);
                }

                return true;
            }

            return false;
        }

        private IBinaryDataSerializer? FindSerializerForKey(Type type) =>
            type.GenericTypeArguments[0].FindBinarySerializer()!;

        private IBinaryDataSerializer? FindSerializerForValue(Type type) =>
            type.GenericTypeArguments[1].FindBinarySerializer()!;
        #endregion
    }
}
