using System;
using System.Collections;
using System.Linq;
using UnhollowerBaseLib;
using I = Il2CppSystem.Collections.Generic;

namespace Laboratory.Extensions
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Convert an Il2Cpp List to a System List
        /// </summary>
        public static System.Collections.Generic.List<T> ToSystemList<T>(this I.List<T> iList)
        {
            return iList.ToArray().ToList();
        }
        
        /// <summary>
        /// Convert a System List to an Il2Cpp List
        /// </summary>
        public static I.List<T> ToIl2CppList<T>(this System.Collections.Generic.List<T> systemList)
        {
            I.List<T> iList = new();
            foreach (var item in systemList) iList.Add(item);
            return iList;
        }
        
        /// <summary>
        /// Shuffle a list with the fisher-yeats algorithm
        /// </summary>
        public static System.Collections.Generic.IList<T> Shuffle<T>(this System.Collections.Generic.IList<T> self)
        {
            var random = new System.Random();
            for (var i = self.Count - 1; i > 0; i--)
            {
                var j = random.Next(i + 1);
                (self[i], self[j]) = (self[j], self[i]);
            }
            return self;
        }

        /// <summary>
        /// Shuffle a list with the fisher-yeats algorithm with a given seed
        /// </summary>
        public static System.Collections.Generic.IList<T> SeededShuffle<T>(this System.Collections.Generic.IList<T> self, int seed)
        {
            var random = new System.Random(seed);
            for (var i = self.Count - 1; i > 0; i--)
            {
                var j = random.Next(i + 1);
                (self[i], self[j]) = (self[j], self[i]);
            }
            return self;
        }

        /// <summary>
        /// Get a random item from a list
        /// </summary>
        public static T? Random<T>(this System.Collections.Generic.IList<T> self)
        {
            return self.Count > 0 ? self[UnityEngine.Random.Range(0, self.Count)] : default(T);
        }
        
        /// <summary>
        /// Perform an action on every member of a list
        /// </summary>
        public static void ForEach<T>(this System.Collections.Generic.IList<T> self, Action<T> todo)
        {
            foreach (var t in self)
            {
                todo(t);
            }
        }
        
        /// <summary>
        /// Get all items in a collection which are of a particular il2cpptype
        /// </summary>
        public static System.Collections.Generic.IEnumerable<T> OfIl2CppType<T>(this IEnumerable source) where T : Il2CppObjectBase
        {
            foreach (var obj in source)
            {
                if (obj is Il2CppObjectBase il2cppObject)
                {
                    if (il2cppObject.TryCast<T>() is { } newObject)
                    {
                        yield return newObject;
                    }
                }
            }
        }
    }
}