public class BipartiteGraphGenerator : GraphGenerator
{
    private readonly int leftPartitionSize;
    private readonly int rightPartitionSize;
    private readonly int edgesCount;
    private readonly Random random;

    public BipartiteGraphGenerator(int leftPartitionSize, int rightPartitionSize, int edgesCount)
    {
        var paramsError = GetParamsError(leftPartitionSize, rightPartitionSize, edgesCount);
        if (paramsError != null)
            throw paramsError;

        this.leftPartitionSize = leftPartitionSize;
        this.rightPartitionSize = rightPartitionSize;
        this.edgesCount = edgesCount;
        random = new();
    }

    private Exception? GetParamsError(int leftSize, int rightSize, int edgeCount)
    {
        if (leftSize < 0)
            return new ArgumentException("Размер левой доли графа не может быть отрицательным");

        if (rightSize < 0)
            return new ArgumentException("Размер правой доли графа не может быть отрицательным");

        int maxEdges = leftSize * rightSize;
        if (edgeCount < 0 || edgeCount > maxEdges)
            return new ArgumentException($"Количество рёбер в двудольном графе должно быть между 0 и {maxEdges}");

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
        return Graph.Empty(leftPartitionSize + rightPartitionSize);
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
        var allEdges = GenerateAllPossibleEdges();
        var result = new List<Edge>();

        for (int i = 0; i < edgesCount; i++)
        {
            int randomEdgeIndex = random.Next(allEdges.Count - i);
            result.Add(allEdges[randomEdgeIndex]);
            allEdges[randomEdgeIndex] = allEdges[allEdges.Count - 1 - i];
        }

        return result;
    }

    private List<Edge> GenerateAllPossibleEdges()
    {
        var edges = new List<Edge>();
        var leftVertices = Enumerable.Range(1, leftPartitionSize)
            .Select(id => new Vertex(id))
            .ToList();
        var rightVertices = Enumerable.Range(leftPartitionSize + 1, rightPartitionSize)
            .Select(id => new Vertex(id))
            .ToList();

        foreach (var leftVertex in leftVertices)
        {
            foreach (var rightVertex in rightVertices)
            {
                edges.Add(new Edge(leftVertex, rightVertex));
            }
        }

        return edges;
    }
}
