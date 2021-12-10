using GraphApp.Core.Data;
using GraphApp.Core.Services;
using GraphApp.Core.ViewModels.Windows;
using GraphApp.Core.Views.Windows;

using Ninject;
using Ninject.Parameters;


namespace GraphApp.Core.Presentations;

internal class ComparableAlgorithmsPresenter : PresenterBase<IComparableAlgorithmsWindowView, IComparableAlgorithmsWindowViewModel>
{
    private readonly Dictionary<IGraphAlgorithm, GraphAlgorithmResults> m_DictionaryResults;


    public ComparableAlgorithmsPresenter(IBusinessLogic businessLogic, Dictionary<IGraphAlgorithm, GraphAlgorithmResults> dictionaryResults) : base(businessLogic)
    {
        m_DictionaryResults = dictionaryResults;

        ViewModel.DictionaryResults = m_DictionaryResults;
    }


    public void RunDialog()
    {
        View.ShowDialog();
    }
}