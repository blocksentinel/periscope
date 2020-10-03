﻿using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Cinder.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<TSource>> Batch<TSource>(this IEnumerable<TSource> source, int size)
        {
            TSource[] bucket = null;
            int count = 0;

            foreach (TSource item in source)
            {
                bucket ??= new TSource[size];
                bucket[count++] = item;

                if (count != size)
                {
                    continue;
                }

                yield return bucket;

                bucket = null;
                count = 0;
            }

            if (bucket != null && count > 0)
            {
                yield return bucket.Take(count);
            }
        }
    }
}
