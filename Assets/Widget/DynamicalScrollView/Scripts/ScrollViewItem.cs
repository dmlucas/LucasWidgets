using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LucasWidget
{
    public class ScrollViewItem : MonoBehaviour
    {
        public Text numberText;

        public Text nameText;

        public Text descriptionText;

        public Button itemButton;

        public void SetNumberText(string value)
        {
            numberText.text = value;
        }

        public void SetNameText(string value)
        {
            nameText.text = value;
        }

        public void SetDescriptionText(string value)
        {
            descriptionText.text = value;
        }

        public void RegisterButtonEvent(UnityAction unityAction)
        {
            itemButton.onClick.AddListener(() => {
                unityAction?.Invoke();
            });
        }

        public RectTransform GetRectTransform()
        {
            return gameObject.GetComponent<RectTransform>();
        }

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
                ScrollViewItemDataJsonWrapper scrollViewItemDataJsonWrapper =
                    JsonUtility.FromJson<ScrollViewItemDataJsonWrapper>(json);
                return scrollViewItemDataJsonWrapper.array as T[];
            }
        }

        [Serializable]
        public class ScrollViewItemDataJsonWrapper
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
}
