using System;

namespace Minesweeper.GameServices.Utilities.Cloners
{
    public static class EnumArrayCloner
    {
        public static TEnum[] Clone<TEnum>(TEnum[] source)
            where TEnum : Enum
        {
            var result = new TEnum[source.Length];

            for (var i = 0; i < source.Length; ++i)
            {
                result[i] = source[i];
            }

            return result;
        }

        public static TEnum[][] Clone<TEnum>(TEnum[][] source)
            where TEnum : Enum
        {
            var result = new TEnum[source.Length][];

            for (var i = 0; i < source.Length; ++i)
            {
                result[i] = Clone(source[i]);
            }

            return result;
        }

        public static TDestEnum[] CloneAndMap<TSourceEnum, TDestEnum>(TSourceEnum[] source, Func<TSourceEnum, TDestEnum> converter)
             where TSourceEnum : Enum
             where TDestEnum : Enum
        {
            var result = new TDestEnum[source.Length];

            for (var i = 0; i < source.Length; ++i)
            {
                result[i] = converter(source[i]);
            }

            return result;
        }

        public static TDestEnum[][] CloneAndMap<TSourceEnum, TDestEnum>(TSourceEnum[][] source, Func<TSourceEnum, TDestEnum> converter)
            where TSourceEnum : Enum
            where TDestEnum : Enum
        {
            var result = new TDestEnum[source.Length][];

            for (var i = 0; i < source.Length; ++i)
            {
                result[i] = CloneAndMap(source[i], converter);
            }

            return result;
        }
    }
}