using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Argos.Framework.IO.BinarySerializers
{
    /// <summary>
    /// Helper class for binary data serializers.
    /// </summary>
    public static class BinaryDataSerializerHelper
    {
        #region Constants
        private static readonly IBinaryDataSerializer[] BUILT_IN_DATA_SERIALIZERS =
            new IBinaryDataSerializer[]
            {
                PrimitiveValueBinaryDataSerializer.instance,
                NonPrimitiveValueBinaryDataSerializer.instance,
                EnumValueBinaryDataSerializer.instance,
                ArrayBinaryDataSerializer.instance,
                IEnumerableBinaryDataSerializer.instance,
                ObjectBinaryDataSerializer.instance,
            };
        #endregion

        #region Methods & Functions
        /// <summary>
        /// Creates an instance of a generic type.
        /// </summary>
        /// <param name="genericType">Generic type to create instance.</param>
        /// <param name="arguments">Optional array of values to pass on the generic type constructor. 
        /// If the generic type constructor doesn't had parameters then leave this array empty.</param>
        /// <returns>Returns an instance of the generic type, or <see langword="null"/> if the type is 
        /// not a generic type.</returns>
        public static object? CreateGenericTypeInstance(Type genericType, params object[] arguments)
        {
            object? instance = null;

            if (genericType.IsGenericType)
                instance = arguments.Length > 0 
                    ? Activator.CreateInstance(genericType, arguments) 
                    : Activator.CreateInstance(genericType);

            return instance;
        }
        #endregion

        #region Extension Methods
        /// <summary>
        /// Checks if a type implements an specific interface.
        /// </summary>
        /// <param name="type"><see cref="Type"/> instance.</param>
        /// <param name="interfaceType">Interface type.</param>
        /// <returns>Returns <see langword="true"/> if the type implement the interface, 
        /// otherwise <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ImplementInterface(this Type type, Type interfaceType) =>
            type.GetInterfaces()
                .Any(e => e == interfaceType);

        /// <summary>
        /// Checks if a type implements an specific interface.
        /// </summary>
        /// <typeparam name="T">Interface type.</typeparam>
        /// <param name="type"><see cref="Type"/> instance.</param>
        /// <returns>Returns <see langword="true"/> if the type implement the interface, 
        /// otherwise <see langword="false"/>.</returns>
        public static bool ImplementInterface<T>(this Type type) =>
            type.ImplementInterface(typeof(T));

        /// <summary>
        /// Checks if the type has an specific attribute.
        /// </summary>
        /// <typeparam name="T">Attribute type to check.</typeparam>
        /// <param name="type"><see cref="Type"/> instance.</param>
        /// <returns>Returns <see langword="true"/> if the type has the attribute, 
        /// otherwise <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAttribute<T>(this Type type)
            where T : Attribute =>
                !(type.GetCustomAttribute<T>() is null);

        /// <summary>
        /// Checks if the <see cref="MemberInfo"/> has an specific attribute.
        /// </summary>
        /// <typeparam name="T">Attribute type to check.</typeparam>
        /// <param name="type"><see cref="Type"/> instance.</param>
        /// <returns>Returns <see langword="true"/> if the <see cref="MemberInfo"/> has the 
        /// attribute, otherwise <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAttribute<T>(this MemberInfo member)
            where T : Attribute =>
                !(member.GetCustomAttribute<T>() is null);

        /// <summary>
        /// Find the binary serializer available for this type.
        /// </summary>
        /// <param name="type"><see cref="Type"/> instance.</param>
        /// <returns>Returns the <see cref="IBinaryDataSerializer"/> available for this type, 
        /// otherwise <see langword="null"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IBinaryDataSerializer? FindBinarySerializer(this Type type)
        {
            foreach (IBinaryDataSerializer serializer in BUILT_IN_DATA_SERIALIZERS)
                if (serializer.IsExpectedType(type))
                    return serializer;

            return null;
        }

        /// <summary>
        /// Checks if this type has a binary serializer available.
        /// </summary>
        /// <param name="type"><see cref="Type"/> instance.</param>
        /// <returns>Returns <see langword="true"/> if this type has an available binary serializer, 
        /// otherwise <see langword="false"/>.</returns>
        public static bool HasBinarySerializer(this Type type) =>
            type.HasBinarySerializer(out _);

        /// <summary>
        /// Checks if this type has a binary serializer available and returns the serializer instance.
        /// </summary>
        /// <param name="type"><see cref="Type"/> instance.</param>
        /// <param name="serializer">The <see cref="IBinaryDataSerializer"/> instance for this type.</param>
        /// <returns>Returns <see langword="true"/> if this type has an available binary serializer, 
        /// otherwise <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasBinarySerializer(this Type type, out IBinaryDataSerializer? serializer)
        {
            serializer = type.FindBinarySerializer();

            return !(serializer is null);
        }
        #endregion
    }
}
