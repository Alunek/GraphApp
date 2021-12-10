using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

using Prism.Mvvm;


namespace GraphApp.Core.Data.Graphs;

public abstract class GraphElement : BindableBase
{
    protected readonly ElementStyle Style;


    public Brush? BackgroundColor
    {
        get => Style.BackgroundColor;
        set => Style.BackgroundColor = value;
    }

    public Brush? BorderColor
    {
        get => Style.BorderColor;
        set => Style.BorderColor = value;
    }

    public double? BorderSize
    {
        get => Style.BorderSize;
        set => Style.BorderSize = value;
    }

    public Brush? TextColor
    {
        get => Style.TextColor;
        set => Style.TextColor = value;
    }

    public double? TextSize
    {
        get => Style.TextSize;
        set => Style.TextSize = value;
    }

    public FontWeight? TextWeight
    {
        get => Style.TextWeight;
        set => Style.TextWeight = value;
    }


    protected GraphElement()
    {
        Style = new ElementStyle();

        Style.PropertyChanged += StylePropertyChanged;
    }


    public void SetStyle(ElementStyle style)
    {
        BackgroundColor = style.BackgroundColor;
        BorderColor     = style.BorderColor;
        BorderSize      = style.BorderSize;
        TextColor       = style.TextColor;
        TextSize        = style.TextSize;
        TextWeight      = style.TextWeight;
    }

    protected virtual void StylePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        RaisePropertyChanged(e.PropertyName);
    }
}