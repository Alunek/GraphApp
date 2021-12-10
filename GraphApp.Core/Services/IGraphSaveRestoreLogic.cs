using GraphApp.Core.Data;
using GraphApp.Core.Data.Graphs;


namespace GraphApp.Core.Services;

public interface IGraphSaveRestoreLogic
{
    public Task            SerializeAsync(GraphData data, string path);
    public Task<GraphData> DeserializeAsync(string  path);
}