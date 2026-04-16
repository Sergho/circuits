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

        FillGraph(graph);

        return graph;
    }

    private Graph GetEmptyGraph()
    {
        return Graph.Empty(verticesCount);
    }

    private void FillGraph(Graph graph)
    {
        while (graph.EdgesCount < edgesCount)
        {
            var firstVertex = new Vertex(random.Next(1, verticesCount + 1));
            var secondVertex = new Vertex(random.Next(1, verticesCount + 1));

            if (firstVertex.Equals(secondVertex))
                continue;

            graph.AddEdge(new Edge(firstVertex, secondVertex));
        }
    }
}