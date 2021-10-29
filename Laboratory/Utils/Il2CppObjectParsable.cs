using Il2CppSystem;

namespace Laboratory.Utils
{
    public readonly struct Il2CppObjectParsable
    {
        public readonly Object Object;

        public Il2CppObjectParsable(Object obj) => Object = obj;

        public static implicit operator Object(Il2CppObjectParsable parsable) => parsable.Object;
        
        public static implicit operator Il2CppObjectParsable(short value) => new(new Int16 { m_value = value }.BoxIl2CppObject());
        public static implicit operator Il2CppObjectParsable(int value) => new(new Int32 { m_value = value }.BoxIl2CppObject());
        public static implicit operator Il2CppObjectParsable(long value) => new(new Int64 { m_value = value }.BoxIl2CppObject());

        public static implicit operator Il2CppObjectParsable(ushort value) => new(new UInt16 { m_value = value }.BoxIl2CppObject());
        public static implicit operator Il2CppObjectParsable(uint value) => new(new UInt32 { m_value = value }.BoxIl2CppObject());
        public static implicit operator Il2CppObjectParsable(ulong value) => new(new UInt64 { m_value = value }.BoxIl2CppObject());
        
        public static implicit operator Il2CppObjectParsable(float value) => new(new Single { m_value = value }.BoxIl2CppObject());
        public static implicit operator Il2CppObjectParsable(double value) => new(new Double { m_value = value }.BoxIl2CppObject());
        
        public static implicit operator Il2CppObjectParsable(string value) => new(value);
    }
}