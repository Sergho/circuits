public class Graph : GraphLoggable
{
    private readonly HashSet<Vertex> vertices;
    private readonly HashSet<Edge> edges;
    private readonly Dictionary<Vertex, HashSet<Vertex>> adjacencyMap;

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
        adjacencyMap = [];

        CreateVertices();
    }

    private void CreateVertices()
    {
        for (int i = 1; i <= VerticesCount; i++)
        {
            var vertex = new Vertex(i);

            vertices.Add(vertex);
            adjacencyMap.Add(vertex, []);
        }
    }

    public bool HasVertex(Vertex vertex)
    {
        return vertices.Contains(vertex);
    }

    public bool HasEdge(Edge edge)
    {
        return edges.Contains(edge);
    }

    public void AddEdge(Edge edge)
    {
        if (edges.Contains(edge)) return;
        if (!vertices.Contains(edge.First) || !vertices.Contains(edge.Second)) return;
        
        edges.Add(edge);
        adjacencyMap[edge.First].Add(edge.Second);
        adjacencyMap[edge.Second].Add(edge.First);
    }

    public IEnumerable<Vertex> GetAdjacencyList(Vertex vertex)
    {
        return adjacencyMap[vertex];
    } 
}