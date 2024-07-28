using Newtonsoft.Json;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace LucasWidget.ListView
{
    public class DynamicalListView : MonoBehaviour
    {
        [SerializeField]
        private ScrollRect scrollRect;

        [SerializeField]
        private View viewPrefab;

        [SerializeField]
        private float padding = 5;

        private ListView listView;

        // Start is called before the first frame update
        private void Start()
        {
            StartCoroutine(InitListView());
        }

        private IEnumerator InitListView()
        {
            yield return new WaitForEndOfFrame();
            ViewModel[] viewModels = GenerateDatas(30);

            // Test Serialize and Deserialize
            //ViewModel[] _viewModels = new ViewModel[viewModels.Length];
            //for (int i = 0; i < viewModels.Length; i++)
            //{
            //    var json = JsonConvert.SerializeObject(viewModels[i].model);
            //    var model = JsonConvert.DeserializeObject<Model>(json);
            //    _viewModels[i] = new ViewModel(model);
            //}

            listView = ListView.ListViewDataBuild(scrollRect, viewPrefab, viewModels, padding);
        }

        public void SetListViewItemData50()
        {
            listView.UpdateData(GenerateDatas(50));
        }

        public void SetListViewItemData100()
        {
            listView.UpdateData(GenerateDatas(100));
        }

        public ViewModel[] GenerateDatas(int dataCount)
        {
            ViewModel[] items = new ViewModel[dataCount];
            for (var i = 0; i < items.Length; i++)
                items[i] = new ViewModel(new() { Number = i + 1, Name = $"Button[{i + 1}]", Description = $"Description[{i + 1}]" });
            return items;
        }

        public class ListView
        {
            public ScrollRect ScrollRect { get; set; }

            public View ViewPrefab { get; set; }

            public View[] Views { get; set; }

            public ViewModel[] ViewModels { get; set; }

            protected float ContentHeight { get; set; }

            protected float ItemHeight { get; set; }

            protected float Padding { get; set; }

            public int DisplayCount { get; set; }

            protected int ExceedCount { get; set; }

            private void Init(ViewModel[] viewModels)
            {
                ViewModels = viewModels;

                Views ??= new View[DisplayCount + 1];
                for (var i = 0; i < DisplayCount + 1; i++)
                {
                    var index = i;
                    if (Views[index] == null)
                        Views[index] = Instantiate(ViewPrefab, ScrollRect.content);
                }

                UpdateItems();

                ScrollRect.onValueChanged.AddListener(v => UpdateItems());
            }

            public void UpdateData(ViewModel[] viewModels)
            {
                ViewModels = viewModels;
                UpdateItems();
            }

            private void CalcContentAndItemHeight(out float contentHeight, out float itemHeight)
            {
                var viewportHeight = ScrollRect.viewport.rect.height;
                itemHeight = (viewportHeight - (DisplayCount - 1) * Padding) / (DisplayCount);
                contentHeight = (ViewModels.Length - 1) * Padding + ViewModels.Length * itemHeight;
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
                ExceedCount = Mathf.Clamp(ExceedCount, 0, ViewModels.Length - (DisplayCount + 1));
                var offsetPosY = (ItemHeight + Padding) * ExceedCount;
                for (int i = 0; i < DisplayCount + 1; i++)
                {
                    var ListViewItem = Views[i];
                    var originPosY = i * (Padding + ItemHeight);
                    ListViewItem.Position = Vector2.up * -(offsetPosY + originPosY);
                    Views[i].SetData(ViewModels[ExceedCount + i]);
                }

                CalcContentAndItemHeight(out var contentHeight, out var itemHeight);
                var resetNormalizedPosition = contentHeight < ContentHeight;
                (ItemHeight, ContentHeight) = (itemHeight, contentHeight);
                SetContentHeight(ContentHeight);
                if (resetNormalizedPosition)
                    ScrollRect.verticalNormalizedPosition = 0;
            }

            public static ListView ListViewDataBuild(ScrollRect scrollRect, View ViewPrefab, ViewModel[] viewModels, float padding)
            {
                var displayCount = Mathf.CeilToInt(scrollRect.viewport.rect.height / (ViewPrefab.Size.y + padding));
                return ListViewDataBuild(scrollRect, ViewPrefab, viewModels, displayCount, padding);
            }

            public static ListView ListViewDataBuild(ScrollRect scrollRect, View ViewPrefab, ViewModel[] viewModels, int displayCount, float padding)
            {
                ListView ListView = new()
                {
                    ScrollRect = scrollRect,
                    ViewPrefab = ViewPrefab,
                    ViewModels = viewModels,
                    DisplayCount = displayCount,
                    Padding = padding,
                };

                ListView.Init(ListView.ViewModels);
                return ListView;
            }
        }
    }
}
