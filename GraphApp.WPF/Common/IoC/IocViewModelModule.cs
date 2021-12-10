using GraphApp.Core.ViewModels.Controls;
using GraphApp.Core.ViewModels.Windows;
using GraphApp.WPF.ViewModels.Controls;
using GraphApp.WPF.ViewModels.Windows;

using Ninject.Modules;


namespace GraphApp.WPF.Common.IoC;

internal class IocViewModelModule : NinjectModule
{
    public override void Load()
    {
        Bind<IGraphAppWindowViewModel>().To<GraphAppWindowViewModel>();
        Bind<IComparableAlgorithmsWindowViewModel>().To<ComparableAlgorithmsWindowViewModel>();

        Bind<IGraphControlViewModel>().To<GraphControlViewModel>();
        Bind<IVertexControlViewModel>().To<VertexControlViewModel>();
        Bind<IEdgeControlViewModel>().To<EdgeControlViewModel>();
        Bind<IMatrixPageControlViewModel>().To<MatrixPageControlViewModel>();
        Bind<IMatrixTableControlViewModel>().To<MatrixTableControlViewModel>();
    }
}