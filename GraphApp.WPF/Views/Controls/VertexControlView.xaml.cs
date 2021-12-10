using System.Windows;

using GraphApp.Core.ViewModels.Controls;
using GraphApp.Core.Views.Controls;

using System.Windows.Controls;
using System.Windows.Input;


namespace GraphApp.WPF.Views.Controls;

/// <summary>
/// Interaction logic for VertexControlView.xaml
/// </summary>
public partial class VertexControlView : UserControl, IVertexControlView
{
    public IVertexControlViewModel? ViewModel
    {
        get => GetValue(DataContextProperty) as IVertexControlViewModel;
        set => SetValue(DataContextProperty, value);
    }

    public GraphControlView? ParentView { get; set; }


    public VertexControlView()
    {
        InitializeComponent();
    }


    public void Dispose()
    {
    }


    #region Mouse

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        ParentView?.UpdateMouseUpDownDataHandler(e, MouseActionUpDownHandler);
    }

    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
        ParentView?.UpdateMouseUpDownDataHandler(e, MouseActionUpDownHandler);
    }

    private bool MouseActionUpDownHandler()
    {
        if (ParentView?.LastMouseUpDown?.ChangedButton == MouseButton.Left)
        {
            if (ParentView?.LastMouseUpDown?.ButtonState      == MouseButtonState.Pressed
                && ParentView?.LastMouseData?.LeftButtonState == MouseButtonState.Pressed)
            {
                if ((Keyboard.GetKeyStates(Key.LeftCtrl) & KeyStates.Down) > 0)
                    ParentView!.SelectPairVertex(this);
                else
                    ParentView!.SelectVertex(this);

                if (ParentView!.CapturedVertex != this) ParentView!.CaptureVertex(this);
            }
            else if (ParentView?.LastMouseUpDown?.ButtonState == MouseButtonState.Released
                && ParentView?.LastMouseData?.LeftButtonState == MouseButtonState.Released)
            {
                if (ParentView!.CapturedVertex == this) ParentView.CaptureVertex(null);
            }

            return true;
        }

        if (ParentView?.LastMouseUpDown?.ChangedButton     == MouseButton.Right
            && ParentView?.LastMouseUpDown?.ButtonState    == MouseButtonState.Released
            && ParentView?.LastMouseData?.RightButtonState == MouseButtonState.Released)
        {
            ShowVertexContextMenu();
            return true;
        }

        return false;
    }

    #endregion


    #region ContextMenu

    private void ShowVertexContextMenu()
    {
        ConnectToButton.IsEnabled = ParentView?.SelectedVertex != this;

        if (TextBlockControl.ContextMenu is not null) TextBlockControl.ContextMenu.IsOpen = true;
    }

    private void ConnectToHandler(object sender, RoutedEventArgs e)
    {
        var From    = ParentView?.SelectedVertex?.ViewModel?.Id;
        var To      = ViewModel?.Id;
        var Command = ParentView?.ViewModel?.AddEdgeCommand;

        if (From is null || To is null || Command is null) return;

        var Path = (From: From.Value, To: To.Value);

        if (!Command.CanExecute(Path)) return;

        Command!.Execute(Path);
    }

    private void RemoveVertexHandler(object sender, RoutedEventArgs e)
    {
        if (ParentView!.CapturedVertex == this) ParentView.CaptureVertex(null);
        if (ParentView!.SelectedVertex == this) ParentView.SelectVertex(null);
        if (ParentView!.SelectedFromVertex == this)
        {
            var Temp = ParentView.SelectedToVertex;
            ParentView.SelectPairVertex(null);
            ParentView.SelectPairVertex(Temp);
        }

        if (ParentView!.SelectedToVertex == this)
        {
            var Temp = ParentView.SelectedFromVertex;
            ParentView.SelectPairVertex(null);
            ParentView.SelectPairVertex(Temp);
        }

        var Id      = ViewModel?.Id;
        var Command = ParentView?.ViewModel?.RemoveVertexCommand;

        if (!Command?.CanExecute(Id) ?? true) return;

        Command!.Execute(Id);
    }

    #endregion
}