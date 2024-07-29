using Newtonsoft.Json;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
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
            Model[] models = GenerateDatas(30);

            // Test Serialize and Deserialize
            Model[] _models = new Model[models.Length];
            for (int i = 0; i < models.Length; i++)
            {
                var json = JsonConvert.SerializeObject(models[i]);
                _models[i] = JsonConvert.DeserializeObject<Model>(json);
            }

            listView = ListView.ListViewBuild(scrollRect, viewPrefab, models, padding);
        }

        public void SetListViewItemData50()
        {
            listView.UpdateData(GenerateDatas(50));
        }

        public void SetListViewItemData100()
        {
            listView.UpdateData(GenerateDatas(100));
        }

        public Model[] GenerateDatas(int dataCount)
        {
            Model[] models = new Model[dataCount];
            for (var i = 0; i < models.Length; i++)
                models[i] = new() { Number = i + 1, Name = $"Button[{i + 1}]", Description = $"Description[{i + 1}]", ThumbUrl = "https://www.imatest.com/wp-content/uploads/2020/06/LabReflectiveModule-3-2-RESIZED.jpg" };
            return models;
        }

        public class ListView : IListView<Model>
        {
            public ScrollRect ScrollRect { get; set; }

            public View ViewPrefab { get; set; }

            public View[] Views { get; set; }

            protected Model[] Models { get; set; }

            protected float ContentHeight { get; set; }

            protected float ItemHeight { get; set; }

            protected float Padding { get; set; }

            public int DisplayCount { get; set; }

            protected int ExceedCount { get; set; }

            public UnityAction<Model> _onItemClicked;

            public UnityAction<Model> onItemClicked { get => _onItemClicked; set { _onItemClicked = value; } }

            public void Init(Model[] models)
            {
                Views ??= new View[DisplayCount + 1];
                for (var i = 0; i < DisplayCount + 1; i++)
                {
                    var index = i;
                    if (Views[index] == null)
                    {
                        Views[index] = Instantiate(ViewPrefab, ScrollRect.content);
                        Views[index].onClicked += _ => { onItemClicked?.Invoke(_.model); };
                    }
                }

                UpdateData(models);
                ScrollRect.onValueChanged.AddListener(v => UpdateItems());
            }

            public void UpdateData(Model[] models)
            {
                Models = models;
                UpdateItems();
            }

            private void CalcContentAndItemHeight(out float contentHeight, out float itemHeight)
            {
                var viewportHeight = ScrollRect.viewport.rect.height;
                itemHeight = (viewportHeight - (DisplayCount - 1) * Padding) / (DisplayCount);
                contentHeight = (Models.Length - 1) * Padding + Models.Length * itemHeight;
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
                ExceedCount = Mathf.Clamp(ExceedCount, 0, Models.Length - (DisplayCount + 1));
                var offsetPosY = (ItemHeight + Padding) * ExceedCount;
                for (int i = 0; i < DisplayCount + 1; i++)
                {
                    var ListViewItem = Views[i];
                    var originPosY = i * (Padding + ItemHeight);
                    ListViewItem.Position = Vector2.up * -(offsetPosY + originPosY);
                    Views[i].SetData(Models[ExceedCount + i]);
                }

                CalcContentAndItemHeight(out var contentHeight, out var itemHeight);
                var resetNormalizedPosition = contentHeight < ContentHeight;
                (ItemHeight, ContentHeight) = (itemHeight, contentHeight);
                SetContentHeight(ContentHeight);
                if (resetNormalizedPosition)
                    ScrollRect.verticalNormalizedPosition = 0;
            }

            public static ListView ListViewBuild(ScrollRect scrollRect, View ViewPrefab, Model[] models, float padding)
            {
                var displayCount = Mathf.CeilToInt(scrollRect.viewport.rect.height / (ViewPrefab.Size.y + padding));
                return ListViewBuild(scrollRect, ViewPrefab, models, displayCount, padding);
            }

            public static ListView ListViewBuild(ScrollRect scrollRect, View ViewPrefab, Model[] models, int displayCount, float padding)
            {
                ListView ListView = new()
                {
                    ScrollRect = scrollRect,
                    ViewPrefab = ViewPrefab,
                    Models = models,
                    DisplayCount = displayCount,
                    Padding = padding,
                };

                ListView.Init(ListView.Models);
                return ListView;
            }
        }
    }
}
