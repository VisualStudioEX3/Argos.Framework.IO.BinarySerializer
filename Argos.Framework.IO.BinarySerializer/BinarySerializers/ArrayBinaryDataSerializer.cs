using Argos.Framework.IO.BinarySerializers.ArraySerializers;
using System;
using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;

namespace Argos.Framework.IO.BinarySerializers
{
    /// <summary>
    /// Binary data serializer for arrays.
    /// </summary>
    /// <remarks>This serializer can serialize and deserialize any array of 
    /// single dimension of the following value types:
    /// <list type="bullet">
    /// <item>Primitive types supported by <see cref="PrimitiveValueBinaryDataSerializer"/>.</item>
    /// <item>Non primitive types supported by <see cref="NonPrimitiveValueBinaryDataSerializer"/>.</item>
    /// <item>Enumeration types supported by <see cref="EnumValueBinaryDataSerializer"/>.</item>
    /// <item><see cref="IEnumerable"/> types supported by <see cref="IEnumerableBinaryDataSerializer"/>.</item>
    /// <item>Serializable objects supported by <see cref="ObjectBinaryDataSerializer"/>.</item>
    /// <item>Arrays supported by this serializer.</item>
    /// </list>
    /// If the type of the array is not supported by the serializers listed above, or had more than one dimension,
    /// the array will be ignored by this serializer.
    /// </remarks>
    public sealed class ArrayBinaryDataSerializer
        : IBinaryDataSerializer
    {
        #region Constants
        private static readonly IBinaryDataSerializer[] ARRAY_SERIALIZERS =
        {
            ByteArrayBinaryDataSerializer.instance,
            CharArrayBinaryDataSerializer.instance,
            PrimitiveArrayBinaryDataSerializer.instance,
            NonPrimitiveArrayBinaryDataSerializer.instance,
            EnumArrayBinaryDataSerializer.instance,
            new ArrayBinaryDataSerializer(),
            IEnumerableArrayBinaryDataSerializer.instance,
            ObjectArrayBinaryDataSerializer.instance,
        };
        #endregion

        #region Public Members
        /// <summary>
        /// Singleton instance of this serializer.
        /// </summary>
        public static readonly ArrayBinaryDataSerializer instance =
            new ArrayBinaryDataSerializer();
        #endregion

        #region Methods & Functions
        public object? Deserialize(Type type, BinaryReader reader)
        {
            object? obj = null;

            if (IsExpectedType(type))
                foreach (IBinaryDataSerializer serializer in ARRAY_SERIALIZERS)
                {
                    obj = serializer.Deserialize(type, reader);

                    if (obj != null)
                        break;
                }

            return obj;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsExpectedType(Type? type) =>
            !(type is null) &&
            type.IsArray && 
            type.GetArrayRank() == 1;

        public bool Serialize(object? obj, BinaryWriter writer)
        {
            foreach (IBinaryDataSerializer serializer in ARRAY_SERIALIZERS)
                if (serializer.Serialize(obj, writer))
                    return true;

            return false;
        }
        #endregion
    }
}
