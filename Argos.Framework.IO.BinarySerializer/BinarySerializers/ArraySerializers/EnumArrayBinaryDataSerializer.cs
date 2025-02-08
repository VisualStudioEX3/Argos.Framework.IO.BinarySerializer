﻿using System;
using System.IO;

namespace Argos.Framework.IO.BinarySerializers.ArraySerializers
{
    /// <summary>
    /// Binary data serializer for <see cref="Enum"/> arrays.
    /// </summary>
    /// <remarks>This serializer uses the <see cref="EnumValueBinaryDataSerializer"/>
    /// to serialize the array values.
    /// <para/>
    /// If the type of the array is not supported by the <see cref="EnumValueBinaryDataSerializer"/>
    /// the array will be ignored by this serializer.
    /// <para/>
    /// This serializer is part of <see cref="ArrayBinaryDataSerializer"/>.</remarks>
    public sealed class EnumArrayBinaryDataSerializer
        : IBinaryDataSerializer
    {
        #region Public Members
        /// <summary>
        /// Singleton instance of this serializer.
        /// </summary>
        public static readonly EnumArrayBinaryDataSerializer instance =
            new EnumArrayBinaryDataSerializer();
        #endregion

        #region Methods & Functions
        public object? Deserialize(Type type, BinaryReader reader)
        {
            Array? array = null;
            Type itemType = type.GetElementType();

            if (IsExpectedType(itemType))
            {
                int length = reader.ReadInt32();
                Type enumType = itemType.GetEnumUnderlyingType();
                
                array = Array.CreateInstance(enumType, length);

                for (int i = 0; i < length; i++)
                {
                    object value = EnumValueBinaryDataSerializer
                        .instance.Deserialize(itemType, reader)!;

                    array.SetValue(value, i);
                }
            }

            return array;
        }

        public bool IsExpectedType(Type? type) =>
            EnumValueBinaryDataSerializer.instance.IsExpectedType(type);

        public bool Serialize(object? obj, BinaryWriter writer)
        {
            Type? type = obj?.GetType();
            Type? itemType = type?.GetElementType();

            if (IsExpectedType(itemType))
            {
                var array = obj as Array;

                writer.Write(array!.Length);

                foreach (object item in array)
                    EnumValueBinaryDataSerializer.instance.Serialize(item, writer);

                return true;
            }

            return false;
        }
        #endregion
    }
}
