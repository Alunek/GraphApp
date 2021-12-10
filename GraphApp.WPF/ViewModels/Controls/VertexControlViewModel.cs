using GraphApp.Core.Services;
using GraphApp.Core.ViewModels.Controls;

using System.Windows;
using System.Windows.Media;

using GraphApp.Core.Data;
using GraphApp.Core.Data.Graphs;
using GraphApp.Core.Models;
using GraphApp.Core.Models.Graphs;


namespace GraphApp.WPF.ViewModels.Controls;

internal class VertexControlViewModel : ControlViewModelBase<VertexData>, IVertexControlViewModel
{
    private readonly Vertex m_Vertex;


    public Guid Id => Data.Id;

    public Point Position
    {
        get => Data.Position;
        set => Data.Position = value;
    }

    public Point Center
    {
        get => Data.Center;
        set => Data.Center = value;
    }

    public int ZIndex
    {
        get => Data.ZIndex;
        set => Data.ZIndex = value;
    }

    public double Size
    {
        get => Data.Size;
        set => Data.Size = value;
    }

    public string TextString
    {
        get => Data.TextString;
        set => Data.TextString = value;
    }

    public Brush? BackgroundColor
    {
        get => Data.BackgroundColor;
        set => Data.BackgroundColor = value;
    }

    public Brush? BorderColor
    {
        get => Data.BorderColor;
        set => Data.BorderColor = value;
    }

    public double? BorderSize
    {
        get => Data.BorderSize;
        set => Data.BorderSize = value;
    }

    public Brush? TextColor
    {
        get => Data.TextColor;
        set => Data.TextColor = value;
    }

    public double? TextSize
    {
        get => Data.TextSize;
        set => Data.TextSize = value;
    }

    public FontWeight? TextWeight
    {
        get => Data.TextWeight;
        set => Data.TextWeight = value;
    }


    public VertexControlViewModel(IBusinessLogic businessLogic, Vertex vertex) : base(businessLogic, vertex.Data)
    {
        m_Vertex = vertex;
    }


    public void SetStyle(ElementStyle style)
    {
        Data.SetStyle(style);
    }
}