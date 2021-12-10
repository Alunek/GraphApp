using System.Diagnostics;

using GraphApp.Core.Data;
using GraphApp.Core.Models.Graphs;
using GraphApp.Core.Services;


namespace GraphApp.WPF.Common.Services;

internal class GraphAlgorithmFloydLogic : GraphAlgorithmBaseLogic, IGraphAlgorithmFloydLogic
{
    private record FloydContext(double[][] Edges, int[][] Path);


    public override string Name => "Флоид";


    protected override GraphAlgorithmResult CalculatePath((Vertex From, Vertex To) path)
    {
        int FromIndex = Vertices!.IndexOf(path.From);
        int ToIndex   = Vertices!.IndexOf(path.To);

        // Start algorithm
        var Watch = Stopwatch.StartNew();

        var Context = CalculateMainPart();

        var Path = BuildPath(Context, FromIndex, ToIndex)?
            .Append(Vertices![ToIndex])
            .ToList();

        // Stop algorithm
        Watch.Stop();

        return new GraphAlgorithmResult(Path, Size, Watch.Elapsed);
    }

    protected override GraphAlgorithmResults CalculateAllPath()
    {
        var DictionaryPaths = new Dictionary<(Vertex From, Vertex To), List<Vertex>?>();

        // Start algorithm
        var Watch = Stopwatch.StartNew();

        var Context = CalculateMainPart();

        for (int FromIndex = 0; FromIndex < Size; FromIndex++)
        for (int ToIndex = 0; ToIndex < Size; ToIndex++)
        {
            if (FromIndex == ToIndex) continue;

            var Path = BuildPath(Context, FromIndex, ToIndex);

            DictionaryPaths[(Vertices![FromIndex], Vertices![ToIndex])] = Path;
        }

        // Stop algorithm
        Watch.Stop();

        return new GraphAlgorithmResults(DictionaryPaths, Size, Watch.Elapsed);
    }


    private FloydContext CalculateMainPart()
    {
        double[][] Edges  = (double[][])this.Edges!;
        int[][]    Path = new int[Size][];

        for (int i = 0; i < Size; ++i)
        {
            Path[i] = new int[Size];
            for (int j = 0; j < Size; ++j)
                if (double.IsNormal(Edges[i][j]))
                {
                    Path[i][j]  = j;
                    Edges[i][j] = Edges[i][j]; 
                }
                else
                {
                    Path[i][j]  = -1;
                    Edges[i][j] = double.PositiveInfinity;
                }
        }

        for (int k = 0; k < Size; ++k)
        for (int i = 0; i < Size; ++i)
        for (int j = 0; j < Size; ++j)
            if (Edges[i][k] < double.PositiveInfinity && Edges[k][j] < double.PositiveInfinity)
                if (Edges[i][k] + Edges[k][j] < Edges[i][j] - double.Epsilon)
                {
                    Edges[i][j] = Edges[i][k] + Edges[k][j];
                    Path[i][j]  = Path[i][k];
                }

        return new FloydContext(Edges, Path);
    }

    private List<Vertex>? BuildPath(FloydContext context, int fromIndex, int toIndex)
    {
        if (context.Path[fromIndex][toIndex] == Size) return null;

        List<Vertex>? PathVertex = new();

        for (int v = fromIndex; v != toIndex; v = context.Path[v][toIndex])
        {
            if (v == -1) return null;
            PathVertex.Add(Vertices![v]);
        }
        PathVertex.Add(Vertices![toIndex]);

        return PathVertex;
    }
}