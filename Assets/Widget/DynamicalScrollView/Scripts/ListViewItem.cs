using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LucasWidget
{
    public class ListViewItem : MonoBehaviour
    {
        public Text numberText;

        public Image thumbImage;

        public Text nameText;

        public Text descriptionText;

        public Button itemButton;

        private ListViewItemData ItemData { get; set; }

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

        public void SetData(ListViewItemData itemData)
        {
            ItemData = itemData;
            numberText.text = ItemData.Number.ToString();

            if (ItemData.Thumb)
                thumbImage.overrideSprite = ItemData.Thumb;

            nameText.text = ItemData.Name;
            descriptionText.text = ItemData.Description;
        }
    }
}
