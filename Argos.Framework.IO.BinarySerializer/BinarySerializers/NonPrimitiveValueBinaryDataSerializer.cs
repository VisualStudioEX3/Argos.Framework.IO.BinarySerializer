using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Argos.Framework.IO.BinarySerializers
{
    /// <summary>
    /// Binary data serializer for supported non primitive types.
    /// </summary>
    /// <remarks>Binary serializer for non primitive types that can be
    /// serializes from-to a fixed collection of bytes.
    /// <para/>
    /// The supported types are: <see cref="decimal"/>, <see cref="string"/>, 
    /// <see cref="DateTime"/>, <see cref="TimeSpan"/> and <see cref="Guid"/>.
    /// <para/>
    /// </remarks>
    public sealed class NonPrimitiveValueBinaryDataSerializer 
        : IBinaryDataSerializer
    {
        #region Constants
        private const int GUID_BYTES_LENGTH = 16;
        #endregion

        #region Public Members
        /// <summary>
        /// Singleton instance of this serializer.
        /// </summary>
        public static readonly NonPrimitiveValueBinaryDataSerializer instance =
            new NonPrimitiveValueBinaryDataSerializer();
        #endregion

        #region Methods & Functions
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Deserialize(Type type, BinaryReader reader)
        {
            object? value = null;

            if (IsExpectedType(type))
            {
                if (type == typeof(decimal))
                    value = reader.ReadDecimal();
                else if (type == typeof(string))
                    value = reader.ReadString();
                else if (type == typeof(DateTime))
                    value = DateTime.FromBinary(reader.ReadInt64());
                else if (type == typeof(TimeSpan))
                    value = TimeSpan.FromTicks(reader.ReadInt64());
                else if (type == typeof(Guid))
                    value = new Guid(reader.ReadBytes(GUID_BYTES_LENGTH));
            }

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsExpectedType(Type? type) =>
            !(type is null) &&
            !type.IsPrimitive && (
                type == typeof(decimal) ||
                type == typeof(string) ||
                type == typeof(DateTime) ||
                type == typeof(TimeSpan) ||
                type == typeof(Guid)
            );

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Serialize(object? obj, BinaryWriter writer)
        {
            if (obj is decimal @decimal)
                writer.Write(@decimal);
            else if (obj is string @string)
                writer.Write(@string);
            else if (obj is DateTime @DateTime)
                writer.Write(@DateTime.ToBinary());
            else if (obj is TimeSpan @TimeSpan)
                writer.Write(@TimeSpan.Ticks);
            else if (obj is Guid @Guid)
                writer.Write(@Guid.ToByteArray());
            else
                return false;

            return true;
        }
        #endregion
    }
}
