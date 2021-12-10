using System.Data;
using System.Windows;

using GraphApp.Core.ViewModels.Controls;
using GraphApp.Core.Views.Controls;

using System.Windows.Controls;


namespace GraphApp.WPF.Views.Controls;

/// <summary>
/// Interaction logic for MatrixPageControlView.xaml
/// </summary>
public partial class MatrixPageControlView : UserControl, IMatrixPageControlView
{
    public IMatrixPageControlViewModel? ViewModel
    {
        get => GetValue(DataContextProperty) as IMatrixPageControlViewModel;
        set => SetValue(DataContextProperty, value);
    }


    public MatrixPageControlView()
    {
        InitializeComponent();

        DataContextChanged += DataContextChangedHandler;
    }


    public void Dispose()
    {
    }

    private void DataContextChangedHandler(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is not IMatrixPageControlViewModel { MatrixTableView: UserControl MatrixTable }) return;

        if (MatrixTable.Parent is Grid ParentGrid) {
            ParentGrid.Children.Remove(MatrixTable);
        }

        MatrixControl.Content = MatrixTable;
    }
}