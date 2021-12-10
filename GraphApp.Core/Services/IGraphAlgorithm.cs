using GraphApp.Core.Data;
using GraphApp.Core.Models;
using GraphApp.Core.Models.Graphs;
using GraphApp.Core.Models.Matrix;


namespace GraphApp.Core.Services;

public interface IGraphAlgorithm
{
    string Name { get; }


    void Initialize(Graph graph, MatrixTable matrixTable);

    Task<GraphAlgorithmResult> CalculatePathAsync((Vertex From, Vertex To) path);
    Task<GraphAlgorithmResults> CalculateAllPathAsync();
}