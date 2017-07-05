using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;

namespace Zane.LogHub.Server
{
    internal static class Extensions
    {
        internal static void Foreach<T>(this IEnumerable<T> enumeranle, Action<T> action)
        {
            foreach (T item in enumeranle)
            {
                action.Invoke(item);
            }
        }
    }
}
