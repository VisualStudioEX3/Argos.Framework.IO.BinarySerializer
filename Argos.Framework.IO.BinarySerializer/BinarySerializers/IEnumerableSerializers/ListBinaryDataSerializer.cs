using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace Argos.Framework.IO.BinarySerializers.IEnumerableSerializers
{
    /// <summary>
    /// Binary data serializer for <see cref="List{T}"/> type.
    /// </summary>
    /// <remarks>This serializer treats <see cref="List{T}"/> as arrays, using the
    /// <see cref="ArrayBinaryDataSerializer"/> to serialize and deserialize the list values, and using the
    /// <see cref="IEnumerable{T}"/> parameter constructor of <see cref="List{T}"/> to optimize the
    /// serialization and deserialization of the list values.
    /// <para/>
    /// If the generic type of the list is not supported by the <see cref="ArrayBinaryDataSerializer"/>
    /// the list will be ignored by this serializer.
    /// <para/>
    /// This serializer is part of <see cref="IEnumerableBinaryDataSerializer"/>.</remarks>
    public sealed class ListBinaryDataSerializer
        : IBinaryDataSerializer
    {
        #region Public Members
        /// <summary>
        /// Singleton instance of this serializer.
        /// </summary>
        public static readonly ListBinaryDataSerializer instance =
            new ListBinaryDataSerializer();
        #endregion

        #region Methods & Functions
        public object? Deserialize(Type type, BinaryReader reader)
        {
            object? list = null;

            if (IsExpectedType(type))
            {
                var values = ArrayBinaryDataSerializer
                    .instance.Deserialize(type.GenericTypeArguments[0], reader) as Array;

                list = BinaryDataSerializerHelper.CreateGenericTypeInstance(type, values!);
            }

            return list;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsExpectedType(Type? type) =>
            type == typeof(List<>) &&
            type.GenericTypeArguments[0].HasBinarySerializer();

        public bool Serialize(object? obj, BinaryWriter writer)
        {
            Type? type = obj?.GetType();

            if (IsExpectedType(type))
            {
                var list = obj as ICollection;
                var array = Array.CreateInstance(type!.GenericTypeArguments[0], list!.Count);

                list.CopyTo(array, 0);

                return ArrayBinaryDataSerializer.instance.Serialize(array, writer);
            }

            return false;
        }
        #endregion
    }
}
