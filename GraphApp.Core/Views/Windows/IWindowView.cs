using GraphApp.Core.ViewModels.Windows;
using GraphApp.Core.Views.Base;


namespace GraphApp.Core.Views.Windows;

public interface IWindowView<T> : IView<T>
    where T : IWindowViewModel?
{
    public void  Show();
    public bool? ShowDialog();
}