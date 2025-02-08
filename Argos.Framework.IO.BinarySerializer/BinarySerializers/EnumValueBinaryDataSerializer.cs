#pragma warning disable IDE0045

using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Argos.Framework.IO.BinarySerializers
{
    /// <summary>
    /// Binary data serializer for enumerations.
    /// </summary>
    /// <remarks>Serializes enumeration values using the specified type for the enum, any 
    /// integral type: <see cref="byte"/>, <see cref="sbyte"/>, <see cref="short"/>, 
    /// <see cref="ushort"/>, <see cref="int"/>, <see cref="uint"/>, <see cref="long"/>
    /// and <see cref="ulong"/>. 
    /// <para/>
    /// If the enumeration not specified any type, the value
    /// is serialized as <see cref="int"/>.</remarks>
    public sealed class EnumValueBinaryDataSerializer
        : IBinaryDataSerializer
    {
        #region Public Members
        /// <summary>
        /// Singleton instance of this serializer.
        /// </summary>
        public static readonly EnumValueBinaryDataSerializer instance =
            new EnumValueBinaryDataSerializer();
        #endregion

        #region Methods & Functions
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Deserialize(Type type, BinaryReader reader)
        {
            object? value = null;

            if (IsExpectedType(type))
            {
                Type enumType = type.GetEnumUnderlyingType();

                if (enumType == typeof(byte))
                    value = reader.ReadByte();
                else if (enumType == typeof(sbyte))
                    value = reader.ReadSByte();
                else if (enumType == typeof(short))
                    value = reader.ReadInt16();
                else if (enumType == typeof(ushort))
                    value = reader.ReadUInt16();
                else if (enumType == typeof(uint))
                    value = reader.ReadUInt32();
                else if (enumType == typeof(long))
                    value = reader.ReadInt64();
                else if (enumType == typeof(ulong))
                    value = reader.ReadUInt64();
                else
                    value = reader.ReadInt32();

                if (!Enum.IsDefined(type, value))
                    throw new InvalidDataException($"{value} is not a valid value for {type.Name} enumeration.");
            }

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsExpectedType(Type? type) =>
            type?.IsEnum ?? false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Serialize(object? obj, BinaryWriter writer)
        {
            if (obj is Enum)
            {
                Type enumType = obj.GetType().GetEnumUnderlyingType();

                if (enumType == typeof(byte))
                    writer.Write((byte)obj);
                else if (enumType == typeof(sbyte))
                    writer.Write((sbyte)obj);
                else if (enumType == typeof(short))
                    writer.Write((short)obj);
                else if (enumType == typeof(ushort))
                    writer.Write((ushort)obj);
                else if (enumType == typeof(uint))
                    writer.Write((uint)obj);
                else if (enumType == typeof(long))
                    writer.Write((long)obj);
                else if (enumType == typeof(ulong))
                    writer.Write((ulong)obj);
                else
                    writer.Write((int)obj);

                return true;
            }

            return false;
        }
        #endregion
    }
}
