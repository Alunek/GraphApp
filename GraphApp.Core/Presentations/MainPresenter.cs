using GraphApp.Core.Services;


namespace GraphApp.Core.Presentations;

public abstract class MainPresenter : IPresenter
{
    private GraphAppPresenter? m_AppPresenter;
    private IBusinessLogic?    m_BusinessLogic;


    public void Initialize(IBusinessLogic businessLogic)
    {
        m_BusinessLogic = businessLogic;
    }

    public void Run()
    {
        m_AppPresenter = new GraphAppPresenter(m_BusinessLogic!);
        m_AppPresenter.Run();
    }

    public void Dispose()
    {
        m_AppPresenter?.Dispose();
    }
}