public interface IGraphLoggable
{
    public int VerticesCount { get; }
    public int EdgesCount { get; }

    public IEnumerable<IEdge> Edges { get; }
}