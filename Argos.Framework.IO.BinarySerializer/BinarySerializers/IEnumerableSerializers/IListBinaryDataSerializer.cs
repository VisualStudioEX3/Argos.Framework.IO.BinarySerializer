using System;
using System.Collections;
using System.IO;

namespace Argos.Framework.IO.BinarySerializers.IEnumerableSerializers
{
    /// <summary>
    /// Binary data serializer for objects which implements <see cref="IList"/> interface.
    /// </summary>
    /// <remarks>
    /// The value type must be any of the supported types by the <see cref="PrimitiveValueBinaryDataSerializer"/>, 
    /// <see cref="NonPrimitiveValueBinaryDataSerializer"/>, <see cref="EnumValueBinaryDataSerializer"/>, 
    /// <see cref="ArrayBinaryDataSerializer"/>, <see cref="IEnumerableBinaryDataSerializer"/> and 
    /// <see cref="ObjectBinaryDataSerializer"/> serializers.
    /// <para/>
    /// Any <see cref="IList"/> object with an unsupported type for values will be ignored by this serializer.
    /// <para/>
    /// This serializer is part of <see cref="IEnumerableBinaryDataSerializer"/>.</remarks>
    public sealed class IListBinaryDataSerializer
        : IBinaryDataSerializer
    {
        #region Public Members
        /// <summary>
        /// Singleton instance of this serializer.
        /// </summary>
        public static readonly IListBinaryDataSerializer instance =
            new IListBinaryDataSerializer();
        #endregion

        #region Methods & Functions
        public object? Deserialize(Type type, BinaryReader reader)
        {
            IList? list = null;

            if (IsExpectedType(type))
            {
                int count = reader.ReadInt32();
                IBinaryDataSerializer? serializer = FindSerializer(type);

                list = BinaryDataSerializerHelper.CreateGenericTypeInstance(type) as IList;

                for (int i = 0; i < count; i++)
                    list!.Add(serializer!.Deserialize(type.GenericTypeArguments[0], reader));
            }

            return list;
        }

        public bool IsExpectedType(Type? type) =>
            !(type is null) &&
            type.ImplementInterface<IList>() &&
            type.GenericTypeArguments[0].HasBinarySerializer();

        public bool Serialize(object? obj, BinaryWriter writer)
        {
            Type? type = obj?.GetType();

            if (IsExpectedType(type))
            {
                var list = obj as ICollection;
                IBinaryDataSerializer? serializer = FindSerializer(type!);

                writer.Write(list!.Count);

                foreach (object value in list)
                    serializer!.Serialize(value, writer);

                return true;
            }

            return false;
        }

        private IBinaryDataSerializer? FindSerializer(Type type) =>
            type.GenericTypeArguments[0].FindBinarySerializer();
        #endregion
    }
}
