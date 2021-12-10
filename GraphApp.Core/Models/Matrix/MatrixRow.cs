using GraphApp.Core.Data.Graphs;
using GraphApp.Core.Models.Graphs;


namespace GraphApp.Core.Models.Matrix;

public class MatrixRow : MatrixItem
{
    public MatrixRow(MatrixTable matrix, Vertex vertex) : base(matrix, vertex)
    {
    }
}