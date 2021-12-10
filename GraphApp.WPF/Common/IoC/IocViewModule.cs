using GraphApp.Core.ViewModels.Controls;
using GraphApp.Core.Views.Controls;
using GraphApp.Core.Views.Windows;
using GraphApp.WPF.ViewModels.Controls;
using GraphApp.WPF.Views.Controls;
using GraphApp.WPF.Views.Windows;

using Ninject.Modules;


namespace GraphApp.WPF.Common.IoC;

internal class IocViewModule : NinjectModule
{
    public override void Load()
    {
        Bind<IGraphAppWindowView>().To<GraphAppWindowView>();
        Bind<IComparableAlgorithmsWindowView>().To<ComparableAlgorithmsWindowView>();

        Bind<IGraphControlView>().To<GraphControlView>();
        Bind<IVertexControlView>().To<VertexControlView>();
        Bind<IEdgeControlView>().To<EdgeControlView>();
        Bind<IMatrixPageControlView>().To<MatrixPageControlView>();
        Bind<IMatrixTableControlView>().To<MatrixTableControlView>();
    }
}