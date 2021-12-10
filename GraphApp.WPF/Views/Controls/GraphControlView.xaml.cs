using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

using GraphApp.Core.ViewModels.Controls;
using GraphApp.Core.Views.Controls;

using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using GraphApp.Core.Data;
using GraphApp.Core.Data.Graphs;
using GraphApp.Core.Models.Graphs;


namespace GraphApp.WPF.Views.Controls;

/// <summary>
/// Interaction logic for GraphControlView.xaml
/// </summary>
public partial class GraphControlView : UserControl, IGraphControlView
{
    public const int MaxVertexCount = 20;

    private static readonly ElementStyle s_DefaultElementStyle = new();

    private static readonly ElementStyle s_SelectedVertexStyle = new(false)
    {
        TextWeight = FontWeight.FromOpenTypeWeight(800),
        BorderSize = 2.0d
    };

    private static readonly ElementStyle s_PathVertexStyle = new(false)
    {
        BackgroundColor = new SolidColorBrush(Colors.LightBlue)
    };

    private static readonly ElementStyle s_PathEdgeStyle = new(false)
    {
        BorderColor = new SolidColorBrush(Colors.DarkBlue),
        TextColor = new SolidColorBrush(Colors.DarkBlue)
    };

    private static readonly ElementStyle s_SelectedPairFirstVertexStyle = new(false)
    {
        BackgroundColor = new SolidColorBrush(Colors.IndianRed)
    };

    private static readonly ElementStyle s_SelectedPairSecondVertexStyle = new(false)
    {
        BackgroundColor = new SolidColorBrush(Colors.DarkRed)
    };


    private IList<VertexControlView>? m_FoundPathVertices;
    private IList<EdgeControlView>?   m_FoundPathEdges;


    public IGraphControlViewModel? ViewModel
    {
        get => GetValue(DataContextProperty) as IGraphControlViewModel;
        set => SetValue(DataContextProperty, value);
    }

    public VertexControlView? CapturedVertex     { get; private set; }
    public VertexControlView? SelectedVertex     { get; private set; }
    public VertexControlView? SelectedFromVertex { get; private set; }
    public VertexControlView? SelectedToVertex   { get; private set; }


    public GraphControlView()
    {
        InitializeComponent();

        CapturedVertex = null;
        SelectedVertex = null;

        DataContextChanged += DataContextChangedHandler;
    }


    public void Dispose()
    {
    }

    public void CaptureVertex(VertexControlView? vertexControl)
    {
        var OldSelectedVertex = CapturedVertex;
        var NewSelectedVertex = vertexControl;

        if (NewSelectedVertex == OldSelectedVertex) return;

        CapturedVertex = NewSelectedVertex;

        if (OldSelectedVertex?.DataContext is IVertexControlViewModel OldViewModel) OldViewModel.ZIndex = 0;
        if (NewSelectedVertex?.DataContext is IVertexControlViewModel NewViewModel) NewViewModel.ZIndex = 1;
    }

    public void SelectVertex(VertexControlView? vertexControl)
    {
        var OldSelectedVertex = SelectedVertex;
        var NewSelectedVertex = vertexControl;

        if (NewSelectedVertex == OldSelectedVertex) return;

        SelectedVertex = NewSelectedVertex;

        UpdateVertexStyle(OldSelectedVertex);
        UpdateVertexStyle(NewSelectedVertex);
    }

    public void SelectPairVertex(VertexControlView? vertexControl)
    {
        var OldSelectedVertex  = SelectedFromVertex;
        var TempSelectedVertex = SelectedToVertex;
        var NewSelectedVertex  = vertexControl;

        if (NewSelectedVertex is not null
            && NewSelectedVertex == TempSelectedVertex) return;

        SelectedFromVertex = TempSelectedVertex;
        SelectedToVertex   = NewSelectedVertex;

        if (ViewModel is not null)
        {
            ViewModel.SelectedFromVertex = TempSelectedVertex?.ViewModel?.Id;
            ViewModel.SelectedToVertex   = NewSelectedVertex?.ViewModel?.Id;
        }

        UpdateVertexStyle(OldSelectedVertex);
        UpdateVertexStyle(TempSelectedVertex);
        UpdateVertexStyle(NewSelectedVertex);
    }


    private void UpdateVertexStyle(VertexControlView? vertexControl)
    {
        if (vertexControl is null) return;

        var SelectedStyle = vertexControl == SelectedVertex
            ? s_SelectedVertexStyle
            : null;

        var PairStyle = vertexControl == SelectedFromVertex
            ? s_SelectedPairFirstVertexStyle
            : vertexControl == SelectedToVertex
                ? s_SelectedPairSecondVertexStyle
                : m_FoundPathVertices?.Contains(vertexControl) ?? false
                    ? s_PathVertexStyle
                    : null;

        vertexControl.ViewModel?.SetStyle(ElementStyle.CombineOrDefault(SelectedStyle, PairStyle));
    }

