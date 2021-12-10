using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

using GraphApp.Core.Data;
using GraphApp.Core.Data.Graphs;
using GraphApp.Core.Services;


namespace GraphApp.WPF.Common.Services;

internal class GraphSaveRestoreLogic : IGraphSaveRestoreLogic
{
    private const string c_VertexSectionName          = "Vertex";
    private const string c_VertexIdPropertyName       = "id";
    private const string c_VertexNamePropertyName     = "name";
    private const string c_VertexPositionPropertyName = "position";
    private const string c_EdgeSectionName            = "Edge";
    private const string c_EdgeFromPropertyName       = "from";
    private const string c_EdgeToPropertyName         = "to";
    private const string c_EdgeValuePropertyName      = "value";


    public Task SerializeAsync(GraphData data, string path)
    {
        return Task.Run(
            () =>
            {
                var File = new FileInfo(path);

                using var Writer = new StreamWriter(File.FullName);

                int VertexIndex = 1;
                foreach (var (_, VertexData) in data.DictionaryVertexes)
                    Writer.Write(VertexSerialize(VertexData, VertexIndex++));

                int EdgeIndex = 1;
                foreach (var (_, EdgeData) in data.DictionaryEdges) Writer.Write(EdgeSerialize(EdgeData, EdgeIndex++));
            });
    }

    public async Task<GraphData> DeserializeAsync(string path)
    {
        var GraphData = new GraphData();

        var TaskDeserialize = Task.Run(
            () =>
            {
                var File = new FileInfo(path);

                if (!File.Exists) return null;

                using var Reader = new StreamReader(File.FullName);

                var ListSectionsData = new List<SectionData>();
                var SectionData      = (SectionData?)null;

                while (!Reader.EndOfStream)
                {
                    string? Line = Reader.ReadLine();
                    if (string.IsNullOrEmpty(Line)) continue;
                    if (TryParseSection(Line, out string SectionName, out int SectionIndex))
                    {
                        SectionData = new SectionData(SectionName, SectionIndex, new());
                        ListSectionsData.Add(SectionData);
                    }
                    else if (SectionData is not null
                        && TryParseValue(Line, out string PropertyName, out string PropertyValue))
                    {
                        SectionData.Properties.Add(PropertyName, PropertyValue);
                    }
                }

                return ListSectionsData;
            });

        var ListSectionsData = await TaskDeserialize;

        if (ListSectionsData != null)
            foreach (var SectionData in ListSectionsData)
                TryResolveSection(SectionData, GraphData);

        return GraphData;
    }


    private static string VertexSerialize(VertexData vertex, int index)
    {
        return $"[{c_VertexSectionName}{index}]\n"
            + $"{c_VertexIdPropertyName}={vertex.Id}\n"
            + $"{c_VertexNamePropertyName}={vertex.TextString}\n"
            + $"{c_VertexPositionPropertyName}={vertex.Position.X:F6};{vertex.Position.Y:F6}\n";
    }

    private static string EdgeSerialize(EdgeData edge, int index)
    {
        return $"[{c_EdgeSectionName}{index}]\n"
            + $"{c_EdgeFromPropertyName}={edge.FromId}\n"
            + $"{c_EdgeToPropertyName}={edge.ToId}\n"
            + $"{c_EdgeValuePropertyName}={edge.Value}\n";
    }

    private static bool TryParseSection(string line, out string sectionName, out int sectionIndex)
    {
        sectionName  = string.Empty;
        sectionIndex = 0;
        if (!Regex.IsMatch(line, @"\s*\[[a-zA-Z]+\d*\]\s*")) return false;

        var Result = Regex.Match(line, @"\s*\[([a-zA-Z]+)(\d*)\]\s*");
        sectionName  = Result.Groups[1].Value;
        sectionIndex = int.Parse(Result.Groups[2].Value);

        return true;
    }

    private static bool TryParseValue(string line, out string propertyName, out string propertyValue)
    {
        propertyName  = string.Empty;
        propertyValue = string.Empty;
        if (!Regex.IsMatch(line, @"\s*[a-zA-Z]+\s*=\s*[\w-;]+\s*")) return false;

        var Result = Regex.Match(line, @"\s*([a-zA-Z]+)\s*=\s*([\w-;,]+)\s*");
        propertyName  = Result.Groups[1].Value;
        propertyValue = Result.Groups[2].Value;

        return true;
    }

    private static bool TryResolveSection(SectionData? section, GraphData graph)
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));

        if (section == null) return false;

        switch (section.Name)
        {
            case c_VertexSectionName:
                if (!TryResolveVertex(section, out var Vertex)) break;
                graph.AddVertex(Vertex!);
                return true;

            case c_EdgeSectionName:
                if (!TryResolveEdge(section, graph, out var Edge)) break;
                graph.AddEdge(Edge!);
                return true;
        }

        return false;
    }

    private static bool TryResolveVertex(SectionData section, out VertexData? vertex)
    {
        vertex = null;
        if (!section.Properties.TryGetValue(c_VertexIdPropertyName, out string? IdString)
            || !Guid.TryParse(IdString, out var Id)) return false;

        vertex = new VertexData(Id);

        if (section.Properties.TryGetValue(c_VertexNamePropertyName, out string? NameString))
            vertex.TextString                              = NameString;
        else if (section.Index.HasValue) vertex.TextString = section.Index.Value.ToString();

        if (section.Properties.TryGetValue(c_VertexPositionPropertyName, out string? PositionString)
            && Regex.IsMatch(PositionString, @"\d+;\d+"))
        {
            var    Result          = Regex.Match(PositionString, @"(\d+,\d+);(\d+,\d+)");
            string XPositionString = Result.Groups[1].Value;
            string YPositionString = Result.Groups[2].Value;

            if (double.TryParse(XPositionString,    out double XPosition)
                && double.TryParse(YPositionString, out double YPosition))
            {
                vertex.Position = new Point(XPosition, YPosition);
            }
        }

        return true;
    }

    private static bool TryResolveEdge(SectionData section, GraphData graph, out EdgeData? edge)
    {
        edge = null;
        if (!section.Properties.TryGetValue(c_EdgeFromPropertyName, out string? FromString)
            || !Guid.TryParse(FromString, out var FromId)) return false;
        if (!section.Properties.TryGetValue(c_EdgeToPropertyName, out string? ToString)
            || !Guid.TryParse(ToString, out var ToId)) return false;

        if (!graph.DictionaryVertexes.TryGetValue(FromId, out var FromVertex)) return false;
        if (!graph.DictionaryVertexes.TryGetValue(ToId,   out var ToVertex)) return false;

        var PathVertex = (From: FromVertex, To: ToVertex);

        edge = new EdgeData(PathVertex);

        if (section.Properties.TryGetValue(c_EdgeValuePropertyName, out string? ValueString)
            && double.TryParse(ValueString, out double Value))
            edge.Value = Value;

        return true;
    }


    private record SectionData(string Name, int? Index, Dictionary<string, string> Properties);
}