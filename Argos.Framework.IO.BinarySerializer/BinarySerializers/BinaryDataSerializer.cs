using System;
using System.IO;

namespace Argos.Framework.IO.BinarySerializers
{
    /// <summary>
    /// Base serializer to serialize any of the supported types by 
    /// the <see cref="BinarySerializer"/>.
    /// </summary>
    public sealed class BinaryDataSerializer
        : IBinaryDataSerializer
    {
        #region Public Members
        /// <summary>
        /// Singleton instance of this serializer.
        /// </summary>
        public static readonly BinaryDataSerializer instance =
            new BinaryDataSerializer();
        #endregion

        #region Methods & Functions
        public object? Deserialize(Type type, BinaryReader reader)
        {
            object? obj = null;

            if (type.HasBinarySerializer(out IBinaryDataSerializer? serializer))
                obj = serializer!.Deserialize(type, reader);

            return obj;
        }

        public bool IsExpectedType(Type? type) =>
            type?.HasBinarySerializer() ?? false;

        public bool Serialize(object? obj, BinaryWriter writer) =>
            !(obj is null) &&
            obj.GetType()
               .HasBinarySerializer(out IBinaryDataSerializer? serializer) && 
            serializer!.Serialize(obj, writer);
        #endregion
    }
}
