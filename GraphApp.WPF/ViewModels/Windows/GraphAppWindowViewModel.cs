using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

using GraphApp.Core.Data;
using GraphApp.Core.Helper;
using GraphApp.Core.Models;
using GraphApp.Core.Models.Graphs;
using GraphApp.Core.Models.Matrix;
using GraphApp.Core.Services;
using GraphApp.Core.ViewModels.Controls;
using GraphApp.Core.ViewModels.Windows;
using GraphApp.Core.Views.Controls;

using Ninject;
using Ninject.Parameters;

using Prism.Commands;


namespace GraphApp.WPF.ViewModels.Windows;

internal class GraphAppWindowViewModel : WindowViewModelBase, IGraphAppWindowViewModel
{
    private readonly Graph                                 m_Graph;
    private readonly MatrixTable                           m_MatrixTable;
    private readonly ObservableCollection<IGraphAlgorithm> m_GraphAlgorithmsCollection;

    private readonly DelegateCommand m_FindPathBetweenPairVertexesCommand;
    private readonly DelegateCommand m_CompareGraphAlgorithmCommand;

    private bool m_IsActiveOperation;


    private bool IsActiveOperation
    {
        get => m_IsActiveOperation;
        set
        {
            m_IsActiveOperation = value;

            m_FindPathBetweenPairVertexesCommand.RaiseCanExecuteChanged();
            m_CompareGraphAlgorithmCommand.RaiseCanExecuteChanged();
        }
    }


    public ReadOnlyObservableCollection<IGraphAlgorithm> GraphAlgorithmsCollection { get; }
    public IGraphAlgorithm?                              SelectedGraphAlgorithm    { get; set; }

    public IGraphControlViewModel      GraphViewModel        { get; }
    public IMatrixPageControlViewModel MatrixPageViewModel   { get; }
    public IGraphControlView           GraphControlView      { get; }
    public IMatrixPageControlView      MatrixPageControlView { get; }

    public ICommand LoadGraphCommand                   { get; }
    public ICommand SaveGraphCommand                   { get; }
    public ICommand ExitCommand                        { get; }
    public ICommand FindPathBetweenPairVertexesCommand => m_FindPathBetweenPairVertexesCommand;
    public ICommand CompareGraphAlgorithmCommand       => m_CompareGraphAlgorithmCommand;

    public string FoundPathString { get; private set; }



    public event EventHandler<Dictionary<IGraphAlgorithm, GraphAlgorithmResults>> ShowComparableAlgorithmsWindowEvent;


    public GraphAppWindowViewModel(IBusinessLogic businessLogic) : base(businessLogic)
    {
        m_Graph       = new Graph();
        m_MatrixTable = new MatrixTable(m_Graph);

        m_GraphAlgorithmsCollection = new();
        GraphAlgorithmsCollection   = new(m_GraphAlgorithmsCollection);

        m_GraphAlgorithmsCollection.Add(BusinessLogic.IoC.Get<IGraphAlgorithmDijkstraLogic>());
        m_GraphAlgorithmsCollection.Add(BusinessLogic.IoC.Get<IGraphAlgorithmFloydLogic>());
        SelectedGraphAlgorithm = m_GraphAlgorithmsCollection.FirstOrDefault((IGraphAlgorithm?)null);

        var GraphParameter = new ConstructorArgument("graph", m_Graph);
        GraphViewModel = BusinessLogic.IoC.Get<IGraphControlViewModel>(GraphParameter);
        var MatrixTableParameter = new ConstructorArgument("matrixTable", m_MatrixTable);
        MatrixPageViewModel = BusinessLogic.IoC.Get<IMatrixPageControlViewModel>(MatrixTableParameter);

        GraphControlView      = BusinessLogic.IoC.Get<IGraphControlView>();
        MatrixPageControlView = BusinessLogic.IoC.Get<IMatrixPageControlView>();

        GraphControlView.ViewModel      = GraphViewModel;
        MatrixPageControlView.ViewModel = MatrixPageViewModel;

        LoadGraphCommand                     = new DelegateCommand<string>(LoadGraphCommandHandler);
        SaveGraphCommand                     = new DelegateCommand<string>(SaveGraphCommandHandler);
        ExitCommand                          = new DelegateCommand(ExitCommandHandler);
        m_FindPathBetweenPairVertexesCommand = new DelegateCommand(FindPathBetweenPairVertexesCommandHandler, CanExecuteFindPathBetweenPairVertexes);
        m_CompareGraphAlgorithmCommand       = new DelegateCommand(CompareGraphAlgorithmCommandHandler,       CanExecuteCompareGraphAlgorithmCommand);

        GraphViewModel.PropertyChanged += GraphViewModelPropertyChangedHandler;
    }


