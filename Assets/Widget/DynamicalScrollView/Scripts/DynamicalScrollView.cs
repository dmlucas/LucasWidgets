using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static LucasWidget.ScrollViewItem;

namespace LucasWidget
{
    public class DynamicalScrollView : MonoBehaviour
    {
        [SerializeField]
        private ScrollRect scrollRect;

        [SerializeField]
        private ScrollViewItem scrollViewItemPrefab;

        [SerializeField]
        private int dataCount = 100;

        [SerializeField]
        private int displayCount = 8;

        [SerializeField]
        private float padding = 10;

        private ScrollView scrollView;

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(InitScrollView());
        }

        private IEnumerator InitScrollView()
        {
            yield return new WaitForEndOfFrame();
            ScrollViewItemData[] items = new ScrollViewItemData[dataCount];
            for (var i = 0; i < items.Length; i++)
            {
                items[i] = new() { description = $"Button[{i + 1}]" };
            }
            scrollView = ScrollView.ScrollViewDataBuild(scrollRect, scrollViewItemPrefab, items, displayCount, padding);
        }

        public void SetScrollViewItemData50()
        {
            ScrollViewItemData[] items = new ScrollViewItemData[dataCount / 2];
            for (var i = 0; i < items.Length; i++)
                items[i] = new() { number = i + 1, name = $"Button[{i + 1}]", description = $"Description[{i + 1}]" };

            scrollView.UpdateData(items);
        }

        public void SetScrollViewItemData100()
        {
            ScrollViewItemData[] items = new ScrollViewItemData[dataCount];
            for (var i = 0; i < items.Length; i++)
                items[i] = new() { number = i + 1, name = $"Button[{i + 1}]", description = $"Description[{i + 1}]" };

            scrollView.UpdateData(items);
        }


        private interface IScrollView
        {
            void UpdateData(ScrollViewItemData[] itemDatas);
            (float, float) CalcContentAndItemHeight();
            void SetContentHeight(float contentHeight);
            void UpdateItems();
            void OnItemClicked(int index);
        }

        [Serializable]
        private class ScrollView : IScrollView
        {
            public ScrollRect scrollRect;
            public ScrollViewItem scrollViewItemPrefab;
            public ScrollViewItemData[] itemDatas;
            public ScrollViewItem[] scrollViewItems;
            private float contentHeight;
            private float itemHeight;
            private float padding;
            public int displayCount;
            private int exceedCount;

            public UnityAction<int> onItemClicked;

            public ScrollView()
            {
                onItemClicked += OnItemClicked;
            }

            ~ScrollView()
            {
                onItemClicked -= OnItemClicked;
            }

            public void UpdateData(ScrollViewItemData[] itemDatas)
            {
                this.itemDatas = itemDatas;
                scrollViewItems ??= new ScrollViewItem[displayCount + 1];

                (var contentHeight, var itemHeight) = CalcContentAndItemHeight();

                var resetNormalizedPosition = contentHeight < scrollRect.content.sizeDelta.y && exceedCount + displayCount >= itemDatas.Length;

                (this.itemHeight, this.contentHeight) = (itemHeight, contentHeight);

                SetContentHeight(this.contentHeight);

                if (resetNormalizedPosition)
                    scrollRect.verticalNormalizedPosition = 0;

                for (var i = 0; i < displayCount + 1; i++)
                {
                    var index = i;
                    if (scrollViewItems[index] == null)
                        scrollViewItems[index] = Instantiate(scrollViewItemPrefab, scrollRect.content);

                    var rectTransform = scrollViewItems[index].GetRectTransform();
                    var itemSize = rectTransform.sizeDelta;
                    rectTransform.sizeDelta = new Vector2(itemSize.x, itemHeight);
                    var targetPosY = i * (padding + itemHeight);
                    var itemPos = rectTransform.anchoredPosition;
                    rectTransform.anchoredPosition = new Vector2(itemPos.x, -targetPosY);

                    scrollViewItems[index].SetNumberText($"{exceedCount + i + 1}");
                    scrollViewItems[index].SetNameText($"Button[{exceedCount + i + 1}]");
                    scrollViewItems[index].SetDescriptionText($"Description[{exceedCount + i + 1}]");
                    scrollViewItems[index].RegisterButtonEvent(() => {
                        onItemClicked?.Invoke(exceedCount + index);
                    });
                }
                UpdateItems();
                scrollRect.onValueChanged.AddListener(v => UpdateItems());
            }

            public (float, float) CalcContentAndItemHeight()
            {
                var viewportHeight = scrollRect.viewport.rect.height;
                var itemHeight = (viewportHeight - (displayCount - 1) * padding) / (displayCount);
                var contentHeight = (itemDatas.Length - 1) * padding + itemDatas.Length * itemHeight;
                return (contentHeight, itemHeight);
            }

            public void SetContentHeight(float contentHeight)
            {
                var contentSize = scrollRect.content.sizeDelta;
                scrollRect.content.sizeDelta = new Vector2(contentSize.x, contentHeight);
            }

            public void UpdateItems()
            {
                var contentPos = scrollRect.content.anchoredPosition;
                exceedCount = Mathf.CeilToInt((contentPos.y - itemHeight) / (itemHeight + padding));
                exceedCount = Mathf.Clamp(exceedCount, 0, itemDatas.Length - (displayCount + 1));
                var offsetPosY = (itemHeight + padding) * exceedCount;
                for (int i = 0; i < displayCount + 1; i++)
                {
                    var scrollViewItem = scrollViewItems[i];
                    var rectTransform = scrollViewItem.GetRectTransform();
                    var originPosY = i * (padding + itemHeight);
                    rectTransform.anchoredPosition = Vector2.up * -(offsetPosY + originPosY);
                    scrollViewItem.GetComponentInChildren<Text>().text = $"Button[{exceedCount + i + 1}]";

                    scrollViewItem.SetNumberText($"{exceedCount + i + 1}");
                    scrollViewItem.SetNameText($"Button[{exceedCount + i + 1}]");
                    scrollViewItem.SetDescriptionText($"Description[{exceedCount + i + 1}]");
                }
            }

            public void OnItemClicked(int index)
            {
                Debug.Log($"Current index: {index}");
            }

            public static ScrollView ScrollViewDataBuild(ScrollRect scrollRect, ScrollViewItem scrollViewItemPrefab, ScrollViewItemData[] itemDatas, int displayCount, float padding)
            {
                ScrollView scrollView = new()
                {
                    scrollRect = scrollRect,
                    scrollViewItemPrefab = scrollViewItemPrefab,
                    itemDatas = itemDatas,
                    displayCount = displayCount,
                    padding = padding,
                };
                scrollView.UpdateData(scrollView.itemDatas);
                return scrollView;
            }
        }
    }
}
