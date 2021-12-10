using System.Collections.ObjectModel;
using System.Collections.Specialized;

using GraphApp.Core.Data;
using GraphApp.Core.Data.Graphs;
using GraphApp.Core.Models.Graphs;

using Prism.Mvvm;


namespace GraphApp.Core.Models.Matrix;

public class MatrixTable : BindableBase
{
    public record Element(Guid Id, MatrixColumn Column, MatrixRow Row);

    private readonly Graph                         m_Graph;
    private readonly ObservableCollection<Element> m_Elements;
    private readonly Dictionary<Guid, Element>     m_DictionaryElements;

    public ReadOnlyObservableCollection<Element> Elements           { get; }
    public IReadOnlyDictionary<Guid, Element>    DictionaryElements => m_DictionaryElements;

    public int Size
    {
        get => Elements.Count;
        set => UpdateSize(value);
    }


    public MatrixTable(Graph graph)
    {
        m_Graph              = graph;
        m_Elements           = new();
        Elements             = new(m_Elements);
        m_DictionaryElements = new();

        m_Elements.CollectionChanged += (s, e) => RaisePropertyChanged(nameof(Size));

        if (graph.Vertices is INotifyCollectionChanged VertexCollection) VertexCollection.CollectionChanged += VertexesCollectionChangedHandler;
        if (graph.Edges is INotifyCollectionChanged EdgesCollection) EdgesCollection.CollectionChanged      += EdgesCollectionChangedHandler;
    }


    public MatrixCell GetCellByIndex(int i, int j)
    {
        var Column = Elements[j].Column;
        var Row    = Elements[i].Row;

        return FindIntersection(Row, Column);
    }

    public IList<IList<double>> GetMatrix()
    {
        double[][] Matrix = new double[Size][];

        for (int i = 0; i < Size; ++i)
        {
            Matrix[i] = new double[Size];

            for (int j = 0; j < Size; ++j)
                if (i == j)
                {
                    Matrix[i][j] = double.NaN;
                }
                else
                {
                    var From = m_Elements[i].Row;
                    var To   = m_Elements[j].Column;

                    Matrix[i][j] = FindIntersection(From, To).Data?.Value ?? double.NaN;
                }
        }

        return Matrix;
    }

    private void UpdateSize(int size)
    {
        int NewSize = size;
        int OldSize = Size;

        if (OldSize > NewSize)
            for (int i = OldSize - 1; i >= NewSize; --i)
                m_Graph.RemoveVertex(m_Elements.ElementAt(i).Id);
        else
            for (int i = OldSize; i < NewSize; ++i)
                m_Graph.AddVertex();
    }

    internal void AddEdge((Guid From, Guid To) path, double value)
    {
        m_Graph.AddEdge(path, value);
    }

    internal void RemoveEdge((Guid From, Guid To) path)
    {
        m_Graph.RemoveEdge(path);
    }

    #region Element

    private void AddElementHelper(Element element, int index)
    {
        var (NewId, NewColumn, NewRow) = element;
        int NewIndex = index;

        m_DictionaryElements.Add(NewId, element);

        if (NewColumn.VertexData != NewRow.VertexData)
            throw new InvalidOperationException("Нарушена целостность матрицы");

        var NewElements = m_Elements
            .Select((e, i) => (Index: i < NewIndex ? i : i + 1, Element: e)).ToList();

        NewElements = NewElements.Take(NewIndex)
            .Append((Index: NewIndex, Element: element))
            .Union(NewElements.TakeLast(m_Elements.Count - NewIndex))
            .ToList();

        foreach ((int Index, var (Id, Column, Row)) in NewElements)
        {
            if (Column.VertexData != Row.VertexData)
                throw new InvalidOperationException("Нарушена целостность матрицы");

            if (Index == NewIndex)
            {
                var Cell = MatrixCell.GetEmpty(NewColumn, NewRow);

                NewColumn.AddCell(NewIndex, NewId, Cell);
                NewRow.AddCell(NewIndex, NewId, Cell);

                continue;
            }

            var NewFromCell = NewRow.CreateCell(Column, NewRow, Id);
            var NewToCell   = Row.CreateCell(NewColumn, Row, NewId);

            NewColumn.AddCell(Index, Id, NewToCell);
            NewRow.AddCell(Index, Id, NewFromCell);

            Column.AddCell(NewIndex, NewId, NewFromCell);
            Row.AddCell(NewIndex, NewId, NewToCell);
        }

        m_Elements.Insert(index, element);
    }

