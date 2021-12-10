using System.Collections.ObjectModel;

using GraphApp.Core.Data;
using GraphApp.Core.Data.Graphs;

using Prism.Mvvm;


namespace GraphApp.Core.Models.Graphs;

public class Vertex : BindableBase
{
    public VertexData                 Data      { get; }
    public ObservableCollection<Edge> FromEdges { get; }
    public ObservableCollection<Edge> ToEdges   { get; }


    internal Vertex(VertexData data)
    {
        Data      = data;
        FromEdges = new ();
        ToEdges   = new ();
    }
}