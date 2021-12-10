using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

using GraphApp.Core.Data;
using GraphApp.Core.Helper;
using GraphApp.Core.Models;
using GraphApp.Core.Models.Graphs;
using GraphApp.Core.Services;
using GraphApp.Core.ViewModels.Controls;
using GraphApp.Core.Views.Controls;

using Ninject;
using Ninject.Parameters;

using Prism.Commands;


namespace GraphApp.WPF.ViewModels.Controls;

internal class GraphControlViewModel : ControlViewModelBase, IGraphControlViewModel
{
    private readonly Graph m_Graph;

    private readonly Dictionary<Guid, (IVertexControlViewModel ViewModel, IVertexControlView View)>             m_DictionaryVertexes;
    private readonly Dictionary<(Guid From, Guid To), (IEdgeControlViewModel ViewModel, IEdgeControlView View)> m_DictionaryEdges;
    private readonly ObservableCollection<IVertexControlView>                                                   m_Vertexes;
    private readonly ObservableCollection<IEdgeControlView>                                                     m_Edges;

    private Guid? m_SelectedFirstVertex;
    private Guid? m_SelectedSecondVertex;

    private List<Vertex>? m_FoundPath;


    public ReadOnlyObservableCollection<IVertexControlView> Vertexes { get; }
    public ReadOnlyObservableCollection<IEdgeControlView>   Edges    { get; }

    public IList<IVertexControlView> FoundPathVertices { get; }
    public IList<IEdgeControlView>   FoundPathEdges    { get; }

    public Guid? SelectedFromVertex
    {
        get => m_SelectedFirstVertex;
        set => SetProperty(ref m_SelectedFirstVertex, value, ClearFoundPath);
    }

    public Guid? SelectedToVertex
    {
        get => m_SelectedSecondVertex;
        set => SetProperty(ref m_SelectedSecondVertex, value, ClearFoundPath);
    }

    public List<Vertex>? FoundPath
    {
        get => m_FoundPath;
        set => SetProperty(ref m_FoundPath, value, UpdateFoundPath);
    }

    public double Height { get; set; }
    public double Width  { get; set; }

    public ICommand AddVertexCommand    { get; }
    public ICommand RemoveVertexCommand { get; }
    public ICommand AddEdgeCommand      { get; }
    public ICommand RemoveEdgeCommand   { get; }


    public GraphControlViewModel(IBusinessLogic businessLogic, Graph graph) : base(businessLogic)
    {
        m_Graph = graph;

        m_DictionaryVertexes = new();
        m_DictionaryEdges    = new();
        m_Vertexes           = new();
        m_Edges              = new();
        Vertexes             = new(m_Vertexes);
        Edges                = new(m_Edges);

        FoundPathVertices = new List<IVertexControlView>();
        FoundPathEdges    = new List<IEdgeControlView>();

        m_SelectedFirstVertex  = null;
        m_SelectedSecondVertex = null;

        AddVertexCommand    = new DelegateCommand<Point?>(AddVertexCommandHandler);
        RemoveVertexCommand = new DelegateCommand<Guid?>(RemoveVertexCommandHandler);
        AddEdgeCommand      = new DelegateCommand<(Guid From, Guid To)?>(AddEdgeCommandHandler);
        RemoveEdgeCommand   = new DelegateCommand<(Guid From, Guid To)?>(RemoveEdgeCommandHandler);

        if (m_Graph.Vertices is INotifyCollectionChanged VertexesCollection)
            VertexesCollection.CollectionChanged += VertexesCollectionChangedHandler;
        if (m_Graph.Edges is INotifyCollectionChanged EdgesCollection)
            EdgesCollection.CollectionChanged += EdgesCollectionChangedHandler;
    }


    #region Vertex

    private void AddVertexCommandHandler(Point? center)
    {
        var Vertex = m_Graph.AddVertex();

        if (center is not null) Vertex.Data.Center = center.Value;
    }

    private void RemoveVertexCommandHandler(Guid? id)
    {
        if (id is null) return;

        m_Graph.RemoveVertex(id.Value);
    }

    private void VertexesCollectionChangedHandler(object? sender, NotifyCollectionChangedEventArgs e)
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

