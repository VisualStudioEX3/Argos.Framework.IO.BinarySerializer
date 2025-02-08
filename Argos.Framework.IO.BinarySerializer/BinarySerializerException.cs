using System;

namespace Argos.Framework.IO
{
    /// <summary>
    /// Binary serializer exception.
    /// </summary>
    public class BinarySerializerException
        : Exception
    {
        #region Constructor
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="sourceException">Source exception.</param>
        /// <param name="serializableType">Type to serialize or deserialize. 
        /// This value is added to <see cref="Exception.Data"/> dictionary.</param>
        /// <param name="objToSerialize">Object to serialize. This value is added to 
        /// <see cref="Exception.Data"/> dictionary.</param>
        /// <param name="serializedData">Byte array with the serialized data. This 
        /// value is added to <see cref="Exception.Data"/> dictionary.</param>
        public BinarySerializerException(string message, Exception sourceException, 
            Type? serializableType, object? objToSerialize = null, byte[]? serializedData = null) :
            base(message, sourceException)
        {
            this.Data.Add("Serializable type", serializableType);
            this.Data.Add("Object to serialize", objToSerialize);
            this.Data.Add("Serialized data", serializedData);
        }
        #endregion
    }
}
