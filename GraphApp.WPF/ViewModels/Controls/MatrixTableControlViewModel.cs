using System.Collections.ObjectModel;
using System.Collections.Specialized;

using GraphApp.Core.Models.Graphs;
using GraphApp.Core.Models.Matrix;
using GraphApp.Core.Services;
using GraphApp.Core.ViewModels.Controls;


namespace GraphApp.WPF.ViewModels.Controls;

internal class MatrixTableControlViewModel : ControlViewModelBase, IMatrixTableControlViewModel
{
    private readonly MatrixTable m_MatrixTable;

    private int m_LastMatrixSize = 0;


    public ObservableCollection<MatrixColumn> Columns { get; }
    public ObservableCollection<MatrixRow>    Rows    { get; }
    public ObservableCollection<MatrixCell>   Cells   { get; }

    public int    MatrixSize => m_MatrixTable.Elements.Count;
    public double CellSize   { get; private set; }


    public MatrixTableControlViewModel(IBusinessLogic businessLogic, MatrixTable matrixTable) : base(businessLogic)
    {
        m_MatrixTable = matrixTable;

        Columns = new();
        Rows    = new();
        Cells   = new();

        CellSize = 35.0d;

        if (m_MatrixTable.Elements is INotifyCollectionChanged ElementsCollection)
            ElementsCollection.CollectionChanged += ElementsCollectionChangedHandler;
    }


    private void ElementsCollectionChangedHandler(object? sender, NotifyCollectionChangedEventArgs e)
    {
        int Index;
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                Index = e.NewStartingIndex;
                foreach (MatrixTable.Element Item in e.NewItems!) AddElementHelper(Item, Index++);
                break;

            case NotifyCollectionChangedAction.Remove:
                Index = e.OldStartingIndex;
                foreach (MatrixTable.Element Item in e.OldItems!) RemoveElementHelper(Item, Index++);
                break;

            case NotifyCollectionChangedAction.Replace:
                Index = e.OldStartingIndex;
                foreach (MatrixTable.Element Item in e.OldItems!) RemoveElementHelper(Item, Index++);
                Index = e.NewStartingIndex;
                foreach (MatrixTable.Element Item in e.NewItems!) AddElementHelper(Item, Index++);
                break;

            case NotifyCollectionChangedAction.Move:
                break;

            case NotifyCollectionChangedAction.Reset:
                ClearElementHelper();
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        m_LastMatrixSize = MatrixSize;
        RaisePropertyChanged(nameof(MatrixSize));
    }

    private void AddElementHelper(MatrixTable.Element element, int index)
    {
        Columns.Insert(index, element.Column);
        Rows.Insert(index, element.Row);

        var Table = element.Column.Table;

        int i = 0;
        int j = index;
        for (; i      < index; ++i) Cells.Insert(i      * MatrixSize + j, Table.GetCellByIndex(i, j));
        for (j = 0; j < MatrixSize; ++j) Cells.Insert(i * MatrixSize + j, Table.GetCellByIndex(i, j));
        i++;
        j = index;
        for (; i < MatrixSize; ++i) Cells.Insert(i * MatrixSize + j, Table.GetCellByIndex(i, j));
    }

    private void RemoveElementHelper(MatrixTable.Element element, int index)
    {
        Columns.Remove(element.Column);
        Rows.Remove(element.Row);

        int OldMatrixSize = m_LastMatrixSize;

        int i = OldMatrixSize - 1;
        int j = index;
        for (; i                      > index; --i) Cells.RemoveAt(i * OldMatrixSize + j);
        for (j = OldMatrixSize - 1; j >= 0; --j) Cells.RemoveAt(i    * OldMatrixSize + j);
        i--;
        j = index;
        for (; i >= 0; --i) Cells.RemoveAt(i * OldMatrixSize + j);
    }

    private void ClearElementHelper()
    {
        Columns.Clear();
        Rows.Clear();
        Cells.Clear();
    }
}