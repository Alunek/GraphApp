using Prism.Mvvm;


namespace GraphApp.Core.Data.Graphs;

public class GraphData : BindableBase
{
    private readonly Dictionary<Guid, VertexData>               m_DictionaryVertexes;
    private readonly Dictionary<(Guid From, Guid To), EdgeData> m_DictionaryEdges;


    public IReadOnlyDictionary<Guid, VertexData>               DictionaryVertexes => m_DictionaryVertexes;
    public IReadOnlyDictionary<(Guid From, Guid To), EdgeData> DictionaryEdges    => m_DictionaryEdges;


    public GraphData()
    {
        m_DictionaryVertexes = new();
        m_DictionaryEdges    = new();
    }


    public void AddVertex(VertexData vertexData)
    {
        m_DictionaryVertexes[vertexData.Id] = vertexData;
    }

    public bool RemoveVertex(Guid id)
    {
        return m_DictionaryVertexes.Remove(id);
    }

    internal void ClearVertexes()
    {
        m_DictionaryVertexes.Clear();
    }

    public void AddEdge(EdgeData edgeData)
    {
        m_DictionaryEdges[edgeData.PathId] = edgeData;
    }

    public bool RemoveEdge((Guid From, Guid To) path)
    {
        return m_DictionaryEdges.Remove(path);
    }

    internal void ClearEdges()
    {
        m_DictionaryEdges.Clear();
    }
}