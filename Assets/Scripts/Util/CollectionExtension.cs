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

    public static void ForEach<T>(this IList<T> _list, System.Action<T, int> _action)
    {
        for (int i = 0; i < _list.Count; ++i)
        {
            _action.Invoke(_list[i], i);
        }
    }

    public static bool IsNullOrEmpty(this string _str)
    {
        return string.IsNullOrEmpty(_str);
    }

    public static bool IsNullOrEmpty<T>(this IList<T> _list)
    {
        return _list == null || _list.Count == 0;
    }
}
