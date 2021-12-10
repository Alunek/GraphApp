using System.ComponentModel;

using GraphApp.Core.Data.Graphs;

using Prism.Mvvm;


namespace GraphApp.Core.Models.Matrix;

public class MatrixCell : BindableBase
{
    private EdgeData? m_Data;


    private MatrixTable          Table => Column.Table;
    private (Guid From, Guid To) Path  => (Row.VertexId, Column.VertexId);


    public bool IsEmpty { get; private init; }

    public string Text
    {
        get => $"{(IsEmpty ? string.Empty : Data?.Value ?? 0):N1}";
        set
        {
            if (IsEmpty || !double.TryParse(value, out double Value)) return;

            if (Value > 0)
            {
                if (Data is null)
                    Table.AddEdge(Path, Value);
                else
                    Data.Value = Value;
            }
            else if (Data is not null)
            {
                Table.RemoveEdge(Path);
            }
        }
    }

    public MatrixColumn Column { get; }
    public MatrixRow    Row    { get; }

    public EdgeData? Data
    {
        get => m_Data;
        set
        {
            if (Data is not null) Data.PropertyChanged -= DataPropertyChangedHandler;
            SetProperty(ref m_Data, value, UpdateData);
            if (Data is not null) Data.PropertyChanged += DataPropertyChangedHandler;
        }
    }


    public MatrixCell(MatrixColumn column, MatrixRow row, EdgeData? data)
    {
        IsEmpty = false;
        Column  = column;
        Row     = row;
        Data    = data;
    }


    public static MatrixCell GetEmpty(MatrixColumn column, MatrixRow row)
    {
        return new(column, row, null)
        {
            IsEmpty = true
        };
    }


    private void UpdateData()
    {
        if (IsEmpty) throw new InvalidOperationException("Данный элемент не может быть изменен");

        RaisePropertyChanged(nameof(Text));
    }

    private void DataPropertyChangedHandler(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is not nameof(EdgeData.Value)) return;

        RaisePropertyChanged(nameof(Text));
    }
}