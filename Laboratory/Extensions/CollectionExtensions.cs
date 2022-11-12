using System;
using System.Collections;
using System.Collections.Generic;
using Il2CppInterop.Runtime.InteropTypes;
using I = Il2CppSystem.Collections.Generic;

namespace Laboratory.Extensions;

public static class CollectionExtensions
{
    /// <summary>
    /// Convert a System List to an Il2Cpp List
    /// </summary>
    public static I.List<T> ToIl2CppList<T>(this List<T> systemList)
    {
        I.List<T> iList = new();
        foreach (T item in systemList) iList.Add(item);
        return iList;
    }

    /// <summary>
    /// Shuffle a list with the fisher-yeats algorithm with Unity random
    /// </summary>
    public static void Shuffle<T>(this IList<T> self)
    {
        for (int i = self.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (self[i], self[j]) = (self[j], self[i]);
        }
    }

    /// <summary>
    /// Shuffle a list with the fisher-yeats algorithm with a given random
    /// </summary>
    public static void Shuffle<T>(this IList<T> self, Random random)
    {
        for (int i = self.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (self[i], self[j]) = (self[j], self[i]);
        }
    }

    /// <summary>
    /// Filters the elements of an <see cref="T:System.Collections.IEnumerable" /> based on a specified IL2CPP type.
    /// </summary>
    public static IEnumerable<T> OfIl2CppType<T>(this IEnumerable source) where T : Il2CppObjectBase
    {
        foreach (object? obj in source)
        {
            if (obj is Il2CppObjectBase il2CppObject)
            {
                if (il2CppObject.TryCast<T>() is { } newObject)
                {
                    yield return newObject;
                }
            }
        }
    }
}