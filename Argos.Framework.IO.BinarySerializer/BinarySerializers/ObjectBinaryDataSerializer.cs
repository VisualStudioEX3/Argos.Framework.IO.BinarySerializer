using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Argos.Framework.IO.BinarySerializers
{
    /// <summary>
    /// Binary data serializer for serializable objects.
    /// </summary>
    /// <remarks>This serializer try to serialize any class marked as <see cref="SerializableAttribute"/>
    /// and his public serializable fields or properties, except ones marked with 
    /// <see cref="NonSerializedAttribute"/> or <see cref="BinaryIgnoreAttribute"/>. The non serializable 
    /// fields or properties are ignored.
    /// <para/>
    /// If the <see cref="DataContractAttribute"/> is present, the serializer ignores 
    /// <see cref="NonSerializedAttribute"/> and <see cref="BinaryIgnoreAttribute"/> attributes and try to 
    /// serialize only all members, private or public, marked with the <see cref="DataMemberAttribute"/>
    /// attribute. The attribute values (Name, Namespace, Order, IsRequired, etc...) are ignored.
    /// <para/>
    /// The <see cref="ObjectBinaryDataSerializer"/> serializes all members sorted by name to ensure always 
    /// the same serialziation and deserialization member order. The 
    /// <see cref="DataMemberAttribute.Order"/> attribute value doesn't affect this behavior. 
    /// <para/>
    /// All serializable members are required for serialization and deserialization. The 
    /// <see cref="DataMemberAttribute.IsRequired"/> attribute value doesn't affect this behavior.
    /// <para/>
    /// All member types must be any of the supported types by the 
    /// <see cref="PrimitiveValueBinaryDataSerializer"/>, <see cref="NonPrimitiveValueBinaryDataSerializer"/>,
    /// <see cref="EnumValueBinaryDataSerializer"/>, <see cref="ArrayBinaryDataSerializer"/>,
    /// <see cref="IEnumerableBinaryDataSerializer"/> serializers, incluiding this one for serializable objects.
    /// <para/>
    /// </remarks>
    public sealed class ObjectBinaryDataSerializer
        : IBinaryDataSerializer
    {
        #region Constants
        private static readonly IBinaryDataSerializer[] VALIDATE_SERIALIZERS = new IBinaryDataSerializer[]
        {
            PrimitiveValueBinaryDataSerializer.instance,
            NonPrimitiveValueBinaryDataSerializer.instance,
            EnumValueBinaryDataSerializer.instance,
            ArrayBinaryDataSerializer.instance,
            IEnumerableBinaryDataSerializer.instance,
        };
        #endregion

        #region Structs
        private struct ObjectMember
        {
            #region Private members
            private MemberInfo member;
            #endregion

            #region Properties
            public Type MemberType { get; private set; }
            #endregion

            #region Methods & Functions
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static ObjectMember CreateFromMemberInfo(MemberInfo member)
            {
                var objectMember = new ObjectMember
                {
                    member = member
                };

                if (objectMember.member is FieldInfo field)
                    objectMember.MemberType = field.FieldType;
                else if (objectMember.member is PropertyInfo property)
                    objectMember.MemberType = property.PropertyType;

                return objectMember;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly void SetValue(object obj, object value)
            {
                if (member is FieldInfo field)
                    field.SetValue(obj, value);
                else if (member is PropertyInfo property)
                    property.SetValue(obj, value);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly object? GetValue(object obj)
            {
                object? value = null;

                if (member is FieldInfo field)
                    value = field.GetValue(obj);
                else if (member is PropertyInfo property)
                    value = property.GetValue(obj);

                return value;
            }

            #region DataContract filters
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool IsDataMember(MemberInfo member) =>
                !member.HasAttribute<DataMemberAttribute>() &&
                (IsDataMemberField(member) || IsDataMemberProperty(member));

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool IsDataMemberField(MemberInfo member) =>
                member is FieldInfo field &&
                !field.IsLiteral;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool IsDataMemberProperty(MemberInfo member) =>
                member is PropertyInfo property &&
                property.CanWrite;
            #endregion

            #region Serializable filters
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool IsSerializableMember(MemberInfo member) =>
                !member.HasAttribute<BinaryIgnoreAttribute>() &&
                (IsSerializableField(member) || IsSerializableProperty(member));

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool IsSerializableField(MemberInfo member) =>
                member is FieldInfo field &&
                !field.IsLiteral &&
                !field.IsNotSerialized &&
                field.FieldType.IsSerializable;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool IsSerializableProperty(MemberInfo member) =>
                member is PropertyInfo property &&
                property.CanWrite &&
                property.PropertyType.IsSerializable;
            #endregion
            #endregion
        }
        #endregion

        #region Public Members
        /// <summary>
        /// Singleton instance of this serializer.
        /// </summary>
        public static readonly ObjectBinaryDataSerializer instance =
            new ObjectBinaryDataSerializer();
        #endregion

        #region Methods & Functions
        public object? Deserialize(Type type, BinaryReader reader)
        {
            object? obj = null;

            if (IsExpectedType(type, out bool isDataContractPresent))
            {
                obj = CreateDefaultInstance(type);

                foreach (ObjectMember member in GetMembersToSerialize(type, isDataContractPresent))
                {
                    object? value = BinaryDataSerializer
                        .instance.Deserialize(member.MemberType, reader);

                    if (value != null)
                        member.SetValue(obj, value);
                }
            }

            return obj;
        }

        public bool IsExpectedType(Type? type) =>
            IsExpectedType(type, out _);

        public bool Serialize(object? obj, BinaryWriter writer)
        {
            Type? type = obj?.GetType();

            if (IsExpectedType(type, out bool isDataContractPresent))
            {
                foreach (ObjectMember member in GetMembersToSerialize(type!, isDataContractPresent))
                    BinaryDataSerializer.instance.Serialize(member.GetValue(obj!), writer);

                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsExpectedType(Type? type, out bool isDataContractPresent)
        {
            isDataContractPresent = type?.HasAttribute<DataContractAttribute>() ?? false;

            return ((type?.IsSerializable ?? false) || isDataContractPresent) &&
                !VALIDATE_SERIALIZERS.Any(e => e.IsExpectedType(type));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object CreateDefaultInstance(Type type)
        {
            // These functions doesn't invoke any constructor and works with types without default parameterless constructor:
#if NET8_0_OR_GREATER
            return System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(type);
#else
            return System.Runtime.Serialization.FormatterServices.GetUninitializedObject(type);
#endif
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IEnumerable<ObjectMember> GetMembersToSerialize(Type type, bool isDataContractPresent) =>
            (isDataContractPresent
                ? GetDataMembers(type)
                : GetSerializableMembers(type))
            .OrderBy(e => e.Name)
            .Select(ObjectMember.CreateFromMemberInfo);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IEnumerable<MemberInfo> GetSerializableMembers(Type type) =>
            type
                .GetMembers(BindingFlags.Instance | BindingFlags.Public)
                .Where(ObjectMember.IsSerializableMember);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IEnumerable<MemberInfo> GetDataMembers(Type type) =>
            type
                .GetMembers(BindingFlags.Instance)
                .Where(ObjectMember.IsDataMember);
        #endregion
    }
}
