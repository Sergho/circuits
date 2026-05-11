public interface IEdge : IEquatable<IEdge>
{
    IVertex First { get; }
    IVertex Second { get; }

    IEdge GetNormalized();
}