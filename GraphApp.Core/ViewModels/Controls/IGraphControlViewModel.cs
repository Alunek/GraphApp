using System.Collections.ObjectModel;
using System.Windows.Input;

using GraphApp.Core.Data;
using GraphApp.Core.Models;
using GraphApp.Core.Models.Graphs;
using GraphApp.Core.Views.Controls;


namespace GraphApp.Core.ViewModels.Controls;

public interface IGraphControlViewModel : IControlViewModel
{
    ReadOnlyObservableCollection<IVertexControlView> Vertexes { get; }
    ReadOnlyObservableCollection<IEdgeControlView>   Edges    { get; }

    IList<IVertexControlView> FoundPathVertices { get; }
    IList<IEdgeControlView>   FoundPathEdges    { get; }

    Guid? SelectedFromVertex { get; set; }
    Guid? SelectedToVertex   { get; set; }

    double Height { get; set; }
    double Width  { get; set; }

    ICommand      AddVertexCommand    { get; }
    ICommand      RemoveVertexCommand { get; }
    ICommand      AddEdgeCommand      { get; }
    ICommand      RemoveEdgeCommand   { get; }
    List<Vertex>? FoundPath           { get; set; }


    void ResetVertexPosition();
}