using System;

namespace ReMod.Core
{
    public static class EnumExtensions
    {
        public static int ToInt<T>(this T value) where T : Enum
        {
            return (int)(object)value;
        }

        public static bool HasFlag<T>(this T one, T other) where T : Enum
        {
            return (one.ToInt() & other.ToInt()) == other.ToInt();
        }

        public static T RemoveFlag<T>(this T one, T other) where T : Enum
        {
            return (T)Enum.ToObject(typeof(T), one.ToInt() & ~other.ToInt());
        }

        public static T AddFlag<T>(this T one, T other) where T : Enum
        {
            return (T)Enum.ToObject(typeof(T), one.ToInt() | other.ToInt());
        }
    }
}