        ClearFoundPath();
    }

    private void AddVertexHelper(Vertex vertex)
    {
        var VertexItem = CreateVertexItem(vertex);

        m_Vertexes.Add(VertexItem.View);

        m_DictionaryVertexes.Add(vertex.Data.Id, VertexItem);
    }

    private bool RemoveVertexHelper(Vertex vertex)
    {
        if (!m_DictionaryVertexes.Remove(vertex.Data.Id, out var VertexItem)) return false;

        m_Vertexes.Remove(VertexItem.View);

        return true;
    }

    private void ClearVertexesHelper()
    {
        ClearEdgesHelper();
        m_DictionaryVertexes.Clear();
        m_Vertexes.Clear();
    }

    private (IVertexControlViewModel ViewModel, IVertexControlView View) CreateVertexItem(Vertex vertex)
    {
        var Parameter  = new ConstructorArgument("vertex", vertex);
        var ViewModels = BusinessLogic.IoC.Get<IVertexControlViewModel>(Parameter);
        var View       = BusinessLogic.IoC.Get<IVertexControlView>();

        View.ViewModel = ViewModels;

        return (ViewModels, View);
    }

    public void ResetVertexPosition()
    {
        int VertexCount = m_DictionaryVertexes.Count;

        if (VertexCount == 0) return;

        var    CenterScreen = new Vector(Width / 2, Height / 2);
        double VertexSize   = m_DictionaryVertexes.Max(pair => pair.Value.ViewModel.Size);

        double Radius = Math.Min(
            (VertexCount - 1) * VertexSize,
            Math.Min(Height / 2, Width / 2) - VertexSize / 2);

        // ReSharper disable once PossibleLossOfFraction
        double Angle = 360 / m_DictionaryVertexes.Count;

        double ThisAngle = 0;

        foreach (var (_, (VertexViewModel, _)) in m_DictionaryVertexes)
        {
            var MatrixRotate = TrigonometricHelper.CreateRotationMatrix(ThisAngle);

            VertexViewModel.Center = (Point)(CenterScreen + MatrixRotate.Transform(new Vector(Radius, 0)));

            ThisAngle += Angle;
        }
    }

    #endregion


    #region Edge

    private void AddEdgeCommandHandler((Guid From, Guid To)? path)
    {
        if (path is null) return;

        m_Graph.AddEdge(path.Value);
    }

    private void RemoveEdgeCommandHandler((Guid From, Guid To)? path)
    {
        if (path is null) return;

        m_Graph.RemoveEdge(path.Value);
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

        ClearFoundPath();
    }

    private void AddEdgeHelper(Edge edge)
    {
        var EdgeItem = CreateEdgeItem(edge);

        m_Edges.Add(EdgeItem.View);

        m_DictionaryEdges.Add(edge.Data.PathId, EdgeItem);
    }

    private bool RemoveEdgeHelper(Edge edge)
    {
        if (!m_DictionaryEdges.Remove(edge.Data.PathId, out var EdgeItem)) return false;

        m_Edges.Remove(EdgeItem.View);

        return true;
    }

    private void ClearEdgesHelper()
    {
        m_DictionaryEdges.Clear();
        m_Edges.Clear();
    }

    private (IEdgeControlViewModel ViewModel, IEdgeControlView View) CreateEdgeItem(Edge edge)
    {
        var Parameter  = new ConstructorArgument("edge", edge);
        var ViewModels = BusinessLogic.IoC.Get<IEdgeControlViewModel>(Parameter);
        var View       = BusinessLogic.IoC.Get<IEdgeControlView>();

        View.ViewModel = ViewModels;

        return (ViewModels, View);
    }

    #endregion

    private void ClearFoundPath()
    {
        FoundPath = null;
        FoundPathVertices.Clear();
        FoundPathEdges.Clear();

        RaisePropertyChanged(nameof(FoundPathVertices));
        RaisePropertyChanged(nameof(FoundPathEdges));
    }

    private void UpdateFoundPath()
    {
        FoundPathVertices.Clear();
        FoundPathEdges.Clear();

        if (FoundPath is not null)
        {
            for (int i = 0; i < FoundPath.Count; i++)
            {
                FoundPathVertices.Add(m_DictionaryVertexes[FoundPath[i].Data.Id].View);
                if (i + 1 == FoundPath.Count) break;
                FoundPathEdges.Add(m_DictionaryEdges[(FoundPath[i].Data.Id, FoundPath[i + 1].Data.Id)].View);
            }
        }

        RaisePropertyChanged(nameof(FoundPathVertices));
        RaisePropertyChanged(nameof(FoundPathEdges));
    }
}