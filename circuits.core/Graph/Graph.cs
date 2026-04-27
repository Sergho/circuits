public class Graph
{
    private readonly HashSet<Vertex> vertices;
    private readonly HashSet<Edge> edges;

    public int VerticesCount { get; }
    public int EdgesCount { get => edges.Count; }

    public IEnumerable<Vertex> Vertices { get => vertices; }
    public IEnumerable<Edge> Edges { get => edges; }

    public static Graph Empty(int verticesCount)
    {
        return new Graph(verticesCount);
    }
    
    private Graph(int verticesCount)
    {
        VerticesCount = verticesCount;
        
        vertices = [];
        edges = [];

        CreateVertices();
    }

    private void CreateVertices()
    {
        for (int i = 1; i <= VerticesCount; i++)
        {
            vertices.Add(new Vertex(i));
        }
    }

    public bool HasVertex(Vertex vertex)
    {
        return vertices.Contains(vertex);
    }

    public void AddEdge(Edge edge)
    {
        if (edges.Contains(edge)) return;
        if (!vertices.Contains(edge.First) || !vertices.Contains(edge.Second)) return;
        
        edges.Add(edge);
    }
}