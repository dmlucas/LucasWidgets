using System;
using UnityEngine;

namespace LucasWidget
{
    [Serializable]
    public class ScrollViewItemDataJsonHelper
    {
        public static string ToJson<T>(T scrollViewItemData) where T : ScrollViewItemData
        {
            return JsonUtility.ToJson(scrollViewItemData);
        }

        public static T[] FromJson<T>(string json) where T : ScrollViewItemData
        {
            json = "{\"array\": " + json + "}";
            JsonWrapper _ = JsonUtility.FromJson<JsonWrapper>(json);
            return _.array as T[];
        }
    }

    [Serializable]
    public class JsonWrapper
    {
        public ScrollViewItemData[] array;
    }

    [Serializable]
    public class ScrollViewItemData
    {
        public int number;
        public string name;
        public string description;
    }
}