    private void UpdateEdgeStyle(EdgeControlView? edgeControl)
    {
        if (edgeControl is null) return;

        var PathStyle = m_FoundPathEdges?.Contains(edgeControl) ?? false
            ? s_PathEdgeStyle
            : null;

        edgeControl.ViewModel?.SetStyle(ElementStyle.CombineOrDefault(PathStyle, null));
    }

    private void DataContextChangedHandler(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is IGraphControlViewModel OldViewModel)
        {
            if (OldViewModel.Vertexes is INotifyCollectionChanged VertexesCollection)
                VertexesCollection.CollectionChanged -= VertexesCollectionChangedHandler;
            if (OldViewModel.Edges is INotifyCollectionChanged EdgesCollection)
                EdgesCollection.CollectionChanged -= EdgesCollectionChangedHandler;

            OldViewModel.PropertyChanged -= ViewModelPropertyChangedHandler;
        }

        if (e.NewValue is IGraphControlViewModel NewViewModel)
        {
            if (NewViewModel.Vertexes is INotifyCollectionChanged VertexesCollection)
                VertexesCollection.CollectionChanged += VertexesCollectionChangedHandler;
            if (NewViewModel.Edges is INotifyCollectionChanged EdgesCollection)
                EdgesCollection.CollectionChanged += EdgesCollectionChangedHandler;

            NewViewModel.PropertyChanged += ViewModelPropertyChangedHandler;
        }

