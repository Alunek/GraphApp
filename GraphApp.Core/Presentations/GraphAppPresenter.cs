using GraphApp.Core.Data;
using GraphApp.Core.Services;
using GraphApp.Core.ViewModels.Windows;
using GraphApp.Core.Views.Windows;


namespace GraphApp.Core.Presentations;

public class GraphAppPresenter : PresenterBase<IGraphAppWindowView, IGraphAppWindowViewModel>
{
    public GraphAppPresenter(IBusinessLogic businessLogic) : base(businessLogic)
    {
        ViewModel.ShowComparableAlgorithmsWindowEvent += ShowComparableAlgorithmsAlgorithmsWindowEventHandler;
    }

    private void ShowComparableAlgorithmsAlgorithmsWindowEventHandler(object? sender, Dictionary<IGraphAlgorithm, GraphAlgorithmResults> e)
    {
        var Presenter = new ComparableAlgorithmsPresenter(BusinessLogic, e);

        Presenter.RunDialog();
    }
}