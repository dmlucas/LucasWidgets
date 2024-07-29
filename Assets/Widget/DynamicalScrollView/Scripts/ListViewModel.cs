using System;
using System.ComponentModel;
using UnityEngine;

namespace LucasWidget.ListView
{
    public interface IModel
    {
        
    }

    public class Model : IModel
    {
        public int Number { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ThumbUrl { get; set; }
    }

    [Serializable]
    public class ViewModel : IModel, INotifyPropertyChanged
    {
        public Model model;

        public ViewModel(Model model)
        {
            this.model = model;
        }

        public int Number 
        { 
            get { return model.Number; } 
            set { model.Number = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Number))); } 
        }

        public string Name
        {
            get { return model.Name; }
            set { model.Name = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name))); }
        }

        public string Description
        {
            get { return model.Description; }
            set { model.Description = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Description))); }
        }

        public string ThumbUrl {
            get 
            {
                return model.ThumbUrl;
            }
            set
            {
                if (model.ThumbUrl != value)
                {
                    model.ThumbUrl = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ThumbUrl)));
                }
            }
        }

        private Sprite _thumb;

        public Sprite Thumb
        { 
            get { return _thumb; } 
            set { _thumb = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Thumb))); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
