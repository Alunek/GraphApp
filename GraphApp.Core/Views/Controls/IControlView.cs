using GraphApp.Core.ViewModels.Controls;
using GraphApp.Core.Views.Base;


namespace GraphApp.Core.Views.Controls;

public interface IControlView<T> : IView<T>
    where T : IControlViewModel?
{
}