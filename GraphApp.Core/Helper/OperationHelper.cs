using GraphApp.Core.Models.Graphs;

namespace GraphApp.Core.Helper;

public class OperationHelper
{
    public static bool IntoRange(int value, int min, int max)
    {
        return value >= min && value <= max;
    }

    public static bool IntoRange(double value, double min, double max)
    {
        return value >= min && value <= max;
    }

    public static string GetPathString(KeyValuePair<(Vertex From, Vertex To), List<Vertex>?> path)
	{
        return $"{path.Key.From.Data.TextString,2} -> {path.Key.To.Data.TextString,2} "
            + $"{(path.Value is null ? "" : $"({path.Value.SkipLast(1).Select((v, i) => v.FromEdges.FirstOrDefault(e => e.To == path.Value[i + 1])?.Data.Value ?? 0).Sum():F2})"),7}: "
            + $"{(path.Value is null ? "отсутствует" : string.Join("->", path.Value.Select(v => v.Data.TextString)))}";

    }
}