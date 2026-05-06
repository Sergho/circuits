public class Vertex : IVertex
{
    public int Index { get; }

    public Vertex(int index)
    {
        if (index < 1)
            throw new ArgumentException("Индекс вершины не может быть меньше 1");

        Index = index;
    }

    public bool Equals(IVertex? other)
    {
        if (other is null) return false;

        return Index == other.Index;
    }

    public override bool Equals(object? obj)
    {
        return obj is IVertex vertex && Equals(vertex);
    }

    public int CompareTo(IVertex? other)
    {
        if (other is null) return 1;

        return Index.CompareTo(other.Index);
    }

    public override int GetHashCode()
    {
        return Index.GetHashCode();
    }
}