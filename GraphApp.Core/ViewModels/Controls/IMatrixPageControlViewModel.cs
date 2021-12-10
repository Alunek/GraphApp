using GraphApp.Core.Models.Matrix;
using GraphApp.Core.Views.Controls;


namespace GraphApp.Core.ViewModels.Controls;

public interface IMatrixPageControlViewModel : IControlViewModel
{
    ICollection<int> MatrixSizeCollection    { get; }
    int              SelectedMatrixSizeValue { get; set; }

    public IMatrixTableControlViewModel MatrixTableViewModel { get; }
    public IMatrixTableControlView      MatrixTableView      { get; }
}