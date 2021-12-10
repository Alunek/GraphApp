using System.Diagnostics;

using GraphApp.Core.Data;
using GraphApp.Core.Models.Graphs;
using GraphApp.Core.Models.Matrix;
using GraphApp.Core.Services;


namespace GraphApp.WPF.Common.Services;

internal abstract class GraphAlgorithmBaseLogic : IGraphAlgorithm
{
    protected int                   Size;
    protected IList<Vertex>?        Vertices;
    protected IList<IList<double>>? Edges;


    public abstract string Name { get; }


    public void Initialize(Graph graph, MatrixTable matrixTable)
    {
        Size     = graph.Vertices.Count;
        Vertices = graph.Vertices.ToArray();
        Edges    = matrixTable.GetMatrix();

        CheckMainData();
    }

    public Task<GraphAlgorithmResult> CalculatePathAsync((Vertex From, Vertex To) path)
    {
        CheckMainData();

        return Task.Run(() => CalculatePath(path));
    }

    public Task<GraphAlgorithmResults> CalculateAllPathAsync()
    {
        CheckMainData();

        return Task.Run(CalculateAllPath);
    }

    protected abstract GraphAlgorithmResult CalculatePath((Vertex From, Vertex To) path);
    protected abstract GraphAlgorithmResults CalculateAllPath();


    protected void CheckMainData()
    {
        if (Vertices is null) throw new ArgumentNullException(nameof(Vertices));
        if (Edges is null) throw new ArgumentNullException(nameof(Edges));
    }
}