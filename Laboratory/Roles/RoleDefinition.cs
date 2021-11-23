using System;

namespace Laboratory.Roles;

public class RoleDefinition
{
    public RoleDefinition(string name, bool isDefault, bool isImpostor, params Type[] components)
    {
        Name = name;
        Default = isDefault;
        Impostor = isImpostor;
        Components = components;
    }
    
    public readonly string? Name;
    public readonly bool Default;
    public readonly bool Impostor;
    public readonly Type[] Components;
}