using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exiled.API.Extensions
{
    public static class EnumExtensions
    {
        public static bool[] ToBoolArray<T>(this T @enum)
            where T : Enum
        {
            int[] values = (int[])Enum.GetValues(@enum.GetType());
            bool[] arr = new bool[values.Length + 1];

            for (var i = 0; i < arr.Length; i++)
                arr[i] = @enum.HasFlag((T)(object)(1 << i));

            return arr;
        }

        public static T ToFlagEnum<T>(this bool[] arr)
            where T : struct
        {
            if (typeof(T).BaseType != typeof(Enum))
                throw new TypeAccessException("This extension method is for flag enums");

            T @enum = default;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i])
                    @enum = (T)(object)((int)(object)@enum | (1 << i));
            }

            return @enum;
        }
    }
}
