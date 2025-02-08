# Argos Framework BinarySerializer
A pure binary serializer for .NET

> [!NOTE]
> This project is in development. This readme is a first draft.

The goal of this serializer is to serialize an object and his data in a pure binary form, only the binary representation of each data, without stored metadata or references to the source type.
Currently can serialize and deserialize any type of object and type. To deserialize them you need to know the original data structure.

The serializer by default serializes any public serializable member but you can use the **DataContract** attributes from [.NET Data Contract Serializer](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.datacontractserializer?view=net-9.0) to setup what classes and members will be serialized.

Built-in data serialization support:
- Primitives (bool, byte, sbyte, short, ushort, int, uint, long, ulong, float, double, char)
- String
- Decimal
- DateTime
- TimeSpan
- Guid
- Typed enums (byte, sbyte, short, ushort, int, uint, long, ulong)
- Arrays of one dimension
- IEnumerables (List<T>, IList, IDictionary)
- Any serializble object

TODO:
- [ ] Add unit tests.
- [ ] Control of recursive serialization (a max deep level).
- [ ] Add support for custom pipeline serialization types (to customize the serialization behavior of custom types)
- [ ] Add option to support serialization of null values or throw an exception instead.
- [ ] Possible support for AOT.
