using System;

namespace AosP2PClient.Common.PriorityQueue
{
    public static partial class Extensions
    {
        public static T[] RemoveAt<T>(this T[] source, int index)
        {
            T[] dest = new T[source.Length - 1];
            if (index > 0)
                Array.Copy(source, 0, dest, 0, index);

            if (index < source.Length - 1)
                Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

            return dest;
        }

        public static T[] AddAt<T>(this T[] source, int index, T element)
        {
            T[] dest = new T[source.Length + 1];
            if (index > 0)
                Array.Copy(source, 0, dest, 0, index);

            if (index < source.Length)
                Array.Copy(source, index, dest, index + 1, source.Length - index);

            dest[index] = element;

            return dest;
        }
    }
}
