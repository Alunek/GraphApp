using Ninject.Syntax;


namespace GraphApp.Core.Services;

public interface IBusinessLogic
{
    IResolutionRoot IoC { get; }
}