using System.Windows;
using System.Windows.Media;

using GraphApp.Core.Data;


namespace GraphApp.Core.ViewModels.Controls;

public interface IVertexControlViewModel : IControlViewModel
{
    Guid        Id              { get; }
    Point       Position        { get; set; }
    Point       Center          { get; set; }
    int         ZIndex          { get; set; }
    double      Size            { get; set; }
    string      TextString      { get; set; }
    Brush?      BackgroundColor { get; set; }
    Brush?      BorderColor     { get; set; }
    double?     BorderSize      { get; set; }
    Brush?      TextColor       { get; set; }
    double?     TextSize        { get; set; }
    FontWeight? TextWeight      { get; set; }


    void SetStyle(ElementStyle style);
}