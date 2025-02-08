using System;
using System.IO;

namespace Argos.Framework.IO
{
    /// <summary>
    /// Contract for implement a binary serializer.
    /// </summary>
    public interface IBinaryDataSerializer
    {
        #region Methods & Functions
        /// <summary>
        /// Serializes the object to binary format.
        /// </summary>
        /// <param name="obj">Object to serialize.</param>
        /// <param name="writer"><see cref="BinaryWriter"/> instance to write the 
        /// data.</param>
        /// <returns>Returns <see langword="true"/> if the data can be serialize, 
        /// otherwise <see langword="false"/>.</returns>
        bool Serialize(object? obj, BinaryWriter writer);

        /// <summary>
        /// Deserializes a number of bytes to a specified data.
        /// </summary>
        /// <param name="type">Type of the data to deserialize.</param>
        /// <param name="reader"><see cref="BinaryReader"/> instnace from read the 
        /// data.</param>
        /// <returns>Returns the deserialized object if the data can be deserialized, 
        /// otherwise <see langword="null"/>.</returns>
        object? Deserialize(Type type, BinaryReader reader);

        /// <summary>
        /// Checks if the type is an expected type for this serializer.
        /// </summary>
        /// <param name="type"><see cref="Type"/> to serialize.</param>
        /// <returns>Returns <see langword="true"/> if the type is a supported type 
        /// for this serializer, otherwise <see langword="false"/>.</returns>
        bool IsExpectedType(Type? type);
        #endregion
    }
}