        UpdateSize();
    }

    private void ViewModelPropertyChangedHandler(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(IGraphControlViewModel.FoundPathVertices):
                UpdateFoundPathVertices();
                break;

            case nameof(IGraphControlViewModel.FoundPathEdges):
                UpdateFoundPathEdges();
                break;
        }
    }

    private void UpdateFoundPathVertices()
    {
        var NewVertices = ViewModel!.FoundPathVertices?.Cast<VertexControlView>().ToList();
        var OldVertices = m_FoundPathVertices;
        m_FoundPathVertices = NewVertices;

        if (OldVertices is not null)
            foreach (var Vertex in OldVertices)
                UpdateVertexStyle(Vertex);
        if (NewVertices is not null)
            foreach (var Vertex in NewVertices)
                UpdateVertexStyle(Vertex);
    }

    private void UpdateFoundPathEdges()
    {
        var NewEdges = ViewModel!.FoundPathEdges?.Cast<EdgeControlView>().ToList();
        var OldEdges = m_FoundPathEdges;
        m_FoundPathEdges = NewEdges;

        if (OldEdges is not null)
            foreach (var Edge in OldEdges)
                UpdateEdgeStyle(Edge);
        if (NewEdges is not null)
            foreach (var Edge in NewEdges)
                UpdateEdgeStyle(Edge);
    }

    private void VertexesCollectionChangedHandler(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                foreach (VertexControlView Item in e.NewItems!) Item.ParentView = this;
                break;

            case NotifyCollectionChangedAction.Remove:
                foreach (VertexControlView Item in e.OldItems!) Item.ParentView = null;
                break;

            case NotifyCollectionChangedAction.Replace:
                foreach (VertexControlView Item in e.OldItems!) Item.ParentView = null;
                foreach (VertexControlView Item in e.NewItems!) Item.ParentView = this;
                break;

            case NotifyCollectionChangedAction.Move:
                break;

            case NotifyCollectionChangedAction.Reset:
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        AddVertexButton.IsEnabled = ViewModel?.Vertexes.Count < MaxVertexCount;
    }

    private void EdgesCollectionChangedHandler(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                foreach (EdgeControlView Item in e.NewItems!) Item.ParentView = this;
                break;

            case NotifyCollectionChangedAction.Remove:
                foreach (EdgeControlView Item in e.OldItems!) Item.ParentView = null;
                break;

            case NotifyCollectionChangedAction.Replace:
                foreach (EdgeControlView Item in e.OldItems!) Item.ParentView = null;
                foreach (EdgeControlView Item in e.NewItems!) Item.ParentView = this;
                break;

            case NotifyCollectionChangedAction.Move:
                break;

            case NotifyCollectionChangedAction.Reset:
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    #region Mouse

    public MouseData?       LastMouseData   { get; private set; }
    public MouseButtonData? LastMouseUpDown { get; private set; }
    public MouseWheelData?  LastMouseWheel  { get; private set; }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        UpdateMouseUpDownDataHandler(e, MouseActionUpDownHandler);
    }

    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
        UpdateMouseUpDownDataHandler(e, MouseActionUpDownHandler);
    }

    protected override void OnMouseEnter(MouseEventArgs e)
    {
        UpdateMouseDataHandler(e, MouseActionHandler);
    }

    protected override void OnMouseLeave(MouseEventArgs e)
    {
        UpdateMouseDataHandler(e, MouseActionHandler);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        UpdateMouseDataHandler(e, MouseActionHandler);
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        UpdateMouseWheelHandler(e, MouseActionWheelHandler);
    }

    public void UpdateMouseDataHandler(MouseEventArgs e, Func<bool> handler)
    {
        if (CanvasControl is null) return;

        LastMouseData = new MouseData(e.GetPosition(CanvasControl), e.LeftButton, e.MiddleButton, e.RightButton);

        e.Handled = handler();
    }

    public void UpdateMouseUpDownDataHandler(MouseButtonEventArgs e, Func<bool> handler)
    {
        if (CanvasControl is null) return;

        var Position = e.GetPosition(CanvasControl);
        LastMouseData   = new MouseData(Position, e.LeftButton, e.MiddleButton, e.RightButton);
        LastMouseUpDown = new MouseButtonData(Position, e.ButtonState, e.ChangedButton, e.ClickCount);

        e.Handled = handler();
    }

    private void UpdateMouseWheelHandler(MouseWheelEventArgs e, Func<bool> handler)
    {
        if (CanvasControl is null) return;

        LastMouseData  = new MouseData(e.GetPosition(CanvasControl), e.LeftButton, e.MiddleButton, e.RightButton);
        LastMouseWheel = new MouseWheelData(e.Delta);

        e.Handled = handler();
    }

    private bool MouseActionHandler()
    {
        if (LastMouseUpDown?.ChangedButton    == MouseButton.Left
            && LastMouseUpDown?.ButtonState   == MouseButtonState.Pressed
            && LastMouseData?.LeftButtonState == MouseButtonState.Pressed)
        {
            if (CapturedVertex?.ViewModel is not null)
            {
                CapturedVertex.ViewModel.Center = LastMouseData.Position;
            }
            else
            {
                var StartPosition = LastMouseUpDown.Position;
                var EndPosition   = LastMouseData.Position;
                var Transform     = (MatrixTransform)CanvasControl.RenderTransform;
                var Matrix        = Transform.Matrix;

                Matrix.TranslatePrepend(EndPosition.X - StartPosition.X, EndPosition.Y - StartPosition.Y);
                Transform.Matrix = Matrix;
            }

            return true;
        }

        return false;
    }

    private bool MouseActionUpDownHandler()
    {
        if (LastMouseUpDown?.ChangedButton    == MouseButton.Left
            && LastMouseUpDown?.ButtonState   == MouseButtonState.Pressed
            && LastMouseData?.LeftButtonState == MouseButtonState.Pressed
            && LastMouseUpDown?.ClickCount    == 1)
        {
            if ((Keyboard.GetKeyStates(Key.LeftCtrl) & KeyStates.Down) > 0)
            {
                SelectPairVertex(null);
                SelectPairVertex(null);
            }
            else
            {
                SelectVertex(null);
            }

            return true;
        }

        if (LastMouseUpDown?.ChangedButton     == MouseButton.Right
            && LastMouseUpDown?.ButtonState    == MouseButtonState.Released
            && LastMouseData?.RightButtonState == MouseButtonState.Released)
        {
            ShowAreaContextMenu();
            return true;
        }

        if (LastMouseUpDown?.ChangedButton      == MouseButton.Middle
            && LastMouseUpDown?.ButtonState     == MouseButtonState.Released
            && LastMouseData?.MiddleButtonState == MouseButtonState.Released)
        {
            var Transform = (MatrixTransform)CanvasControl.RenderTransform;

            ViewModel?.ResetVertexPosition();

            Transform.Matrix = Matrix.Identity;
            return true;
        }

        return false;
    }

    private bool MouseActionWheelHandler()
    {
        if (LastMouseData is null
            || LastMouseWheel is null) return false;

        var    Position  = LastMouseData.Position;
        var    Transform = (MatrixTransform)CanvasControl.RenderTransform;
        var    Matrix    = Transform.Matrix;
        double Scale     = LastMouseWheel.Delta >= 0 ? 1.1 : 1.0 / 1.1;

        Matrix.ScaleAtPrepend(Scale, Scale, Position.X, Position.Y);
        Transform.Matrix = Matrix;

        return true;
    }

    public record MouseData(Point Position, MouseButtonState LeftButtonState, MouseButtonState MiddleButtonState, MouseButtonState RightButtonState);

    public record MouseButtonData(Point Position, MouseButtonState ButtonState, MouseButton ChangedButton, int ClickCount);

    public record MouseWheelData(int Delta);

    #endregion

    #region ContextMenu

    private void ShowAreaContextMenu()
    {
        if (CanvasControl.ContextMenu is not null) CanvasControl.ContextMenu.IsOpen = true;
    }

    private void AddVertexClickHandler(object sender, RoutedEventArgs e)
    {
        var Position = LastMouseUpDown?.Position;

        if (!ViewModel?.AddVertexCommand.CanExecute(Position) ?? true) return;

        ViewModel!.AddVertexCommand.Execute(Position);
    }

    #endregion

    private void SizeChangedHandler(object sender, SizeChangedEventArgs e)
    {
        UpdateSize();
    }

    private void UpdateSize()
    {
        if (ViewModel is null) return;

        ViewModel.Height = ActualHeight;
        ViewModel.Width  = ActualWidth;
    }
}