using GraphApp.Core.Data;
using GraphApp.Core.Data.Graphs;

using Prism.Mvvm;


namespace GraphApp.Core.Models.Graphs;

public class Edge : BindableBase
{
    public EdgeData Data { get; }
    public Vertex   From { get; }
    public Vertex   To   { get; }


    internal Edge(EdgeData data, Vertex from, Vertex to)
    {
        Data = data;
        From = from;
        To   = to;
    }
}