using System.Text;

namespace Argos.Framework.IO
{
    /// <summary>
    /// Specifies the settings for a <see cref="BinarySerializer"/> operation.
    /// </summary>
    public sealed class BinarySerializerSettings
    {
        #region Constants
        private static readonly Encoding DEFAULT_TEXT_ENCODING = Encoding.UTF8;

        /// <summary>
        /// Default settings.
        /// </summary>
        public static readonly BinarySerializerSettings DEFAULT_SETTINGS
            = new BinarySerializerSettings();
        #endregion

        #region Private members
        private Encoding textEnconding = DEFAULT_TEXT_ENCODING;
        #endregion

        #region Properties
        /// <summary>
        /// The character encoding.
        /// </summary>
        /// <remarks>This setting is used on <see cref="char"/> and <see cref="string"/>
        /// serialization and deserialization operations.
        /// <para/>
        /// By default is <see cref="UTF8Encoding"/>.</remarks>
        public Encoding TextEncoding
        {
            get => textEnconding;
            set => textEnconding = 
                value is null 
                    ? DEFAULT_TEXT_ENCODING 
                    : value;
        }
        #endregion
    }
}
