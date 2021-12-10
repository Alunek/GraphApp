using System.Diagnostics;

using GraphApp.Core.Data;
using GraphApp.Core.Models.Graphs;
using GraphApp.Core.Models.Matrix;
using GraphApp.Core.Services;


namespace GraphApp.WPF.Common.Services;

internal class GraphAlgorithmDijkstraLogic : GraphAlgorithmBaseLogic, IGraphAlgorithmDijkstraLogic
{
    private record DijkstraContext(double[] Distances, int[] Parents, bool[] Frozen);


    public override string Name => "Дейкстра";


    protected override GraphAlgorithmResult CalculatePath((Vertex From, Vertex To) path)
    {
        int FromIndex = Vertices!.IndexOf(path.From);
        int ToIndex   = Vertices!.IndexOf(path.To);

        // Start algorithm
        var Watch = Stopwatch.StartNew();

        var Context = CalculateMainPart(FromIndex);

        var Path = BuildPath(Context, FromIndex, ToIndex);

        // Stop algorithm
        Watch.Stop();

        return new GraphAlgorithmResult(Path, Size, Watch.Elapsed);
    }

    protected override GraphAlgorithmResults CalculateAllPath()
    {
        var DictionaryPaths = new Dictionary<(Vertex From, Vertex To), List<Vertex>?>();

        // Start algorithm
        var Watch = Stopwatch.StartNew();

        for (int FromIndex = 0; FromIndex < Size; FromIndex++)
        for (int ToIndex = 0; ToIndex < Size; ToIndex++)
        {
            if (FromIndex == ToIndex) continue;

            var Context = CalculateMainPart(FromIndex);

            var Path = BuildPath(Context, FromIndex, ToIndex);

            DictionaryPaths[(Vertices![FromIndex], Vertices![ToIndex])] = Path;
        }

        // Stop algorithm
        Watch.Stop();

        return new GraphAlgorithmResults(DictionaryPaths, Size, Watch.Elapsed);
    }


    private DijkstraContext CalculateMainPart(int fromIndex)
    {
        double[][] Edges = (double[][])this.Edges!;

        double[] Distances = new double[Size];
        int[]    Parents   = new int[Size];
        bool[]   Frozen    = new bool[Size];
        Distances[fromIndex] = 0;

        for (int i = 0; i < Size; ++i)
            if (i != fromIndex)
                Distances[i] = double.PositiveInfinity;

        for (int i = 0; i < Size; ++i)
        {
            int v = -1;
            for (int j = 0; j < Size; ++j)
                if (!Frozen[j] && (v == -1 || Distances[j] < Distances[v]))
                    v = j;

            if (double.IsPositiveInfinity(Distances[v]))
                break;

            Frozen[v] = true;

            for (int j = 0; j < Size; ++j)
            {
                double NewDistance = Distances[v] + Edges[v][j];

                if (!(NewDistance < Distances[j])) continue;

                Distances[j] = NewDistance;
                Parents[j]   = v;
            }
        }

        return new DijkstraContext(Distances, Parents, Frozen);
    }

    private List<Vertex>? BuildPath(DijkstraContext context, int fromIndex, int toIndex)
    {
        if (!context.Frozen[toIndex]) return null;

        List<Vertex>? PathVertex = new();

        for (int v = toIndex; v != fromIndex; v = context.Parents[v])
            PathVertex.Add(Vertices![v]);
        PathVertex.Add(Vertices![fromIndex]);
        PathVertex.Reverse();

        return PathVertex;
    }
}