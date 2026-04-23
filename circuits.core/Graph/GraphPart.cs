public class GraphPart : IGraph
{
    private readonly IGraph parent;
    private readonly HashSet<Vertex> vertices;

    public int VerticesCount { get => vertices.Count; }
    public int EdgesCount { get => GetEdges().ToList().Count; }

    public IEnumerable<Vertex> Vertices { get => vertices; }
    public IEnumerable<Edge> Edges { get => GetEdges(); }

    private IEnumerable<Edge> GetEdges()
    {
        foreach (var edge in parent.Edges)
        {
            if (!vertices.Contains(edge.First) || !vertices.Contains(edge.Second)) continue;

            yield return edge;
        }
    }
    
    public GraphPart(IGraph parent)
    {
        this.parent = parent;
        vertices = [];
    }

    public void UseParentVertex(Vertex vertex)
    {
        if(!parent.HasVertex(vertex)) return;

        vertices.Add(vertex);
    }

    public bool HasVertex(Vertex vertex)
    {
        return vertices.Contains(vertex);
    }

    public bool HasEdge(Edge edge)
    {
        return GetEdges().Contains(edge);
    }
}