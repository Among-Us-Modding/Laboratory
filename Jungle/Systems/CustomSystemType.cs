using System;
using System.Collections.Generic;
using System.Linq;

namespace Jungle.Systems;

public class CustomSystemType
{
#pragma warning disable CS0414
    private static int _lastId = -1;
#pragma warning restore CS0414

    private static readonly List<CustomSystemType> _list = new();

    public static IReadOnlyList<CustomSystemType> List => _list.AsReadOnly();

    public static CustomSystemType Register<T>() where T : ICustomSystemType
    {
        CustomSystemType customStringName = new CustomSystemType(_lastId--, typeof(T));
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
    public static explicit operator CustomSystemType(SystemTypes name) => List.SingleOrDefault(x => x.Id == (int)name);
}
