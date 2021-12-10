using System.Windows;

using GraphApp.Core.ViewModels.Windows;
using GraphApp.Core.Views.Windows;


namespace GraphApp.WPF.Views.Windows;

/// <summary>
/// Interaction logic for ComparableAlgorithmsWindowView.xaml
/// </summary>
public partial class ComparableAlgorithmsWindowView : Window, IComparableAlgorithmsWindowView
{
    public IComparableAlgorithmsWindowViewModel? ViewModel
    {
        get => GetValue(DataContextProperty) as IComparableAlgorithmsWindowViewModel;
        set => SetValue(DataContextProperty, value);
    }


    public ComparableAlgorithmsWindowView()
    {
        InitializeComponent();
    }


    public void Dispose()
    {
    }
}