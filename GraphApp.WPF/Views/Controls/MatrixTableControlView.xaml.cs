using System.Windows.Controls;

using GraphApp.Core.ViewModels.Controls;
using GraphApp.Core.Views.Controls;


namespace GraphApp.WPF.Views.Controls;

/// <summary>
/// Interaction logic for MatrixTableControlView.xaml
/// </summary>
public partial class MatrixTableControlView : UserControl, IMatrixTableControlView
{
    public IMatrixTableControlViewModel? ViewModel
    {
        get => GetValue(DataContextProperty) as IMatrixTableControlViewModel;
        set => SetValue(DataContextProperty, value);
    }


    public MatrixTableControlView()
    {
        InitializeComponent();
    }


    public void Dispose()
    {
    }
}