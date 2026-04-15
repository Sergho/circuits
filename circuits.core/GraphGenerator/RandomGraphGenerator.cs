public class RandomGraphGenerator : GraphGenerator
{
    private readonly int verticesCount;
    private readonly int edgesCount;
    private readonly Random random;

    public RandomGraphGenerator(int verticesCount, int edgesCount)
    {
        this.verticesCount = verticesCount;
        this.edgesCount = edgesCount;

        random = new();
    }

    public Graph Generate()
    {
        Graph graph = GetEmptyGraph();

        if (edgesCount == 0)
            return graph;

        FillGraph(graph);

        return graph;
    }

    private Graph GetEmptyGraph()
    {
        return Graph.Empty(verticesCount);
    }

    private void FillGraph(Graph graph)
    {
        var existingEdges = new HashSet<(int, int)>();
        while (existingEdges.Count < edgesCount)
        {
            int firstVertex = random.Next(verticesCount);
            int secondVertex = random.Next(verticesCount);

            if (firstVertex == secondVertex)
                continue;

            var edge = GetEdge(firstVertex, secondVertex);

            if(existingEdges.Contains(edge)) continue;

            existingEdges.Add(edge);
            graph.AddEdge(firstVertex, secondVertex);
        }
    }

    private (int, int) GetEdge(int firstVertex, int secondVertex)
    {
        return firstVertex < secondVertex ? (firstVertex, secondVertex) : (secondVertex, firstVertex);
    }
}