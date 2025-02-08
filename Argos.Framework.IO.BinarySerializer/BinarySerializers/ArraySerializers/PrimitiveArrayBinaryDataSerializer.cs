using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace Argos.Framework.IO.BinarySerializers.ArraySerializers
{
    /// <summary>
    /// Binary data serializer for primitive value arrays.
    /// </summary>
    /// <remarks>This serializer uses the <see cref="PrimitiveValueBinaryDataSerializer"/>
    /// to serialize the array values.
    /// <para/>
    /// If the type of the array is not supported by the <see cref="PrimitiveValueBinaryDataSerializer"/>
    /// the array will be ignored by this serializer.
    /// <para/>
    /// This serializer is part of <see cref="ArrayBinaryDataSerializer"/>.</remarks>
    public sealed class PrimitiveArrayBinaryDataSerializer
        : IBinaryDataSerializer
    {
        #region Constants
        // This dictionary not include byte or char types because them has his own array serializers:
        private static readonly Dictionary<Type, int> FIXED_TYPE_SIZES = new Dictionary<Type, int>
        {
            [typeof(bool)] = sizeof(bool),
            [typeof(sbyte)] = sizeof(sbyte),
            [typeof(short)] = sizeof(short),
            [typeof(ushort)] = sizeof(ushort),
            [typeof(int)] = sizeof(int),
            [typeof(uint)] = sizeof(uint),
            [typeof(long)] = sizeof(long),
            [typeof(ulong)] = sizeof(ulong),
            [typeof(float)] = sizeof(float),
            [typeof(double)] = sizeof(double),
            [typeof(decimal)] = sizeof(decimal),
        };
        #endregion

        #region Public Members
        /// <summary>
        /// Singleton instance of this serializer.
        /// </summary>
        public static readonly PrimitiveArrayBinaryDataSerializer instance =
            new PrimitiveArrayBinaryDataSerializer();
        #endregion

        #region Methods & Functions
        public object? Deserialize(Type type, BinaryReader reader)
        {
            Array? array = null;
            Type itemType = type.GetElementType();

            if (IsExpectedType(itemType))
            {
                int length = reader.ReadInt32();
                int totalBytes = CalculateArraySize(itemType, length);
                byte[] rawArray = reader.ReadBytes(totalBytes);

                array = BytesToArray(rawArray, itemType, length);
            }

            return array;
        }

        public bool IsExpectedType(Type? type) =>
            PrimitiveValueBinaryDataSerializer.instance.IsExpectedType(type);

        public bool Serialize(object? obj, BinaryWriter writer)
        {
            Type? type = obj?.GetType();
            Type? itemType = type?.GetElementType();

            if (IsExpectedType(itemType))
            {
                var array = obj as Array;
                byte[] bytes = ArrayToBytes(array!, itemType!);

                writer.Write(array!.Length);
                writer.Write(bytes);

                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int CalculateArraySize(Type itemType, int arrayLength) =>
            FIXED_TYPE_SIZES[itemType] * arrayLength;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte[] ArrayToBytes(Array array, Type type)
        {
            int length = array.Length;
            int totalBytes = CalculateArraySize(type, length);
            var rawArray = new byte[totalBytes];

            Buffer.BlockCopy(array, 0, rawArray, 0, totalBytes);

            return rawArray;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Array BytesToArray(byte[] bytes, Type arrayType, int arrayLength)
        {
            var array = Array.CreateInstance(arrayType, arrayLength);

            Buffer.BlockCopy(bytes, 0, array, 0, bytes.Length);

            return array;
        }
        #endregion
    }
}
