using System.ComponentModel;

using GraphApp.Core.Services;


namespace GraphApp.Core.ViewModels.Base;

public interface IViewModel : INotifyPropertyChanged
{
    IBusinessLogic BusinessLogic { get; }
}