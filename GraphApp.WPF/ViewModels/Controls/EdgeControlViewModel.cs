using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

using GraphApp.Core.Data;
using GraphApp.Core.Data.Graphs;
using GraphApp.Core.Models.Graphs;
using GraphApp.Core.Services;
using GraphApp.Core.ViewModels.Controls;


namespace GraphApp.WPF.ViewModels.Controls;

internal class EdgeControlViewModel : ControlViewModelBase<EdgeData>, IEdgeControlViewModel
{
    private readonly Edge m_Edge;


    public (Guid From, Guid To) PathId             => Data.PathId;
    public Guid                 FromId             => Data.FromId;
    public Guid                 ToId               => Data.ToId;
    public Point                TextPosition       => Data.TextPosition;
    public Point                TextCenter         => Data.TextCenter;
    public int                  ZIndex             => Data.ZIndex;
    public double               Size               => Data.Size;
    public Point                FromPosition       => Data.FromPosition;
    public Point                ToPosition         => Data.ToPosition;
    public Point                LeftArrowPosition  => Data.LeftArrowPosition;
    public Point                RightArrowPosition => Data.RightArrowPosition;

    public double TextHeight
    {
        set => Data.TextHeight = value;
    }

    public double TextWidth
    {
        set => Data.TextWidth = value;
    }

    public string StringValue => $"{Data.Value:N1}";

    public double Value
    {
        get => Data.Value;
        set
        {
            Data.Value = value;
            RaisePropertyChanged(nameof(StringValue));
        }
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


    public EdgeControlViewModel(IBusinessLogic businessLogic, Edge edge) : base(businessLogic, edge.Data)
    {
        m_Edge = edge;
    }


    public void SetStyle(ElementStyle style)
    {
        Data.SetStyle(style);
    }


    protected override void OnPropertyChanged(PropertyChangedEventArgs args)
    {
        if (args.PropertyName is nameof(EdgeData.Value))
        {
            RaisePropertyChanged(nameof(StringValue));
        }

        base.OnPropertyChanged(args);
    }
}