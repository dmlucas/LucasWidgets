namespace LucasWidget
{
    /// <summary>
    /// IListView
    /// </summary>
    /// <typeparam name="T">Data type</typeparam>
    public interface IListView<T> where T : IListViewItemData
    {
        void Init(T[] itemDatas);

        void UpdateData(T[] itemDatas);

        void OnItemClicked(int index);
    }
}