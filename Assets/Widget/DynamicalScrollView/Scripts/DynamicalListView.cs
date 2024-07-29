using Newtonsoft.Json;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LucasWidget
{
    public class DynamicalListView : MonoBehaviour
    {
        [SerializeField]
        private ScrollRect scrollRect;

        [SerializeField]
        private ListViewItem listViewItemPrefab;

        [SerializeField]
        private float padding = 5;

        private IListView<ListViewItemData> listView;

        // Start is called before the first frame update
        private void Start()
        {
            StartCoroutine(InitListView());
        }

        private IEnumerator InitListView()
        {
            yield return new WaitForEndOfFrame();
            ListViewItemData[] itemDatas = GenerateDatas(30);

            // Test Serialize and Deserialize
            var json = JsonConvert.SerializeObject(itemDatas);
            var _itemDatas = JsonConvert.DeserializeObject<ListViewItemData[]>(json);

            listView = ListView.ListViewDataBuild(scrollRect, listViewItemPrefab, _itemDatas, padding);
        }

        public void SetListViewItemData50()
        {
            listView.UpdateData(GenerateDatas(50));
        }

        public void SetListViewItemData100()
        {
            listView.UpdateData(GenerateDatas(100));
        }

        public ListViewItemData[] GenerateDatas(int dataCount)
        {
            ListViewItemData[] items = new ListViewItemData[dataCount];
            for (var i = 0; i < items.Length; i++)
                items[i] = new() { Number = i + 1, Name = $"Button[{i + 1}]", Description = $"Description[{i + 1}]" };
            return items;
        }

        public class ListView : IListView<ListViewItemData>
        {
            public ScrollRect ScrollRect { get; set; }

            public ListViewItem ListViewItemPrefab { get; set; }

            public ListViewItemData[] ItemDatas { get; set; }

            public ListViewItem[] ViewItems { get; set; }

            protected float ContentHeight { get; set; }

            protected float ItemHeight { get; set; }

            protected float Padding { get; set; }

            public int DisplayCount { get; set; }

            protected int ExceedCount { get; set; }

            public UnityAction<int> onItemClicked;

            public ListView()
            {
                onItemClicked += OnItemClicked;
            }

            ~ListView()
            {
                onItemClicked -= OnItemClicked;
            }

            public void Init(ListViewItemData[] itemDatas)
            {
                ItemDatas = itemDatas;

                ViewItems ??= new ListViewItem[DisplayCount + 1];

                for (var i = 0; i < DisplayCount + 1; i++)
                {
                    var index = i;
                    if (ViewItems[index] == null)
                        ViewItems[index] = Instantiate(ListViewItemPrefab, ScrollRect.content);

                    ViewItems[index].RegisterButtonEvent(() => {
                        onItemClicked?.Invoke(ExceedCount + index);
                    });
                }

                UpdateItems();

                ScrollRect.onValueChanged.AddListener(v => UpdateItems());
            }

            public void UpdateData(ListViewItemData[] itemDatas)
            {
                ItemDatas = itemDatas;
                UpdateItems();
            }

            public void OnItemClicked(int index)
            {
                Debug.Log($"Item index: {index}");
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
                    var ListViewItem = ViewItems[i];
                    var rectTransform = ListViewItem.GetRectTransform();
                    var originPosY = i * (Padding + ItemHeight);
                    rectTransform.anchoredPosition = Vector2.up * -(offsetPosY + originPosY);

                    ViewItems[i].SetData(ItemDatas[ExceedCount + i]);
                }

                CalcContentAndItemHeight(out var contentHeight, out var itemHeight);
                var resetNormalizedPosition = contentHeight < ContentHeight;
                (ItemHeight, ContentHeight) = (itemHeight, contentHeight);
                SetContentHeight(ContentHeight);
                if (resetNormalizedPosition)
                    ScrollRect.verticalNormalizedPosition = 0;
            }

            public static ListView ListViewDataBuild(ScrollRect scrollRect, ListViewItem ListViewItemPrefab, ListViewItemData[] itemDatas, float padding)
            {
                var displayCount = Mathf.CeilToInt(scrollRect.viewport.rect.height / (ListViewItemPrefab.GetRectTransform().rect.height + padding));
                return ListViewDataBuild(scrollRect, ListViewItemPrefab, itemDatas, displayCount, padding);
            }

            public static ListView ListViewDataBuild(ScrollRect scrollRect, ListViewItem ListViewItemPrefab, ListViewItemData[] itemDatas, int displayCount, float padding)
            {
                ListView ListView = new()
                {
                    ScrollRect = scrollRect,
                    ListViewItemPrefab = ListViewItemPrefab,
                    ItemDatas = itemDatas,
                    DisplayCount = displayCount,
                    Padding = padding,
                };

                ListView.Init(ListView.ItemDatas);
                return ListView;
            }
        }
    }
}
