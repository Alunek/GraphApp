using System.Windows;
using System.Windows.Media;


namespace GraphApp.Core.Helper;

public static class TrigonometricHelper
{
    public static readonly Vector NormalVectorX = new(1, 0);
    public static readonly Vector NormalVectorY = new(0, 1);


    public static Matrix CreateRotationMatrix(double angle)
    {
        double Radians = angle / 180D * Math.PI;
        double Sin     = Math.Sin(Radians);
        double Cos     = Math.Cos(Radians);

        var Matrix = new Matrix(
            Cos, Sin,
            -Sin, Cos,
            0, 0);

        return Matrix;
    }

    public static Point PositionToCenter(Point position, double size)
    {
        return new Point(position.X + size / 2, position.Y + size / 2);
    }

    public static Point CenterToPosition(Point center, double size)
    {
        return new Point(center.X - size / 2, center.Y - size / 2);
    }

    public static Point PositionToCenter(Point position, double? height, double? width)
    {
        return new Point(position.X + (width ?? 0) / 2, position.Y + (height ?? 0) / 2);
    }

    public static Point CenterToPosition(Point center, double? height, double? width)
    {
        return new Point(center.X - (width ?? 0) / 2, center.Y - (height ?? 0) / 2);
    }
}