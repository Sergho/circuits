public interface IGraph
{
    public int VerticesCount { get; }
    public int EdgesCount { get; }

    public IEnumerable<Vertex> Vertices { get; }
    public IEnumerable<Edge> Edges { get; }

    public bool HasVertex(Vertex vertex);
    public bool HasEdge(Edge edge);
}