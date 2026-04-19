using System.Collections;

public class Graph : IEnumerable<Edge>
{
    private readonly HashSet<Edge> edges;

    public int VerticesCount { get; private set; }
    public int EdgesCount { get => edges.Count; }

    public static Graph Empty(int verticesCount)
    {
        return new Graph(verticesCount);
    }
    
    private Graph(int verticesCount)
    {
        VerticesCount = verticesCount;
        edges = [];
    }

    public void AddEdge(Edge edge)
    {
        if (edges.Contains(edge)) return;
        
        edges.Add(edge);
    }

    public IEnumerator<Edge> GetEnumerator()
    {
        return edges.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}