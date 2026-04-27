public class FisherYatesGraphGenerator : GraphGenerator
{
    private readonly int verticesCount;
    private readonly int edgesCount;
    private readonly Random random;

    public FisherYatesGraphGenerator(int verticesCount, int edgesCount)
    {
        var paramsError = GetParamsError(verticesCount, edgesCount);
        if (paramsError != null)
            throw paramsError;

        this.verticesCount = verticesCount;
        this.edgesCount = edgesCount;
        random = new();
    }

    private Exception? GetParamsError(int verticesCount, double edgesCount)
    {
        if (verticesCount < 0)
            return new ArgumentException("Количество вершин в графе не может быть отрицательным");

        int maxEdges = verticesCount * (verticesCount - 1) / 2;
        if (edgesCount < 0 || edgesCount > maxEdges)
            return new ArgumentException($"Количество ребер в графе должно быть между 0 и {maxEdges}");

        return null;
    }

    public Graph Generate()
    {
        Graph graph = GetEmptyGraph();
        FillGraph(graph);

        return graph;
    }

    private Graph GetEmptyGraph()
    {
        return Graph.Empty(verticesCount);
    }

    private void FillGraph(Graph graph)
    {
        foreach (var edge in GenerateRandomEdges())
        {
            graph.AddEdge(edge);
        }
    }

    
    private List<Edge> GenerateRandomEdges()
    {
        var allEdges = GenerateAllEdges();
        var result = new List<Edge>();

        for (int i = 0; i < edgesCount; i++)
        {
            int randomEdgeIndex = random.Next(allEdges.Count - i);
            result.Add(allEdges[randomEdgeIndex]);
            allEdges[randomEdgeIndex] = allEdges[allEdges.Count - 1 - i];
        }

        return result;
    }

    private List<Edge> GenerateAllEdges()
    {
        var edges = new List<Edge>();

        for (int i = 1; i <= verticesCount; i++)
        {
            for (int j = i + 1; j <= verticesCount; j++)
            {
                var firstVertex = new Vertex(i);
                var secondVertex = new Vertex(j);

                edges.Add(new Edge(firstVertex, secondVertex));
            }
        }

        return edges;
    }

}
