using System;

namespace Argos.Framework.IO
{
    /// <summary>
    /// Indicates that a field or property of a serializable class should not be serialized by
    /// the <see cref="BinarySerializer"/>. This class cannot be inherited.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false)]
    public sealed class BinaryIgnoreAttribute 
        : Attribute
    {
    }
}
