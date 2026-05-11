public class GraphPart : IGraphPart
{
    private IGraph parent;
    private HashSet<IVertex> vertices;

    public int VerticesCount { get => vertices.Count; }
    public int EdgesCount { get => GetEdges().ToList().Count; }

    public IEnumerable<IVertex> Vertices { get => vertices; }
    public IEnumerable<IEdge> Edges { get => GetEdges(); }

    private IEnumerable<IEdge> GetEdges()
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

    public void AssignVertex(IVertex vertex)
    {
        if(!parent.HasVertex(vertex)) return;

        vertices.Add(vertex);
    }

    public bool HasVertex(IVertex vertex)
    {
        return vertices.Contains(vertex);
    }

    public void RemoveVertex(IVertex vertex)
    {
        vertices.Remove(vertex);
    }
}