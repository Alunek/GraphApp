using GraphApp.Core.Data.Graphs;
using GraphApp.Core.Models.Graphs;


namespace GraphApp.Core.Models.Matrix;

public class MatrixColumn : MatrixItem
{
    public MatrixColumn(MatrixTable matrix, Vertex vertex) : base(matrix, vertex)
    {
    }
}