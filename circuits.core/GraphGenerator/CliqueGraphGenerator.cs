public class CliqueGraphGenerator : GraphGenerator
{
    private readonly int verticesCount;

    public CliqueGraphGenerator(int verticesCount)
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

                graph.AddEdge(new Edge(firstVertex, secondVertex));
            }
        }
    }
}
