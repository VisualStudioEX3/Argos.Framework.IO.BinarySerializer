using System;
using System.IO;

namespace Argos.Framework.IO.BinarySerializers.ArraySerializers
{
    /// <summary>
    /// Binary data serializer for serializable object arrays.
    /// </summary>
    /// <remarks>This serializer uses the <see cref="ObjectBinaryDataSerializer"/>
    /// to serialize the array values.
    /// <para/>
    /// If the type of the array is not supported by the <see cref="ObjectBinaryDataSerializer"/>
    /// the array will be ignored by this serializer.
    /// <para/>
    /// This serializer is part of <see cref="ArrayBinaryDataSerializer"/>.</remarks>
    public sealed class ObjectArrayBinaryDataSerializer
        : IBinaryDataSerializer
    {
        #region Public Members
        /// <summary>
        /// Singleton instance of this serializer.
        /// </summary>
        public static readonly ObjectArrayBinaryDataSerializer instance =
            new ObjectArrayBinaryDataSerializer();
        #endregion

        public object? Deserialize(Type type, BinaryReader reader)
        {
            Array? array = null;
            Type itemType = type.GetElementType();

            if (IsExpectedType(itemType))
            {
                int length = reader.ReadInt32();

                array = Array.CreateInstance(itemType, length);

                for (int i = 0; i < length; i++)
                {
                    object value = ObjectBinaryDataSerializer
                        .instance.Deserialize(itemType, reader)!;

                    array.SetValue(value, i);
                }
            }

            return array;
        }

        public bool IsExpectedType(Type? type) =>
            ObjectBinaryDataSerializer.instance.IsExpectedType(type);

        public bool Serialize(object? obj, BinaryWriter writer)
        {
            Type? type = obj?.GetType();
            Type? itemType = type?.GetElementType();

            if (IsExpectedType(itemType))
            {
                var array = obj as Array;

                writer.Write(array!.Length);

                foreach (object item in array)
                    ObjectBinaryDataSerializer.instance.Serialize(item, writer);

                return true;
            }

            return false;
        }
    }
}
