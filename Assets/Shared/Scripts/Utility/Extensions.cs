using System.Collections.Generic;

namespace Shared.Utility
{
    public static class Extensions
    {
        public static string Flatten<T>(this IEnumerable<T> source)
        {
            return $"[{string.Join(", ", source)}]";
        }
    }
}