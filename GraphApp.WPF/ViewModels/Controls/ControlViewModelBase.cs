using System.ComponentModel;

using GraphApp.Core.Services;
using GraphApp.Core.ViewModels.Controls;
using GraphApp.WPF.ViewModels.Base;


namespace GraphApp.WPF.ViewModels.Controls;

internal abstract class ControlViewModelBase : ViewModelBase, IControlViewModel
{
    protected ControlViewModelBase(IBusinessLogic businessLogic) : base(businessLogic)
    {
    }
}

internal abstract class ControlViewModelBase<T> : ViewModelBase, IControlViewModel
    where T : class, INotifyPropertyChanged
{
    protected readonly T Data;

    protected ControlViewModelBase(IBusinessLogic businessLogic, T data) : base(businessLogic)
    {
        Data                 =  data ?? throw new ArgumentNullException(nameof(data));
        Data.PropertyChanged += DataPropertyChanged;
    }


    protected virtual void DataPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        RaisePropertyChanged(e.PropertyName);
    }
}