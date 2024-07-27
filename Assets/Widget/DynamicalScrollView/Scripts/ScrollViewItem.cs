using Newtonsoft.Json.Linq;
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

        private ScrollViewItemData ScrollViewItemData { get; set; }

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

        public void SetData(ScrollViewItemData scrollViewItemData)
        {
            ScrollViewItemData = scrollViewItemData;
            numberText.text = ScrollViewItemData.number.ToString();
            nameText.text = ScrollViewItemData.name;
            descriptionText.text = ScrollViewItemData.description;
        }
    }
}
