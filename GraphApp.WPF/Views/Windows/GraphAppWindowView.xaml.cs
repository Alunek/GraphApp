using GraphApp.Core.ViewModels.Windows;
using GraphApp.Core.Views.Windows;

using System.Windows;

using Microsoft.Win32;

using System.Windows.Input;


namespace GraphApp.WPF.Views.Windows;

/// <summary>
/// Interaction logic for GraphAppWindowView.xaml
/// </summary>
public partial class GraphAppWindowView : Window, IGraphAppWindowView
{
    private OpenFileDialog? m_OpenFileDialog;
    private SaveFileDialog? m_SaveFileDialog;


    public IGraphAppWindowViewModel? ViewModel
    {
        get => GetValue(DataContextProperty) as IGraphAppWindowViewModel;
        set => SetValue(DataContextProperty, value);
    }


    public GraphAppWindowView()
    {
        InitializeComponent();
    }


    public void Dispose()
    {
    }

    private void OpenClickHandler(object sender, RoutedEventArgs e)
    {
        m_OpenFileDialog ??= new()
        {
            AddExtension     = false,
            CheckFileExists  = true,
            CheckPathExists  = true,
            DefaultExt       = "ini",
            Filter           = "ini files (*.ini)|*.ini|All files (*.*)|*.*",
            RestoreDirectory = true,
            Multiselect      = false,
            FileName         = "Matrix",
            InitialDirectory = Environment.CurrentDirectory
        };


        if (m_OpenFileDialog.ShowDialog(this) != true) return;

        if (!ViewModel?.LoadGraphCommand.CanExecute(m_OpenFileDialog.FileName) ?? true) return;

        ViewModel?.LoadGraphCommand.Execute(m_OpenFileDialog.FileName);
    }

    private void SaveClickHandler(object sender, RoutedEventArgs e)
    {
        m_SaveFileDialog ??= new()
        {
            AddExtension     = false,
            CheckPathExists  = true,
            DefaultExt       = "ini",
            Filter           = "ini files (*.ini)|*.ini|All files (*.*)|*.*",
            RestoreDirectory = true,
            FileName         = "Matrix",
            InitialDirectory = Environment.CurrentDirectory
        };

        if (m_SaveFileDialog.ShowDialog(this) != true) return;

        if (!ViewModel?.SaveGraphCommand.CanExecute(m_SaveFileDialog.FileName) ?? true) return;

        ViewModel?.SaveGraphCommand.Execute(m_SaveFileDialog.FileName);
    }

    private void ExitClickHandler(object sender, RoutedEventArgs e)
    {
        if (ViewModel?.ExitCommand.CanExecute(null) ?? false) ViewModel?.ExitCommand.Execute(null);

        Close();
    }
}