    private bool RemoveElementHelper(Element element)
    {
        if (!m_Elements.Contains(element)) return false;

        var (OldId, OldColumn, OldRow) = element;

        m_DictionaryElements.Remove(OldId);

        if (OldColumn.VertexData != OldRow.VertexData)
            throw new InvalidOperationException("Нарушена целостность матрицы");

        foreach (var (Id, Column, Row) in m_Elements)
        {
            if (Column.VertexData != Row.VertexData)
                throw new InvalidOperationException("Нарушена целостность матрицы");

            OldColumn.RemoveCell(Id);
            OldRow.RemoveCell(Id);

            if (OldId == Id) continue;

            Column.RemoveCell(OldId);
            Row.RemoveCell(OldId);
        }

        return m_Elements.Remove(element);
    }

    private void ClearElementsHelper()
    {
        m_DictionaryElements.Clear();
        m_Elements.Clear();
    }

    #endregion

    #region Vertex

    private void VertexesCollectionChangedHandler(object? sender, NotifyCollectionChangedEventArgs e)
    {
        int Index;
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                Index = e.NewStartingIndex;
                foreach (Vertex Item in e.NewItems!) AddVertexHelper(Item, Index++);
                break;

            case NotifyCollectionChangedAction.Remove:
                foreach (Vertex Item in e.OldItems!) RemoveVertexHelper(Item);
                break;

            case NotifyCollectionChangedAction.Replace:
                foreach (Vertex Item in e.OldItems!) RemoveVertexHelper(Item);
                Index = e.NewStartingIndex;
                foreach (Vertex Item in e.NewItems!) AddVertexHelper(Item, Index++);
                break;

            case NotifyCollectionChangedAction.Move:
                break;

            case NotifyCollectionChangedAction.Reset:
                ClearVertexesHelper();
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void AddVertexHelper(Vertex vertex, int index)
    {
        var Element = CreateElement(vertex);

        InsertElement(Element, index);
    }

    private void RemoveVertexHelper(Vertex vertex)
    {
        var Element = FindElement(vertex.Data.Id);
        if (Element is null) return;

        RemoveElement(Element);
    }

    private void ClearVertexesHelper()
    {
        ClearEdgesHelper();
        ClearElements();
    }

    #endregion

    #region Edge

    private void EdgesCollectionChangedHandler(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                foreach (Edge Item in e.NewItems!) AddEdgeHelper(Item);
                break;

            case NotifyCollectionChangedAction.Remove:
                foreach (Edge Item in e.OldItems!) RemoveEdgeHelper(Item);
                break;

            case NotifyCollectionChangedAction.Replace:
                foreach (Edge Item in e.OldItems!) RemoveEdgeHelper(Item);
                foreach (Edge Item in e.NewItems!) AddEdgeHelper(Item);
                break;

            case NotifyCollectionChangedAction.Move:
                break;

            case NotifyCollectionChangedAction.Reset:
                ClearEdgesHelper();
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void AddEdgeHelper(Edge edge)
    {
        var Cell = FindCell(edge.Data.PathId);
        if (Cell is null) return;

        SetCellData(Cell, edge.Data);
    }

    private void RemoveEdgeHelper(Edge edge)
    {
        var Cell = FindCell(edge.Data.PathId);
        if (Cell is null) return;

        SetCellData(Cell);
    }

    private void ClearEdgesHelper()
    {
        foreach (var (_, Column, Row) in m_Elements)
        {
            Column.ClearCells();
            Row.ClearCells();
        }
    }

    #endregion

    private Element CreateElement(Vertex vertex)
    {
        var NewColumn = new MatrixColumn(this, vertex);
        var NewRow    = new MatrixRow(this, vertex);

        return new Element(vertex.Data.Id, NewColumn, NewRow);
    }

    private void InsertElement(Element element, int index)
    {
        AddElementHelper(element, index);
    }

    private Element? FindElement(Guid id)
    {
        return m_DictionaryElements.TryGetValue(id, out var Element) ? Element : null;
    }

    private bool RemoveElement(Element element)
    {
        return RemoveElementHelper(element);
    }

    private void ClearElements()
    {
        ClearElementsHelper();
    }

    private MatrixCell? FindCell((Guid From, Guid To) path)
    {
        var FromElement = FindElement(path.From);
        if (FromElement is null) return null;

        var ToElement = FindElement(path.To);
        if (ToElement is null) return null;

        return FindIntersection(FromElement.Row, ToElement.Column);
    }

    private static MatrixCell FindIntersection(MatrixRow rowFrom, MatrixColumn columnTo)
    {
        var FromCell = rowFrom.FindCell(columnTo.VertexId);
        var ToCell   = columnTo.FindCell(rowFrom.VertexId);

        if (FromCell != ToCell)
            throw new InvalidOperationException("Нарушена целостность матрицы");

        return FromCell;
    }

    private static void SetCellData(MatrixCell cell, EdgeData? data = null)
    {
        cell.Data = data;
    }
}