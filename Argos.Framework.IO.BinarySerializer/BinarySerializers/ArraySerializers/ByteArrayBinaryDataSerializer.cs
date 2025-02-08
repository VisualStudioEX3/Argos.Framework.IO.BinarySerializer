using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Argos.Framework.IO.BinarySerializers.ArraySerializers
{
    /// <summary>
    /// Binary data serializer for <see cref="byte"/> arrays.
    /// </summary>
    /// <remarks>This serializer not uses the <see cref="PrimitiveValueBinaryDataSerializer"/>
    /// to serialize the array values. Directly writes or read the byte array from the memory stream.
    /// <para/>
    /// This serializer is part of <see cref="ArrayBinaryDataSerializer"/>.</remarks>
    public sealed class ByteArrayBinaryDataSerializer
        : IBinaryDataSerializer
    {
        #region Public Members
        /// <summary>
        /// Singleton instance of this serializer.
        /// </summary>
        public static readonly ByteArrayBinaryDataSerializer instance =
            new ByteArrayBinaryDataSerializer();
        #endregion

        #region Methods & Functions
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Deserialize(Type type, BinaryReader reader)
        {
            object? obj = null;

            if (IsExpectedType(type))
                obj = reader.ReadBytes(reader.ReadInt32());

            return obj;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsExpectedType(Type? type) =>
            type == typeof(byte[]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Serialize(object? obj, BinaryWriter writer)
        {
            if (IsExpectedType(obj?.GetType()))
            {
                var bytes = obj as byte[];
                
                writer.Write(bytes!.Length);
                writer.Write(bytes);

                return true;
            }

            return false;
        }
        #endregion
    }
}
