using GraphApp.Core.ViewModels.Base;


namespace GraphApp.Core.Views.Base;

public interface IView<T> : IDisposable
    where T : IViewModel?
{
    public T ViewModel { get; set; }
}