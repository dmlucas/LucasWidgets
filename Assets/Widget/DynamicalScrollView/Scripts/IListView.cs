using UnityEngine.Events;

namespace LucasWidget.ListView
{
    /// <summary>
    /// IListView
    /// </summary>
    /// <typeparam name="T">Data type</typeparam>
    public interface IListView<T> where T : IModel
    {
        void Init(T[] models);

        void UpdateData(T[] models);

        UnityAction<T> onItemClicked { get; set; }
    }
}