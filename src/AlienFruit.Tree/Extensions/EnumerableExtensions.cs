using System;
using System.Collections.Generic;

namespace AlienFruit.Tree.Extensions
{
    public static class EnumerableExtensions
    {
        public static Tree<T> ToTree<T>(this IEnumerable<T> self, Func<T, IEnumerable<T>> childrenSelector)
            => new Tree<T>(self, childrenSelector);
    }
}
