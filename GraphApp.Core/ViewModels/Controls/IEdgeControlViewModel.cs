using System.Windows;
using System.Windows.Media;

using GraphApp.Core.Data;


namespace GraphApp.Core.ViewModels.Controls;

public interface IEdgeControlViewModel : IControlViewModel
{
    (Guid From, Guid To) PathId             { get; }
    Guid                 FromId             { get; }
    Guid                 ToId               { get; }
    Point                TextPosition       { get; }
    Point                TextCenter         { get; }
    int                  ZIndex             { get; }
    double               Size               { get; }
    Point                FromPosition       { get; }
    Point                ToPosition         { get; }
    Point                LeftArrowPosition  { get; }
    Point                RightArrowPosition { get; }
    double               TextHeight         { set; }
    double               TextWidth          { set; }
    double               Value              { get; set; }
    Brush?               BackgroundColor    { get; set; }
    Brush?               BorderColor        { get; set; }
    double?              BorderSize         { get; set; }
    Brush?               TextColor          { get; set; }
    double?              TextSize           { get; set; }
    FontWeight?          TextWeight         { get; set; }


    void SetStyle(ElementStyle style);
}