using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class EnumerableExtensions
{
    /// <summary>
    /// Executes a for each on any ICollection, not just Lists
    /// </summary>
    public static void ForEach<T>(this IEnumerable<T> coll, Action<T> action)
    {
        foreach (var c in coll.ToList())
        {
            action(c);
        }
    }

    /// <summary>
    /// Executes an action in Parallel against any Enumerable
    /// </summary>
    public static void ParallelForEach<T>(this IEnumerable<T> coll, Action<T> action)
    {
        coll.ToList().AsParallel().ForAll(p => action(p));
    }

    /// <summary>
    /// Performs a transformation on a set on one type into a set of another type in Parallel
    /// </summary>
    public static ICollection<TOut> ParallelTransform<TIn, TOut>(this IEnumerable<TIn> coll, Func<TIn, TOut> action)
    {
        var res = new HashSet<TOut>();
        coll.ToList().AsParallel().ForAll(p => res.Add(action(p)));
        return res;
    }
}

