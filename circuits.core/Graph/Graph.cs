public class Graph : IGraph
{
    private HashSet<IVertex> vertices;
    private HashSet<IEdge> edges;
    private Dictionary<IVertex, HashSet<IVertex>> adjacencyMap;

    public int VerticesCount { get; }
    public int EdgesCount { get => edges.Count; }

    public IEnumerable<IVertex> Vertices { get => vertices; }
    public IEnumerable<IEdge> Edges { get => edges; }

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

    public bool HasVertex(IVertex vertex)
    {
        return vertices.Contains(vertex);
    }

    public bool HasEdge(IEdge edge)
    {
        return edges.Contains(edge);
    }

    public void AddEdge(IEdge edge)
    {
        if (edges.Contains(edge)) return;
        if (!vertices.Contains(edge.First) || !vertices.Contains(edge.Second)) return;
        
        edges.Add(edge);
        adjacencyMap[edge.First].Add(edge.Second);
        adjacencyMap[edge.Second].Add(edge.First);
    }

    public IEnumerable<IVertex> GetAdjacencyList(IVertex vertex)
    {
        return adjacencyMap[vertex];
    } 
}