using GraphApp.Core.Services;
using GraphApp.Core.ViewModels.Windows;
using GraphApp.Core.Views.Windows;

using Ninject;
using Ninject.Parameters;
using Ninject.Syntax;


namespace GraphApp.Core.Presentations;

public abstract class PresenterBase<TView, TViewModel> : IPresenter
    where TView : IWindowView<TViewModel?>
    where TViewModel : IWindowViewModel
{
    protected readonly IBusinessLogic BusinessLogic;
    protected readonly TViewModel     ViewModel;
    protected readonly TView          View;

    protected IResolutionRoot IoC => BusinessLogic.IoC;


    protected PresenterBase(IBusinessLogic businessLogic)
    {
        BusinessLogic = businessLogic;

        ViewModel = IoC.Get<TViewModel>();
        View      = IoC.Get<TView>();

        View.ViewModel = ViewModel;
    }


    public virtual void Run()
    {
        View.Show();
    }


    public void Dispose()
    {
        View.Dispose();
    }
}