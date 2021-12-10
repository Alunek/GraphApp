using GraphApp.Core.Services;
using GraphApp.WPF.Common.IoC;
using GraphApp.WPF.Common.Presentations;

using Ninject;

using System.Windows;


namespace GraphApp.WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        using var Presenter = new WpfMainPresenter();

        var ServiceModule   = new IocServiceModule();
        var ViewModelModule = new IocViewModelModule();
        var ViewModule      = new IocViewModule();

        var IoC = new IocKernel(ServiceModule, ViewModelModule, ViewModule);

        var BusinessLogic = IoC.Kernel.Get<IBusinessLogic>();

        Presenter.Initialize(BusinessLogic);

        Presenter.Run();
    }
}