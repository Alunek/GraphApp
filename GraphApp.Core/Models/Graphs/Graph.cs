using System.Collections.ObjectModel;
using System.Collections.Specialized;

using GraphApp.Core.Data;
using GraphApp.Core.Data.Graphs;

using Prism.Mvvm;


namespace GraphApp.Core.Models.Graphs;

public class Graph : BindableBase
{
    private readonly ObservableCollection<Vertex>           m_Vertices;
    private readonly ObservableCollection<Edge>             m_Edges;
    private readonly Dictionary<Guid, Vertex>               m_DictionaryVertices;
    private readonly Dictionary<(Guid From, Guid To), Edge> m_DictionaryEdges;


    public GraphData Data { get; private set; }

    public ReadOnlyObservableCollection<Vertex>    Vertices           { get; }
    public ReadOnlyObservableCollection<Edge>      Edges              { get; }
    public IReadOnlyDictionary<Guid, Vertex>       DictionaryVertices => m_DictionaryVertices;
    public IReadOnlyDictionary<(Guid, Guid), Edge> DictionaryEdges    => m_DictionaryEdges;


    public Graph()
    {
        Data = new GraphData();

        m_Vertices           = new();
        m_Edges              = new();
        Vertices             = new(m_Vertices);
        Edges                = new(m_Edges);
        m_DictionaryVertices = new();
        m_DictionaryEdges    = new();

        m_Vertices.CollectionChanged += VerticesCollectionChangedHandler;
        m_Edges.CollectionChanged    += EdgesCollectionChangedHandler;
    }


    public void ApplyData(GraphData data)
    {
        m_Vertices.Clear();
        m_Edges.Clear();

        Data = data;

        foreach (var (_, VertexData) in data.DictionaryVertexes) AddVertex(VertexData);

        foreach (var (_, EdgeData) in data.DictionaryEdges) AddEdge(EdgeData);

        RaisePropertyChanged(nameof(Data));
    }


    #region Vertex

    public Vertex AddVertex(string? name = null)
    {
        var Id = Guid.NewGuid();
        var VertexData = new VertexData(Id)
        {
            TextString = name ?? GetFreeName()
        };

        return AddVertex(VertexData);
    }

    private Vertex AddVertex(VertexData vertexData)
    {
        var NewVertex = new Vertex(vertexData);

        int?   NewVertexValue  = int.TryParse(NewVertex.Data.TextString, out int Value) ? Value : null;
        string NewVertexString = NewVertex.Data.TextString;

        int Index = m_Vertices
            .Select((v, i) => (Text: v.Data.TextString, Index: i))
            .FirstOrDefault(
                pair => NewVertexValue.HasValue && int.TryParse(pair.Text, out int VertexValue)
                    ? VertexValue                                                          >= NewVertexValue
                    : string.Compare(pair.Text, NewVertexString, StringComparison.Ordinal) >= 0,
                (Text: $"{m_Vertices.Count}", Index: m_Vertices.Count)).Index;

        m_Vertices.Insert(Index, NewVertex);

        return NewVertex;
    }

    public bool RemoveVertex(Guid id)
    {
        return m_DictionaryVertices.TryGetValue(id, out var Vertex) && m_Vertices.Remove(Vertex);
    }

    private void VerticesCollectionChangedHandler(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                foreach (Vertex Item in e.NewItems!) AddVertexHelper(Item);
                break;

            case NotifyCollectionChangedAction.Remove:
                foreach (Vertex Item in e.OldItems!) RemoveVertexHelper(Item);
                break;

            case NotifyCollectionChangedAction.Replace:
                foreach (Vertex Item in e.OldItems!) RemoveVertexHelper(Item);
                foreach (Vertex Item in e.NewItems!) AddVertexHelper(Item);
                break;

            case NotifyCollectionChangedAction.Move:
                break;

            case NotifyCollectionChangedAction.Reset:
                ClearVertexesHelper();
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void AddVertexHelper(Vertex vertex)
    {
        m_DictionaryVertices[vertex.Data.Id] = vertex;
        Data.AddVertex(vertex.Data);
    }

    private void RemoveVertexHelper(Vertex vertex)
    {
        foreach (var Edge in vertex.FromEdges) m_Edges.Remove(Edge);
        foreach (var Edge in vertex.ToEdges) m_Edges.Remove(Edge);
        var Id = vertex.Data.Id;
        m_DictionaryVertices.Remove(Id);
        Data.RemoveVertex(Id);
    }

    private void ClearVertexesHelper()
    {
        ClearEdgesHelper();
        m_DictionaryVertices.Clear();
        Data.ClearVertexes();
    }

    #endregion


    #region Edge

    public Edge? AddEdge((Guid From, Guid To) path, double value = 1.0d)
    {
        if (m_DictionaryEdges.TryGetValue(path, out _)
            || !m_DictionaryVertices.TryGetValue(path.From, out var FromVertex)
            || !m_DictionaryVertices.TryGetValue(path.To,   out var ToVertex)
            || path.From == path.To) return null;

        var PathVertex = (From: FromVertex.Data, To: ToVertex.Data);

        var EdgeData = new EdgeData(PathVertex, value);

        return AddEdge(EdgeData, FromVertex, ToVertex);
    }

    private Edge? AddEdge(EdgeData edgeData, Vertex? fromVertex = null, Vertex? toVertex = null)
    {
        if (fromVertex is null)
        {
            if (!m_DictionaryVertices.TryGetValue(edgeData.FromId, out var FromVertex)) return null;

            fromVertex = FromVertex;
        }

        if (toVertex is null)
        {
            if (!m_DictionaryVertices.TryGetValue(edgeData.ToId, out var ToVertex)) return null;

            toVertex = ToVertex;
        }

        var Edge = new Edge(edgeData, fromVertex, toVertex);

        fromVertex.FromEdges.Add(Edge);
        toVertex.ToEdges.Add(Edge);
        m_Edges.Add(Edge);

        return Edge;
    }

    public bool RemoveEdge((Guid From, Guid To) path)
    {
        return m_DictionaryEdges.TryGetValue(path, out var Edge) && m_Edges.Remove(Edge);
    }

    private void EdgesCollectionChangedHandler(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                foreach (Edge Item in e.NewItems!) AddEdgeHelper(Item);
                break;

            case NotifyCollectionChangedAction.Remove:
                foreach (Edge Item in e.OldItems!) RemoveEdgeHelper(Item);
                break;

            case NotifyCollectionChangedAction.Replace:
                foreach (Edge Item in e.OldItems!) RemoveEdgeHelper(Item);
                foreach (Edge Item in e.NewItems!) AddEdgeHelper(Item);
                break;

            case NotifyCollectionChangedAction.Move:
                break;

            case NotifyCollectionChangedAction.Reset:
                ClearEdgesHelper();
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void AddEdgeHelper(Edge edge)
    {
        m_DictionaryEdges[edge.Data.PathId] = edge;
        Data.AddEdge(edge.Data);
    }

    private void RemoveEdgeHelper(Edge edge)
    {
        var Path = edge.Data.PathId;
        m_DictionaryEdges.Remove(Path);
        Data.RemoveEdge(Path);
    }

    private void ClearEdgesHelper()
    {
        m_DictionaryEdges.Clear();
        Data.ClearEdges();
    }

    #endregion

    private string GetFreeName()
    {
        var HashSetNames = m_Vertices
            .Select(v => v.Data.TextString)
            .ToHashSet();

        int Number = 1;
        while (HashSetNames.Contains($"{Number}")) Number++;

        return $"{Number}";
    }
}