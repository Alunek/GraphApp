using Ninject;
using Ninject.Modules;


namespace GraphApp.WPF.Common.IoC;

internal class IocKernel
{
    public IKernel Kernel { get; }


    public IocKernel(params INinjectModule[] modules)
    {
        Kernel = new StandardKernel(modules);
    }
}