namespace LucasWidget
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">Data type</typeparam>
    public interface IScrollView<T>
    {
        void Init(T[] itemDatas);
        void UpdateData(T[] itemDatas);
        void UpdateItems();
        void OnItemClicked(int index);
    }
}