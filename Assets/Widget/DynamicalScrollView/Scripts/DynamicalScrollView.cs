using System;
using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
        private float padding = 5;

        private ScrollView scrollView;

        // Start is called before the first frame update
        private void Start()
        {
            StartCoroutine(InitScrollView());
        }

        private IEnumerator InitScrollView()
        {
            yield return new WaitForEndOfFrame();
            ScrollViewItemData[] items = new ScrollViewItemData[dataCount];
            for (var i = 0; i < items.Length; i++)
                items[i] = new() { number = i + 1, name = $"Button[{i + 1}]", description = $"Description[{i + 1}]" };

            scrollView = ScrollView.ScrollViewDataBuild(scrollRect, scrollViewItemPrefab, items, padding);
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

        public class ScrollView : IScrollView<ScrollViewItemData>
        {
            public ScrollRect ScrollRect { get; set; }

            public ScrollViewItem ScrollViewItemPrefab { get; set; }

            public ScrollViewItemData[] ItemDatas { get; set; }

            public ScrollViewItem[] ScrollViewItems { get; set; }

            protected float ContentHeight { get; set; }

            protected float ItemHeight { get; set; }

            protected float Padding { get; set; }

            public int DisplayCount { get; set; }

            protected int ExceedCount { get; set; }

            public UnityAction<int> onItemClicked;

            public ScrollView()
            {
                onItemClicked += OnItemClicked;
            }

            ~ScrollView()
            {
                onItemClicked -= OnItemClicked;
            }

            public void Init(ScrollViewItemData[] itemDatas)
            {
                ItemDatas = itemDatas;

                ScrollViewItems ??= new ScrollViewItem[DisplayCount + 1];

                for (var i = 0; i < DisplayCount + 1; i++)
                {
                    var index = i;
                    if (ScrollViewItems[index] == null)
                        ScrollViewItems[index] = Instantiate(ScrollViewItemPrefab, ScrollRect.content);

                    ScrollViewItems[index].RegisterButtonEvent(() => {
                        onItemClicked?.Invoke(ExceedCount + index);
                    });
                }

                UpdateItems();

                ScrollRect.onValueChanged.AddListener(v => UpdateItems());
            }

            public void UpdateData(ScrollViewItemData[] itemDatas)
            {
                ItemDatas = itemDatas;
                UpdateItems();
            }

            private void CalcContentAndItemHeight(out float contentHeight, out float itemHeight)
            {
                var viewportHeight = ScrollRect.viewport.rect.height;
                itemHeight = (viewportHeight - (DisplayCount - 1) * Padding) / (DisplayCount);
                contentHeight = (ItemDatas.Length - 1) * Padding + ItemDatas.Length * itemHeight;
            }

            private void SetContentHeight(float contentHeight)
            {
                var contentSize = ScrollRect.content.sizeDelta;
                ScrollRect.content.sizeDelta = new Vector2(contentSize.x, contentHeight);
            }

            private void UpdateItems()
            {
                var contentPos = ScrollRect.content.anchoredPosition;
                ExceedCount = Mathf.CeilToInt((contentPos.y - ItemHeight) / (ItemHeight + Padding));
                ExceedCount = Mathf.Clamp(ExceedCount, 0, ItemDatas.Length - (DisplayCount + 1));
                var offsetPosY = (ItemHeight + Padding) * ExceedCount;
                for (int i = 0; i < DisplayCount + 1; i++)
                {
                    var scrollViewItem = ScrollViewItems[i];
                    var rectTransform = scrollViewItem.GetRectTransform();
                    var originPosY = i * (Padding + ItemHeight);
                    rectTransform.anchoredPosition = Vector2.up * -(offsetPosY + originPosY);

                    ScrollViewItems[i].SetData(ItemDatas[ExceedCount + i]);
                }

                CalcContentAndItemHeight(out var contentHeight, out var itemHeight);
                var resetNormalizedPosition = contentHeight < ContentHeight;
                (ItemHeight, ContentHeight) = (itemHeight, contentHeight);
                SetContentHeight(ContentHeight);
                if (resetNormalizedPosition)
                    ScrollRect.verticalNormalizedPosition = 0;
            }

            public void OnItemClicked(int index)
            {
                Debug.Log($"Item index: {index}");
            }

            public static ScrollView ScrollViewDataBuild(ScrollRect scrollRect, ScrollViewItem scrollViewItemPrefab, ScrollViewItemData[] itemDatas, float padding)
            {
                var displayCount = Mathf.CeilToInt(scrollRect.viewport.rect.height / (scrollViewItemPrefab.GetRectTransform().rect.height + padding));
                return ScrollViewDataBuild(scrollRect, scrollViewItemPrefab, itemDatas, displayCount, padding);
            }

            public static ScrollView ScrollViewDataBuild(ScrollRect scrollRect, ScrollViewItem scrollViewItemPrefab, ScrollViewItemData[] itemDatas, int displayCount, float padding)
            {
                ScrollView scrollView = new()
                {
                    ScrollRect = scrollRect,
                    ScrollViewItemPrefab = scrollViewItemPrefab,
                    ItemDatas = itemDatas,
                    DisplayCount = displayCount,
                    Padding = padding,
                };

                scrollView.Init(scrollView.ItemDatas);
                return scrollView;
            }
        }
    }
}
