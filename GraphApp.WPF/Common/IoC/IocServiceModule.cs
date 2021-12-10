using GraphApp.Core.Services;
using GraphApp.WPF.Common.Services;

using Ninject.Modules;


namespace GraphApp.WPF.Common.IoC;

internal class IocServiceModule : NinjectModule
{
    public override void Load()
    {
        Bind<IBusinessLogic>().To<BusinessLogic>().InSingletonScope();
        Bind<IGraphSaveRestoreLogic>().To<GraphSaveRestoreLogic>().InSingletonScope();
        Bind<IGraphAlgorithmDijkstraLogic>().To<GraphAlgorithmDijkstraLogic>().InSingletonScope();
        Bind<IGraphAlgorithmFloydLogic>().To<GraphAlgorithmFloydLogic>().InSingletonScope();
    }
}