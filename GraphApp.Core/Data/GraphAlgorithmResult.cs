using GraphApp.Core.Models.Graphs;


namespace GraphApp.Core.Data;

public record GraphAlgorithmResult(List<Vertex>? Path, int CountVertices, TimeSpan Time);

public record GraphAlgorithmResults(Dictionary<(Vertex From, Vertex To), List<Vertex>?> Paths, int CountVertices, TimeSpan Time);