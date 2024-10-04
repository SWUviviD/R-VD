using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CollectionExtension
{
    public static void ForEach<T>(this IList<T> _list, System.Action<T> _action)
    {
        foreach (var obj in _list)
        {
            _action.Invoke(obj);
        }
    }
}
