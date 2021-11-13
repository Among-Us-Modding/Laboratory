using System;
using System.Collections.Generic;
using System.Linq;
using Laboratory.Systems;

namespace Laboratory.CustomMap;

public class CustomSystemType
{
    private static int _lastId = -1;

    private static readonly List<CustomSystemType> _list = new();

    public static IReadOnlyList<CustomSystemType> List => _list.AsReadOnly();

    public static CustomSystemType Register<T>() where T : ICustomSystemType
    {
        var customStringName = new CustomSystemType(_lastId--, typeof(T));
        _list.Add(customStringName);
        SystemTypeHelpers.AllTypes = SystemTypeHelpers.AllTypes.Append(customStringName).ToArray();

        return customStringName;
    }

    public int Id { get; }

    public Type Value { get; }

    private CustomSystemType(int id, Type value)
    {
        Id = id;
        Value = value;
    }

    public static implicit operator SystemTypes(CustomSystemType name) => (SystemTypes)name.Id;
    public static explicit operator CustomSystemType?(SystemTypes name) => List.SingleOrDefault(x => x.Id == (int)name);
}
