using System.Collections.Generic;
using System.Linq;

namespace Milou.Web.ClientResources
{
    internal static class EnumerableExtensions
    {
        public static IReadOnlyCollection<T> SafeToReadOnlyCollection<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                return new List<T>();
            }

            List<T> list = enumerable as List<T> ?? enumerable.ToList();

            return list;
        }
    }
}