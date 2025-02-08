using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Argos.Framework.IO.BinarySerializers
{
    /// <summary>
    /// Binary data serializer for primitive data types.
    /// </summary>
    /// <remarks>The primitive types are: 
    /// <see cref="bool"/>, <see cref="byte"/>, <see cref="sbyte"/>, 
    /// <see cref="char"/>, <see cref="short"/>, <see cref="ushort"/>, 
    /// <see cref="int"/>, <see cref="uint"/>, <see cref="long"/>, 
    /// <see cref="ulong"/>, <see cref="float"/> and <see cref="double"/>.
    /// <para/>
    /// </remarks>
    public sealed class PrimitiveValueBinaryDataSerializer 
        : IBinaryDataSerializer
    {
        #region Public Members
        /// <summary>
        /// Singleton instance of this serializer.
        /// </summary>
        public static readonly PrimitiveValueBinaryDataSerializer instance =
            new PrimitiveValueBinaryDataSerializer();
        #endregion

        #region Methods & Functions
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? Deserialize(Type type, BinaryReader reader)
        {
            object? value = null;

            if (IsExpectedType(type))
            {
                if (type == typeof(bool))
                    value = reader.ReadBoolean();
                else if (type == typeof(byte))
                    value = reader.ReadByte();
                else if (type == typeof(sbyte))
                    value = reader.ReadSByte();
                else if (type == typeof(char))
                    value = reader.ReadChar();
                else if (type == typeof(short))
                    value = reader.ReadInt16();
                else if (type == typeof(ushort))
                    value = reader.ReadUInt16();
                else if (type == typeof(int))
                    value = reader.ReadInt32();
                else if (type == typeof(uint))
                    value = reader.ReadUInt32();
                else if (type == typeof(long))
                    value = reader.ReadInt64();
                else if (type == typeof(ulong))
                    value = reader.ReadUInt64();
                else if (type == typeof(float))
                    value = reader.ReadSingle();
                else if (type == typeof(double))
                    value = reader.ReadDouble();
            }

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsExpectedType(Type? type) =>
            type?.IsPrimitive ?? false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Serialize(object? obj, BinaryWriter writer)
        {
            if (obj is bool @bool)
                writer.Write(@bool);
            else if (obj is byte @byte)
                writer.Write(@byte);
            else if (obj is sbyte @sbyte)
                writer.Write(@sbyte);
            else if (obj is char @char)
                writer.Write(@char);
            else if (obj is short @short)
                writer.Write(@short);
            else if (obj is ushort @ushort)
                writer.Write(@ushort);
            else if (obj is int @int)
                writer.Write(@int);
            else if (obj is uint @uint)
                writer.Write(@uint);
            else if (obj is long @long)
                writer.Write(@long);
            else if (obj is ulong @ulong)
                writer.Write(@ulong);
            else if (obj is float @float)
                writer.Write(@float);
            else if (obj is double @double)
                writer.Write(@double);
            else
                return false;

            return true;
        }
        #endregion
    }
}
