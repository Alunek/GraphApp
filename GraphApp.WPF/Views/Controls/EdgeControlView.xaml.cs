using System.Windows;

using GraphApp.Core.ViewModels.Controls;
using GraphApp.Core.Views.Controls;

using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;


namespace GraphApp.WPF.Views.Controls;

/// <summary>
/// Interaction logic for EdgeControlView.xaml
/// </summary>
public partial class EdgeControlView : UserControl, IEdgeControlView
{
    public IEdgeControlViewModel? ViewModel
    {
        get => GetValue(DataContextProperty) as IEdgeControlViewModel;
        set => SetValue(DataContextProperty, value);
    }

    public GraphControlView? ParentView { get; set; }


    public EdgeControlView()
    {
        InitializeComponent();

        DataContextChanged += DataContextChangedHandler;
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
        if (ParentView?.LastMouseUpDown?.ChangedButton     == MouseButton.Right
            && ParentView?.LastMouseUpDown?.ButtonState    == MouseButtonState.Released
            && ParentView?.LastMouseData?.RightButtonState == MouseButtonState.Released)
        {
            ShowEdgeContextMenu();
            return true;
        }

        return false;
    }

    #endregion

    #region ContextMenu

    private void ShowEdgeContextMenu()
    {
        if (TextBlockControl.ContextMenu is not null) TextBlockControl.ContextMenu.IsOpen = true;
    }

    private void RemoveEdgeHandler(object sender, RoutedEventArgs e)
    {
        var PathId  = ViewModel?.PathId;
        var Command = ParentView?.ViewModel?.RemoveEdgeCommand;

        if (!Command?.CanExecute(PathId) ?? true) return;

        Command!.Execute(PathId);
    }

    #endregion

    private void DataContextChangedHandler(object sender, DependencyPropertyChangedEventArgs e)
    {
        UpdateTextSize();
    }

    public void TextBlockSizeChangedHandler(object? sender, SizeChangedEventArgs e)
    {
        UpdateTextSize();
    }

    private void UpdateTextSize()
    {
        if (ViewModel is null) return;

        ViewModel.TextHeight = TextBlockControl.ActualHeight;
        ViewModel.TextWidth  = TextBlockControl.ActualWidth;
    }

    private void TextBlockMouseDownHandler(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed || e.ClickCount != 2) return;

        TextBoxControl.Text = TextBlockControl.Text;

        TextBlockControl.Visibility = Visibility.Collapsed;
        TextBoxControl.Visibility   = Visibility.Visible;

        Dispatcher.BeginInvoke(() => Keyboard.Focus(TextBoxControl), DispatcherPriority.Render);
    }

    private void TextBoxLostFocusHandler(object sender, RoutedEventArgs e)
    {
        UpdateTextBoxFocus();
    }

    private void TextBoxKeyDownHandler(object sender, KeyEventArgs e)
    {
        if (e.Key is Key.Escape) UpdateTextBoxFocus();
        if (e.Key is Key.Enter) UpdateTextBoxFocus(true);
    }

    private void UpdateTextBoxFocus(bool apply = false)
    {
        if (apply
            && ViewModel is not null
            && double.TryParse(TextBoxControl.Text, out double Value))
            ViewModel.Value = Value;

        TextBlockControl.Visibility = Visibility.Visible;
        TextBoxControl.Visibility   = Visibility.Collapsed;
    }
}