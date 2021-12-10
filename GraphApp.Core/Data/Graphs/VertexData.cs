using System.Windows;

using GraphApp.Core.Helper;


namespace GraphApp.Core.Data.Graphs;

public class VertexData : GraphElement
{
    private Point  m_Position;
    private Point  m_Center;
    private int    m_ZIndex;
    private double m_Size;
    private string m_TextString;


    public static Point  DefaultPosition => new(0, 0);
    public static double DefaultSize     => 50.0d;


    public Guid Id { get; }

    public Point Position
    {
        get => m_Position;
        set => SetProperty(ref m_Position, value, UpdateCenter);
    }

    public Point Center
    {
        get => m_Center;
        set => SetProperty(ref m_Center, value, UpdatePosition);
    }

    public int ZIndex
    {
        get => m_ZIndex;
        set => SetProperty(ref m_ZIndex, value);
    }

    public double Size
    {
        get => m_Size;
        set => SetProperty(ref m_Size, value, UpdatePosition);
    }

    public string TextString
    {
        get => m_TextString;
        set => SetProperty(ref m_TextString, value);
    }


    public VertexData(Guid id) : base()
    {
        Id = id;

        m_Position   = DefaultPosition;
        m_Size       = DefaultSize;
        m_ZIndex     = 0;
        m_Center     = TrigonometricHelper.PositionToCenter(m_Position, m_Size);
        m_TextString = string.Empty;
    }


    private void UpdateCenter()
    {
        Center = TrigonometricHelper.PositionToCenter(Position, Size);
    }

    private void UpdatePosition()
    {
        Position = TrigonometricHelper.CenterToPosition(Center, Size);
    }
}