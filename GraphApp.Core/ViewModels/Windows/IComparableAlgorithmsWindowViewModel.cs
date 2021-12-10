using GraphApp.Core.Data;
using GraphApp.Core.Services;


namespace GraphApp.Core.ViewModels.Windows;

public interface IComparableAlgorithmsWindowViewModel : IWindowViewModel
{
    Dictionary<IGraphAlgorithm, GraphAlgorithmResults> DictionaryResults { get; set; }
}