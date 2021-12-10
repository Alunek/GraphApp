using System.Collections.ObjectModel;
using System.ComponentModel;

using GraphApp.Core.Data.Graphs;
using GraphApp.Core.Models.Graphs;

using Prism.Mvvm;


namespace GraphApp.Core.Models.Matrix;

public abstract class MatrixItem : BindableBase
{
    protected readonly Vertex                       Vertex;
    protected readonly Dictionary<Guid, MatrixCell> DictionaryCells;


    public MatrixTable Table { get; }

    public Guid                                  VertexId                => VertexData.Id;
    public VertexData                            VertexData              => Vertex.Data;
    public string                                Text                    => VertexData.TextString;
    public ObservableCollection<MatrixCell>      Cells                   { get; }
    public IReadOnlyDictionary<Guid, MatrixCell> ReadOnlyDictionaryCells => DictionaryCells;


    protected MatrixItem(MatrixTable matrix, Vertex vertex)
    {
        Table           = matrix;
        Vertex          = vertex;
        Cells           = new();
        DictionaryCells = new();

        Vertex.PropertyChanged += VertexPropertyChangedHandler;
    }


    public MatrixCell CreateCell(MatrixColumn column, MatrixRow row, Guid vertexId)
    {
        var EdgeData = Vertex.FromEdges
            .FirstOrDefault(edge => edge.Data.ToId == vertexId)?.Data;

        return new MatrixCell(column, row, EdgeData);
    }

    public void AddCell(int index, Guid vertexId, MatrixCell cell)
    {
        DictionaryCells.Add(vertexId, cell);
        Cells.Insert(index, cell);
    }

    public bool RemoveCell(Guid vertexId)
    {
        if (!DictionaryCells.Remove(vertexId, out var Cell)) return false;

        return Cells.Remove(Cell);
    }

    public MatrixCell FindCell(Guid vertexId)
    {
        return DictionaryCells[vertexId];
    }

    public void ClearCells()
    {
        foreach (var Cell in Cells) Cell.Data = null;
    }


    private void VertexPropertyChangedHandler(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(VertexData.TextString)) RaisePropertyChanged(nameof(Text));
    }
}