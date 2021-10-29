using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Il2CppSystem;

namespace Laboratory.Utils
{
    /// <summary>
    /// Equality comparer for comparison of Il2Cpp Objects
    /// which can allow Il2Cpp Objects to be used as keys in Dictionaries
    /// </summary>
    /// <typeparam name="T">Type being compared</typeparam>
    public class Il2CppEqualityComparer<T> : IEqualityComparer<T> where T : Object
    {
        private static Il2CppEqualityComparer<T> _instance;

        public static Il2CppEqualityComparer<T> Instance => _instance ??= new Il2CppEqualityComparer<T>();
        
        private Il2CppEqualityComparer() { }
        
        public int GetHashCode(T value) => RuntimeHelpers.GetHashCode(value);

        public bool Equals(T left, T right) => left == null || right == null ? left == null && right == null : left.Equals(right);
    }
}