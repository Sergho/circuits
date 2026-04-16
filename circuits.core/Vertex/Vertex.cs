using System.ComponentModel.DataAnnotations;

public class Vertex : IComparable<Vertex>, IEquatable<Vertex>
{
    public int Index { get; }

    public Vertex(int index)
    {
        if (index < 1)
            throw new ArgumentException("Индекс вершины не может быть меньше 1");

        Index = index;
    }

    public bool Equals(Vertex? other)
    {
        if (other is null) return false;

        return Index == other.Index;
    }

    public int CompareTo(Vertex? other)
    {
        if (other is null) return 1;

        return Index.CompareTo(other.Index);
    }
}