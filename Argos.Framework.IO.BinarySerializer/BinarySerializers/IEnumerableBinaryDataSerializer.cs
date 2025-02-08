using Argos.Framework.IO.BinarySerializers.IEnumerableSerializers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace Argos.Framework.IO.BinarySerializers
{
    /// <summary>
    /// Binary data serializer for supported <see cref="IEnumerable"/> types.
    /// </summary>
    /// <remarks>
    /// The supported types are <see cref="List{T}"/> and objects which implements 
    /// <see cref="IList"/> or <see cref="IDictionary"/> interfaces.
    /// </remarks>
    public sealed class IEnumerableBinaryDataSerializer
        : IBinaryDataSerializer
    {
        #region Constants
        private static readonly IBinaryDataSerializer[] IENUMERABLE_SERIALIZERS =
            new IBinaryDataSerializer[]
            {
                ListBinaryDataSerializer.instance,
                IListBinaryDataSerializer.instance,
                IDictionaryBinaryDataSerializer.instance,
            };
        #endregion

        #region Public Members
        /// <summary>
        /// Singleton instance of this serializer.
        /// </summary>
        public static readonly IEnumerableBinaryDataSerializer instance =
            new IEnumerableBinaryDataSerializer();
        #endregion

        #region Methods & Functions
        public object? Deserialize(Type type, BinaryReader reader)
        {
            object? obj = null;

            if (IsExpectedType(type))
                foreach (IBinaryDataSerializer serializer in IENUMERABLE_SERIALIZERS)
                {
                    obj = serializer.Deserialize(type, reader);

                    if (obj != null)
                        break;
                }

            return obj;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsExpectedType(Type? type) =>
            (type?.IsGenericType ?? false) &&
            type.ImplementInterface<IEnumerable>();

        public bool Serialize(object? obj, BinaryWriter writer)
        {
            if (obj is IEnumerable)
                foreach (IBinaryDataSerializer serializer in IENUMERABLE_SERIALIZERS)
                    if (serializer.Serialize(obj, writer))
                        return true;

            return false;
        }
        #endregion
    }
}
