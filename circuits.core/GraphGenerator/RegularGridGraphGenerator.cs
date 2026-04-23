public class RegularGridGraphGenerator : GraphGenerator
{
    private readonly int rowsCount;
    private readonly int colsCount;

    public RegularGridGraphGenerator(int rowsCount, int colsCount)
    {
        var paramsError = GetParamsError(rowsCount, colsCount);
        if (paramsError != null)
            throw paramsError;

        this.rowsCount = rowsCount;
        this.colsCount = colsCount;
    }

    private Exception? GetParamsError(int rows, int cols)
    {
        if (rows <= 0)
            return new ArgumentException("Количество строк в решётке должно быть положительным");
        if (cols <= 0)
            return new ArgumentException("Количество столбцов в решётке должно быть положительным");

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
        return BaseGraph.Empty(rowsCount * colsCount);
    }

    private void FillGraph(IGraph graph)
    {
        foreach (var edge in GenerateHorizontalEdges())
        {
            graph.AddEdge(edge);
        }

        foreach (var edge in GenerateVerticalEdges())
        {
            graph.AddEdge(edge);
        }
    }

    private List<Edge> GenerateHorizontalEdges()
    {
        var edges = new List<Edge>();

        for (int rowIndex = 0; rowIndex < rowsCount; rowIndex++)
        {
            for (int colIndex = 0; colIndex < colsCount - 1; colIndex++)
            {
                int firstVertex = GetVertexId(rowIndex, colIndex);
                int secondVertex = GetVertexId(rowIndex, colIndex + 1);

                edges.Add(new Edge(new Vertex(firstVertex), new Vertex(secondVertex)));
            }
        }

        return edges;
    }

    private List<Edge> GenerateVerticalEdges()
    {
        var edges = new List<Edge>();

        for (int rowIndex = 0; rowIndex < rowsCount - 1; rowIndex++)
        {
            for (int colIndex = 0; colIndex < colsCount; colIndex++)
            {
                int firstVertex = GetVertexId(rowIndex, colIndex);
                int secondVertex = GetVertexId(rowIndex + 1, colIndex);

                edges.Add(new Edge(new Vertex(firstVertex), new Vertex(secondVertex)));
            }
        }

        return edges;
    }

    private int GetVertexId(int rowIndex, int colIndex)
    {
        return rowIndex * colsCount + colIndex + 1;
    }
}
