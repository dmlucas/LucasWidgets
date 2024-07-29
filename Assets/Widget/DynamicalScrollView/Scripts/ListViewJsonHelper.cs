using System;
using UnityEngine;

namespace LucasWidget
{
    [Serializable]
    public class JsonWrapper<T>
    {
        public T[] array;
    }

    [Serializable]
    public static class DataJsonHelper
    {
        public static string ToJson(JsonWrapper<ListViewItemData> jsonWrapper)
        {
            return JsonUtility.ToJson(jsonWrapper);
        }

        public static ListViewItemData[] FromJson(string json)
        {
            json = "{\"array\": " + json + "}";
            JsonWrapper<ListViewItemData> _ = JsonUtility.FromJson<JsonWrapper<ListViewItemData>>(json);
            return _.array as ListViewItemData[];
        }
    }
}