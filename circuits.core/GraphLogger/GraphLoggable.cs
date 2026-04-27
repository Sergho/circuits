public interface GraphLoggable
{
    public int VerticesCount { get; }
    public int EdgesCount { get; }

    public IEnumerable<Edge> Edges { get; }
}