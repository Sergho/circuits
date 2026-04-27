public class GraphPartition
{
    private readonly GraphPart[] parts;

    public Graph Graph { get; }
    public int PartsCount { get; }
    public IEnumerable<GraphPart> Parts { get => parts; }

    public GraphPartition(Graph graph, int partsCount)
    {
        Graph = graph;
        PartsCount = partsCount;

        parts = new GraphPart[PartsCount];

        CreateParts();
        DistributeVertices();
    }

    private void CreateParts()
    {
        for (int i = 0; i < PartsCount; i++)
        {
            parts[i] = new GraphPart(Graph);
        }
    }

    private void DistributeVertices()
    {
        var vertices = Graph.Vertices.ToArray();
        int verticesPerPart = Graph.VerticesCount / PartsCount;
        int extraVerticesCount = Graph.VerticesCount % PartsCount;

        int currentIndex = 0;
        for (int partIndex = 0; partIndex < PartsCount; partIndex++)
        {
            int partSize = verticesPerPart + (partIndex < extraVerticesCount ? 1 : 0);
            var part = new GraphPart(Graph);
            
            for(int vertexIndex = currentIndex; vertexIndex < currentIndex + partSize; vertexIndex++)
            {
                part.UseParentVertex(vertices[vertexIndex]);
            }

            parts[partIndex] = part;
            currentIndex += partSize;
        }
    }

    public void SwapVertices(Vertex first, Vertex second)
    {
        if(!Graph.HasVertex(first) || !Graph.HasVertex(second)) return;

        var firstPart = GetPartOfVertex(first);
        var secondPart = GetPartOfVertex(second);

        firstPart.RemoveVertex(first);
        secondPart.RemoveVertex(second);

        firstPart.UseParentVertex(second);
        secondPart.UseParentVertex(first);
    }

    public int CalculateGain(Vertex firstVertex, Vertex secondVertex)
    {
        int internalCount = GetInternalVerticesCount(firstVertex) + GetInternalVerticesCount(secondVertex);
        int externalCount = GetExternalVerticesCount(firstVertex) + GetExternalVerticesCount(secondVertex);
        bool graphHasEdge = Graph.HasEdge(new Edge(firstVertex, secondVertex));

        return externalCount - internalCount - (graphHasEdge ? 2 : 0);
    }

    private int GetInternalVerticesCount(Vertex firstVertex)
    {
        var part = GetPartOfVertex(firstVertex);
        int counter = 0;
        foreach(var adjacentVertex in Graph.GetAdjacencyList(firstVertex))
        {
            if(part.HasVertex(adjacentVertex)) counter++;
        }

        return counter;
    }

    private int GetExternalVerticesCount(Vertex firstVertex)
    {
        var part = GetPartOfVertex(firstVertex);
        var adjacencyList = Graph.GetAdjacencyList(firstVertex).ToList();
        int counter = adjacencyList.Count;
        foreach(var adjacentVertex in adjacencyList)
        {
            if(part.HasVertex(adjacentVertex)) counter--;
        }

        return counter;
    }

    private GraphPart GetPartOfVertex(Vertex vertex)
    {
        foreach (var part in parts) {
            if(part.HasVertex(vertex))
            {
                return part;
            }
        }

        throw new Exception("Incorrect Graph Partition");
    }

}