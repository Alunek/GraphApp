using System.Windows;
using System.Windows.Media;

using Prism.Mvvm;


namespace GraphApp.Core.Data;

public class ElementStyle : BindableBase
{
    private Brush?      m_BackgroundColor;
    private Brush?      m_BorderColor;
    private double?     m_BorderSize;
    private Brush?      m_TextColor;
    private double?     m_TextSize;
    private FontWeight? m_TextWeight;


    public static ElementStyle Default                => new();
    public static Brush        DefaultBackgroundColor => new SolidColorBrush(Colors.White);
    public static Brush        DefaultBorderColor     => new SolidColorBrush(Colors.Black);
    public static double       DefaultBorderSize      => 1.0d;
    public static Brush        DefaultTextColor       => new SolidColorBrush(Colors.Black);
    public static double       DefaultTextSize        => 15.0d;
    public static FontWeight   DefaultTextWeight      => FontWeight.FromOpenTypeWeight(400);


    public Brush? BackgroundColor
    {
        get => m_BackgroundColor;
        set => SetProperty(ref m_BackgroundColor, value);
    }

    public Brush? BorderColor
    {
        get => m_BorderColor;
        set => SetProperty(ref m_BorderColor, value);
    }

    public double? BorderSize
    {
        get => m_BorderSize;
        set => SetProperty(ref m_BorderSize, value);
    }

    public Brush? TextColor
    {
        get => m_TextColor;
        set => SetProperty(ref m_TextColor, value);
    }

    public double? TextSize
    {
        get => m_TextSize;
        set => SetProperty(ref m_TextSize, value);
    }

    public FontWeight? TextWeight
    {
        get => m_TextWeight;
        set => SetProperty(ref m_TextWeight, value);
    }


    public ElementStyle(bool initialize = true)
    {
        if (!initialize) return;

        m_BackgroundColor = DefaultBackgroundColor;
        m_BorderColor     = DefaultBorderColor;
        m_BorderSize      = DefaultBorderSize;
        m_TextColor       = DefaultTextColor;
        m_TextSize        = DefaultTextSize;
        m_TextWeight      = DefaultTextWeight;
    }


    public static ElementStyle CombineOrDefault(ElementStyle? left, ElementStyle? right, ElementStyle? @default = null)
    {
        var DefaultStyle = @default ?? Default;

        return new(false)
        {
            BackgroundColor = left?.BackgroundColor ?? right?.BackgroundColor ?? DefaultStyle.BackgroundColor,
            BorderColor     = left?.BorderColor     ?? right?.BorderColor     ?? DefaultStyle.BorderColor,
            BorderSize      = left?.BorderSize      ?? right?.BorderSize      ?? DefaultStyle.BorderSize,
            TextColor       = left?.TextColor       ?? right?.TextColor       ?? DefaultStyle.TextColor,
            TextSize        = left?.TextSize        ?? right?.TextSize        ?? DefaultStyle.TextSize,
            TextWeight      = left?.TextWeight      ?? right?.TextWeight      ?? DefaultStyle.TextWeight
        };
    }
}