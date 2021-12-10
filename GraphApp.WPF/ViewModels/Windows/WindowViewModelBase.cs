using GraphApp.Core.Services;
using GraphApp.Core.ViewModels.Windows;
using GraphApp.WPF.ViewModels.Base;


namespace GraphApp.WPF.ViewModels.Windows;

internal abstract class WindowViewModelBase : ViewModelBase, IWindowViewModel
{
    protected WindowViewModelBase(IBusinessLogic businessLogic) : base(businessLogic)
    {
    }
}