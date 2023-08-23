namespace EnumerableDerivatives;

using EnumerableDerivatives.Implementations;
using EnumerableDerivatives.Interfaces;
using System.Collections.Generic;

public static class EnumerableExtensions
{
    public static IAnyAwareEnumerable<T> AsAnyAwareEnumerable<T>(this IEnumerable<T> enumerable)
    {
        return new AnyAwareEnumerable<T>(enumerable);
    }
}
