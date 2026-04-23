public class ChainGraphGenerator : GraphGenerator
{
    private readonly int verticesCount;

    public ChainGraphGenerator(int verticesCount)
    {
        var paramsError = GetParamsError(verticesCount);
        if (paramsError != null)
            throw paramsError;

        this.verticesCount = verticesCount;
    }

    private Exception? GetParamsError(int verticesCount)
    {
        if (verticesCount < 0)
            return new ArgumentException("Количество вершин в графе не может быть отрицательным");

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
        return BaseGraph.Empty(verticesCount);
    }

    private void FillGraph(IGraph graph)
    {
        for (int i = 1; i < verticesCount; i++)
        {
            var firstVertex = new Vertex(i);
            var secondVertex = new Vertex(i + 1);

            graph.AddEdge(new Edge(firstVertex, secondVertex));
        }

        graph.AddEdge(new Edge(new Vertex(verticesCount), new Vertex(1)));
    }
}
