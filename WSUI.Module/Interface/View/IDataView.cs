
namespace WSUI.Module.Interface.View
{
    public interface IDataView<T> : IDataKindView
    {
        T Model
        {
            get;
            set;
        }
    }
}
