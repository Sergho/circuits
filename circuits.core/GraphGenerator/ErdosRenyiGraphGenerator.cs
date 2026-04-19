public class ErdosRenyiProbabilityGenerator : GraphGenerator
{
    private readonly int verticesCount;
    private readonly double edgeProbability;
    private readonly Random random;

    public ErdosRenyiProbabilityGenerator(int verticesCount, double edgeProbability)
    {
        var paramsError = GetParamsError(verticesCount, edgeProbability);
        if (paramsError != null)
            throw paramsError;

        this.verticesCount = verticesCount;
        this.edgeProbability = edgeProbability;
        
        random = new();
    }

    private Exception? GetParamsError(int verticesCount, double edgeProbability)
    {
        if (verticesCount < 0)
            return new ArgumentException("Количество вершин в графе не может быть отрицательным");
        if (edgeProbability < 0 || edgeProbability > 1)
            return new ArgumentException("Вероятность появления ребра должна быть между 0 и 1");

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
        for (int i = 1; i <= verticesCount; i++)
        {
            for (int j = i + 1; j <= verticesCount; j++)
            {
                var firstVertex = new Vertex(i);
                var secondVertex = new Vertex(j);

                if (random.NextDouble() < edgeProbability)
                {
                    graph.AddEdge(new Edge(firstVertex, secondVertex));
                }
            }
        }
    }
}
