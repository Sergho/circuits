public class Edge : IEdge
{
    public IVertex First { get; }
    public IVertex Second { get; }

    public Edge(IVertex first, IVertex second)
    {
        First = first;
        Second = second;
    }

    public IEdge GetNormalized()
    {
        IVertex min = First.CompareTo(Second) > 0 ? Second : First;
        IVertex max = First.CompareTo(Second) > 0 ? First : Second;

        return new Edge(min, max);
    }

    public bool Equals(IEdge? other)
    {
        if (other is null) return false;

        var thisNormalized = GetNormalized();
        var otherNormalized = other.GetNormalized();

        return thisNormalized.First.Equals(otherNormalized.First) && thisNormalized.Second.Equals(otherNormalized.Second);
    }

    public override bool Equals(object? obj)
    {
        return obj is IEdge edge && Equals(edge);
    }

    public override int GetHashCode()
    {
        var normalized = GetNormalized();
        
        return HashCode.Combine(normalized.First, normalized.Second);
    }
}