using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Argos.Framework.IO.BinarySerializers.ArraySerializers
{
    /// <summary>
    /// Binary data serializer for <see cref="char"/> arrays.
    /// </summary>
    /// <remarks>This serializer not uses the <see cref="PrimitiveValueBinaryDataSerializer"/>
    /// to serialize the array values. Directly writes or read the char array from the memory stream.
    /// <para/>
    /// This serializer is part of <see cref="ArrayBinaryDataSerializer"/>.</remarks>
    public sealed class CharArrayBinaryDataSerializer
        : IBinaryDataSerializer
    {
        #region Public Members
        /// <summary>
        /// Singleton instance of this serializer.
        /// </summary>
        public static readonly CharArrayBinaryDataSerializer instance =
            new CharArrayBinaryDataSerializer();
        #endregion

        #region Methods & Functions
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Deserialize(Type type, BinaryReader reader)
        {
            object? obj = null;

            if (IsExpectedType(type))
                obj = reader.ReadChars(reader.ReadInt32());

            return obj;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsExpectedType(Type? type) =>
            type == typeof(char[]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Serialize(object? obj, BinaryWriter writer)
        {
            if (IsExpectedType(obj?.GetType()))
            {
                var chars = obj as char[];

                writer.Write(chars!.Length);
                writer.Write(chars);

                return true;
            }

            return false;
        }
        #endregion
    }
}
