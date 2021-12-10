using GraphApp.Core.Services;
using GraphApp.Core.ViewModels.Base;

using Prism.Mvvm;


namespace GraphApp.WPF.ViewModels.Base;

internal abstract class ViewModelBase : BindableBase, IViewModel
{
    public IBusinessLogic BusinessLogic { get; }


    protected ViewModelBase(IBusinessLogic businessLogic)
    {
        BusinessLogic = businessLogic ?? throw new ArgumentNullException(nameof(businessLogic));
    }
}