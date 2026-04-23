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
            return new ArgumentException($"Количество рёбер в графе должно быть между 0 и {maxEdges}");

        return null;
    }

    public IGraph Generate()
    {
        IGraph graph = GetEmptyGraph();
        FillGraph(graph);

        return graph;
    }

    private IGraph GetEmptyGraph()
    {
        return BaseGraph.Empty(leftPartitionSize + rightPartitionSize);
    }

    private void FillGraph(IGraph graph)
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
        for (int i = 1; i <= leftPartitionSize; i++)
        {
            for (int j = leftPartitionSize + 1; j <= leftPartitionSize + rightPartitionSize; j++)
            {
                var firstVertex = new Vertex(i);
                var secondVertex = new Vertex(j);

                edges.Add(new Edge(firstVertex, secondVertex));
            }
        }

        return edges;
    }
}
