using System.Collections;
using System.Collections.Specialized;

using GraphApp.Core.Models.Matrix;
using GraphApp.Core.Services;
using GraphApp.Core.ViewModels.Controls;
using GraphApp.Core.Views.Controls;

using Ninject;
using Ninject.Parameters;


namespace GraphApp.WPF.ViewModels.Controls;

internal class MatrixPageControlViewModel : ControlViewModelBase, IMatrixPageControlViewModel
{
    private const int c_MinSize = 0;
    private const int c_MaxSize = 20;


    private readonly MatrixTable m_MatrixTable;


    public ICollection<int> MatrixSizeCollection { get; }

    public int SelectedMatrixSizeValue
    {
        get => m_MatrixTable.Size;
        set => m_MatrixTable.Size = value;
    }

    public IMatrixTableControlViewModel MatrixTableViewModel { get; }
    public IMatrixTableControlView      MatrixTableView      { get; }


    public MatrixPageControlViewModel(IBusinessLogic businessLogic, MatrixTable matrixTable) : base(businessLogic)
    {
        m_MatrixTable        = matrixTable;
        MatrixSizeCollection = Enumerable.Range(c_MinSize, c_MaxSize - c_MinSize + 1).ToArray();

        var MatrixTableParameter = new ConstructorArgument("matrixTable", m_MatrixTable);
        MatrixTableViewModel = BusinessLogic.IoC.Get<IMatrixTableControlViewModel>(MatrixTableParameter);
        MatrixTableView      = BusinessLogic.IoC.Get<IMatrixTableControlView>();

        MatrixTableView.ViewModel = MatrixTableViewModel;

        if (m_MatrixTable.Elements is INotifyCollectionChanged ElementsCollection)
            ElementsCollection.CollectionChanged += (o, e) => RaisePropertyChanged(nameof(SelectedMatrixSizeValue));
    }
}