using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LucasWidget.ListView
{
    public class View : MonoBehaviour
    {
        public Text numberText;

        public Image thumbImage;

        public Text nameText;

        public Text descriptionText;

        public Button itemButton;

        private ViewModel viewModel { get; set; }

        public UnityAction<ViewModel> onClicked;

        private void Awake()
        {
            viewModel ??= new ViewModel(new Model());
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void Start()
        {
            itemButton.onClick.AddListener(() => {
                onClicked?.Invoke(viewModel);
            });
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ViewModel.Number):
                    numberText.text = $"{viewModel.Number}";
                    break;

                case nameof(ViewModel.Name):
                    nameText.text = $"{viewModel.Name}";
                    break;

                case nameof(ViewModel.Description):
                    descriptionText.text = $"{viewModel.Description}";
                    break;

                case nameof(ViewModel.ThumbUrl):
                    StartCoroutine(SpriteLoader.Load(viewModel.ThumbUrl, sprite => { viewModel.Thumb = sprite; }));
                    break;

                case nameof(ViewModel.Thumb):
                    thumbImage.overrideSprite = viewModel.Thumb;
                    break;
            }
        }

        public Vector2 Size 
        { 
            get { return gameObject.GetComponent<RectTransform>().sizeDelta; }
            set { gameObject.GetComponent<RectTransform>().sizeDelta = value; } 
        }

        public Vector2 Position
        {
            get { return gameObject.GetComponent<RectTransform>().anchoredPosition; }
            set { gameObject.GetComponent<RectTransform>().anchoredPosition = value; }
        }

        public void SetData(Model model)
        {
            viewModel ??= new ViewModel(new Model());
            viewModel.Number = model.Number;
            viewModel.Name = model.Name;
            viewModel.Description = model.Description;
            viewModel.ThumbUrl = model.ThumbUrl;
        }
    }
}
