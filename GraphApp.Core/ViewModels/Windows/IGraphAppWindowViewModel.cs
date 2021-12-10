using System.Windows.Input;

using GraphApp.Core.Data;
using GraphApp.Core.Services;
using GraphApp.Core.ViewModels.Controls;
using GraphApp.Core.Views.Controls;


namespace GraphApp.Core.ViewModels.Windows;

public interface IGraphAppWindowViewModel : IWindowViewModel
{
    IGraphControlViewModel      GraphViewModel        { get; }
    IMatrixPageControlViewModel MatrixPageViewModel   { get; }
    IGraphControlView           GraphControlView      { get; }
    IMatrixPageControlView      MatrixPageControlView { get; }
    ICommand                    LoadGraphCommand      { get; }
    ICommand                    SaveGraphCommand      { get; }
    ICommand                    ExitCommand           { get; }


    event EventHandler<Dictionary<IGraphAlgorithm, GraphAlgorithmResults>> ShowComparableAlgorithmsWindowEvent;
}