using System;
using UnityEngine;

namespace LucasWidget
{
    public interface IListViewItemData
    {

    }

    [Serializable]
    public class ListViewItemData : IListViewItemData
    {
        public int Number {  get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        private string _thumbUrl;

        public string ThumbUrl {
            get 
            {
                return _thumbUrl;
            }
            set
            {
                if (_thumbUrl != value)
                {
                    _thumbUrl = value;
                    SpriteLoader.Load(_thumbUrl, sprite => { Thumb = sprite; });
                }
            }
        }

        public Sprite Thumb { get; private set; }
    }
}
