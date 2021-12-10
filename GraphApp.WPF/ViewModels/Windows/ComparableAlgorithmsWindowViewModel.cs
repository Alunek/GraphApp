using System.Collections.ObjectModel;

using GraphApp.Core.Data;
using GraphApp.Core.Helper;
using GraphApp.Core.Services;
using GraphApp.Core.ViewModels.Windows;


namespace GraphApp.WPF.ViewModels.Windows;

internal class ComparableAlgorithmsWindowViewModel : WindowViewModelBase, IComparableAlgorithmsWindowViewModel
{
    public record ComparableAlgorithmsItem(string AlgorithmName, string CountVertices, string TimeValue, List<string> PathsVertices);


    private Dictionary<IGraphAlgorithm, GraphAlgorithmResults> m_DictionaryResults = new();


    public Dictionary<IGraphAlgorithm, GraphAlgorithmResults> DictionaryResults
    {
        get => m_DictionaryResults;
        set
        {
            m_DictionaryResults = value;

            UpdateComparableAlgorithmsItems();
        }
    }

    public ObservableCollection<ComparableAlgorithmsItem> Items { get; }


    public ComparableAlgorithmsWindowViewModel(
        IBusinessLogic businessLogic) : base(businessLogic)
    {
        Items = new();
    }


    private void UpdateComparableAlgorithmsItems()
    {
        Items.Clear();

        foreach ((var GraphAlgorithm, (var Paths, int Count, var Time)) in DictionaryResults)
        {
            string Name          = GraphAlgorithm.Name;
            string CountVertices = $"{Count}";
            string TimeValue     = $"{Time:fffffff}".Insert(2, ".");

            var PathsVertices = Paths
                .Select(
                    path => OperationHelper.GetPathString(path))
                .ToList();

            Items.Add(new ComparableAlgorithmsItem(Name, CountVertices, TimeValue, PathsVertices));
        }

        RaisePropertyChanged(nameof(Items));
    }
}