public class GraphPartition
{
    private readonly GraphPart[] parts;
    private Dictionary<Vertex, GraphPart> vertexToPart;

    public Graph Graph { get; }
    public int PartsCount { get; }
    public IEnumerable<GraphPart> Parts { get => parts; }

    public GraphPartition(Graph graph, int partsCount)
    {
        Graph = graph;
        PartsCount = partsCount;

        parts = new GraphPart[PartsCount];
        vertexToPart = [];

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
                vertexToPart[vertices[vertexIndex]] = part;
            }

            parts[partIndex] = part;
            currentIndex += partSize;
        }
    }

    public void SwapVertices(Vertex first, Vertex second)
    {
        if(!Graph.HasVertex(first) || !Graph.HasVertex(second)) return;

        var firstPart = GetPart(first);
        var secondPart = GetPart(second);

        firstPart.RemoveVertex(first);
        secondPart.RemoveVertex(second);

        firstPart.UseParentVertex(second);
        secondPart.UseParentVertex(first);

        vertexToPart[second] = firstPart;
        vertexToPart[first] = secondPart;
    }

    public int CalculateGain(Vertex firstVertex, Vertex secondVertex)
    {
        int internalCount = GetInternalVerticesCount(firstVertex) + GetInternalVerticesCount(secondVertex);
        int externalCount = GetExternalVerticesCount(firstVertex) + GetExternalVerticesCount(secondVertex);
        bool graphHasEdge = Graph.HasEdge(new Edge(firstVertex, secondVertex));

        return externalCount - internalCount - (graphHasEdge ? 2 : 0);
    }

    public int GetCrossEdgesCount()
    {
        int totalExternalVertices = 0;
        foreach(var vertex in Graph.Vertices)
        {
            totalExternalVertices += GetExternalVerticesCount(vertex);
        }

        return totalExternalVertices / 2;
    }

    public int CutSize => GetCrossEdgesCount();

    public int GetPartIndex(Vertex vertex)
    {
        var part = GetPart(vertex);
        for (int i = 0; i < parts.Length; i++)
            if (parts[i] == part) return i;
        throw new InvalidOperationException("Vertex not found in any partition part");
    }

    public void MoveVertex(Vertex vertex, int toPartIndex)
    {
        if (!Graph.HasVertex(vertex)) return;
        var fromPart = GetPart(vertex);
        if (fromPart == parts[toPartIndex]) return;
        fromPart.RemoveVertex(vertex);
        parts[toPartIndex].UseParentVertex(vertex);
        vertexToPart[vertex] = parts[toPartIndex];
    }

    public GraphPart GetPart(Vertex vertex)
    {
        return vertexToPart[vertex];
    }

    private int GetInternalVerticesCount(Vertex firstVertex)
    {
        var part = GetPart(firstVertex);
        int counter = 0;
        foreach(var adjacentVertex in Graph.GetAdjacencyList(firstVertex))
        {
            if(part.HasVertex(adjacentVertex)) counter++;
        }

        return counter;
    }

    private int GetExternalVerticesCount(Vertex firstVertex)
    {
        var part = GetPart(firstVertex);
        int counter = 0;
        foreach(var adjacentVertex in Graph.GetAdjacencyList(firstVertex))
        {
            if(!part.HasVertex(adjacentVertex)) counter++;
        }

        return counter;
    }
}