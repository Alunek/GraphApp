using GraphApp.Core.Services;

using Ninject;
using Ninject.Syntax;


namespace GraphApp.WPF.Common.Services;

internal class BusinessLogic : IBusinessLogic
{
    private readonly IKernel m_Kernel;


    public IResolutionRoot IoC => m_Kernel;


    public BusinessLogic(IKernel kernel)
    {
        m_Kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
    }
}