public class Edge : IEquatable<Edge>
{
    public Vertex First { get; }
    public Vertex Second { get; }

    public Edge(Vertex first, Vertex second)
    {
        First = first;
        Second = second;
    }

    public Edge GetNormalized()
    {
        Vertex min = First.CompareTo(Second) > 0 ? Second : First;
        Vertex max = First.CompareTo(Second) > 0 ? First : Second;

        return new Edge(min, max);
    }

    public bool Equals(Edge? other)
    {
        if (other is null) return false;

        var thisNormalized = GetNormalized();
        var otherNormalized = other.GetNormalized();

        return thisNormalized.First.Equals(otherNormalized.First) && thisNormalized.Second.Equals(otherNormalized.Second);
    }

    public override int GetHashCode()
    {
        var normalized = GetNormalized();
        
        return HashCode.Combine(normalized.First, normalized.Second);
    }
}