    private async void LoadGraphCommandHandler(string path)
    {
        var Logic = BusinessLogic.IoC.Get<IGraphSaveRestoreLogic>();

        var GraphData = await Logic.DeserializeAsync(path);

        m_Graph.ApplyData(GraphData);
    }

    private async void SaveGraphCommandHandler(string path)
    {
        var Logic = BusinessLogic.IoC.Get<IGraphSaveRestoreLogic>();

        await Logic.SerializeAsync(m_Graph.Data, path);
    }

    private void ExitCommandHandler()
    {
    }

    private async void FindPathBetweenPairVertexesCommandHandler()
    {
        if (SelectedGraphAlgorithm is null
            || !GraphViewModel.SelectedFromVertex.HasValue
            || !GraphViewModel.SelectedToVertex.HasValue) return;

        IsActiveOperation = true;

        var Algorithm  = SelectedGraphAlgorithm;
        var FromVertex = m_Graph.DictionaryVertices[GraphViewModel.SelectedFromVertex.Value];
        var ToVertex   = m_Graph.DictionaryVertices[GraphViewModel.SelectedToVertex.Value];

        Algorithm.Initialize(m_Graph, m_MatrixTable);

        var Result = await Algorithm.CalculatePathAsync((FromVertex, ToVertex));

        if (Result.Path is null)
            MessageBox.Show($"Пути из вершины \"{FromVertex.Data.TextString}\" в вершину \"{ToVertex.Data.TextString}\" не существует");

        GraphViewModel.FoundPath = Result.Path;
        FoundPathString = OperationHelper.GetPathString(new ((FromVertex, ToVertex), Result.Path));

        RaisePropertyChanged(nameof(FoundPathString));

        IsActiveOperation = false;
    }

    private bool CanExecuteFindPathBetweenPairVertexes()
    {
        return !IsActiveOperation && GraphViewModel.SelectedFromVertex is not null && GraphViewModel.SelectedToVertex is not null;
    }

    private async void CompareGraphAlgorithmCommandHandler()
    {
        IsActiveOperation = true;

        var ListAlgorithms    = m_GraphAlgorithmsCollection.ToList();
        var DictionaryResults = new Dictionary<IGraphAlgorithm, GraphAlgorithmResults>();

        foreach (var Algorithm in ListAlgorithms)
        {
            Algorithm.Initialize(m_Graph, m_MatrixTable);

            var Result = await Algorithm.CalculateAllPathAsync();

            DictionaryResults.Add(Algorithm, Result);
        }

        ShowComparableAlgorithmsWindowEvent?.Invoke(this, DictionaryResults);

        IsActiveOperation = false;
    }

    private bool CanExecuteCompareGraphAlgorithmCommand()
    {
        return !IsActiveOperation;
    }

    private void GraphViewModelPropertyChangedHandler(object? sender, PropertyChangedEventArgs e)
    {
        if (nameof(GraphViewModel.SelectedFromVertex).Equals(e.PropertyName)
            || nameof(GraphViewModel.SelectedFromVertex).Equals(e.PropertyName))
            m_FindPathBetweenPairVertexesCommand.RaiseCanExecuteChanged();
    }
}