using System.ComponentModel;
using System.Windows;

using GraphApp.Core.Helper;


namespace GraphApp.Core.Data.Graphs;

public class EdgeData : GraphElement
{
    public const double MinValue   = 0.1d;
    public const double MaxValue   = 100.0d;
    public const int    RoundValue = 1;

    private readonly (VertexData From, VertexData To) m_PathVertex;

    private Point  m_TextPosition;
    private Point  m_TextCenter;
    private int    m_ZIndex;
    private double m_Size;
    private Point  m_FromPosition;
    private Point  m_ToPosition;
    private Point  m_LeftArrowPosition;
    private Point  m_RightArrowPosition;
    private double m_Value;

    private double? m_TextHeight;
    private double? m_TextWidth;


    public static Point  DefaultPosition => new(0, 0);
    public static double DefaultSize     => 50.0d;


    private VertexData FromVertex => m_PathVertex.From;
    private VertexData ToVertex   => m_PathVertex.To;

    public (Guid From, Guid To) PathId => (From: FromVertex.Id, To: ToVertex.Id);
    public Guid                 FromId => FromVertex.Id;
    public Guid                 ToId   => ToVertex.Id;

    public Point TextPosition
    {
        get => m_TextPosition;
        set => SetProperty(ref m_TextPosition, value, UpdateTextCenter);
    }

    public Point TextCenter
    {
        get => m_TextCenter;
        set => SetProperty(ref m_TextCenter, value, UpdateTextPosition);
    }

    public int ZIndex
    {
        get => m_ZIndex;
        set => SetProperty(ref m_ZIndex, value);
    }

    public double Size
    {
        get => m_Size;
        set => SetProperty(ref m_Size, value);
    }

    public Point FromPosition
    {
        get => m_FromPosition;
        private set => SetProperty(ref m_FromPosition, value);
    }

    public Point ToPosition
    {
        get => m_ToPosition;
        private set => SetProperty(ref m_ToPosition, value);
    }

    public Point LeftArrowPosition
    {
        get => m_LeftArrowPosition;
        private set => SetProperty(ref m_LeftArrowPosition, value);
    }

    public Point RightArrowPosition
    {
        get => m_RightArrowPosition;
        private set => SetProperty(ref m_RightArrowPosition, value);
    }

    public double Value
    {
        get => m_Value;
        set => SetProperty(ref m_Value, Math.Min(Math.Max(Math.Round(value, RoundValue), MinValue), MaxValue));
    }

    public double? TextHeight
    {
        get => m_TextHeight;
        set => SetProperty(ref m_TextHeight, value, UpdateTextPosition);
    }

    public double? TextWidth
    {
        get => m_TextWidth;
        set => SetProperty(ref m_TextWidth, value, UpdateTextPosition);
    }


    public EdgeData((VertexData From, VertexData To) pathVertex, double value = 1.0d) : base()
    {
        m_PathVertex = pathVertex;
        m_Value      = value;

        m_TextPosition = DefaultPosition;
        m_Size         = DefaultSize;
        m_TextCenter   = TrigonometricHelper.PositionToCenter(m_TextPosition, m_Size);

        UpdatePosition();

        FromVertex.PropertyChanged += PathVertexPropertyChangedHandler;
        ToVertex.PropertyChanged   += PathVertexPropertyChangedHandler;
    }


    private void PathVertexPropertyChangedHandler(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(FromVertex.Center) or nameof(FromVertex.Size))
            UpdatePosition();
    }

    private void UpdatePosition()
    {
        var FromVertexVector = (Vector)FromVertex.Center;
        var ToVertexVector   = (Vector)ToVertex.Center;

        var    MovementVector = ToVertexVector - FromVertexVector;
        double MovementAngle  = Vector.AngleBetween(TrigonometricHelper.NormalVectorX, MovementVector);
        var    MatrixRotate   = TrigonometricHelper.CreateRotationMatrix(MovementAngle);

        double VertexRadius = FromVertex.Size                            / 2;
        double Spread       = FromVertex.Size                            / 10;
        double Arrow        = FromVertex.Size                            / 20;
        double OffsetY      = (TextSize ?? ElementStyle.DefaultTextSize) * 1;
        double OffsetX      = Math.Max(Math.Min(OffsetY, MovementVector.Length - VertexRadius * 2.5), VertexRadius / 2);

        var NewFromPosition       = FromVertexVector + MatrixRotate.Transform(new Vector(VertexRadius, Spread));
        var NewToPosition         = ToVertexVector   - MatrixRotate.Transform(new Vector(VertexRadius, -Spread));
        var NewLeftArrowPosition  = NewToPosition    - MatrixRotate.Transform(new Vector(Spread,       Arrow));
        var NewRightArrowPosition = NewToPosition    - MatrixRotate.Transform(new Vector(Spread,       -Arrow));
        var NewTextCenter         = NewToPosition    - MatrixRotate.Transform(new Vector(OffsetX,      -OffsetY));

        FromPosition       = (Point)NewFromPosition;
        ToPosition         = (Point)NewToPosition;
        LeftArrowPosition  = (Point)NewLeftArrowPosition;
        RightArrowPosition = (Point)NewRightArrowPosition;
        TextCenter         = (Point)NewTextCenter;
    }

    private void UpdateTextCenter()
    {
        TextCenter = TrigonometricHelper.PositionToCenter(TextPosition, TextHeight, TextWidth);
    }

    private void UpdateTextPosition()
    {
        TextPosition = TrigonometricHelper.CenterToPosition(TextCenter, TextHeight, TextWidth);
    }
}