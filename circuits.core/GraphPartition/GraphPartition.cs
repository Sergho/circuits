public class GraphPartition
{
    private readonly IReadonlyGraph[] parts;
    private readonly Random random;

    public IReadonlyGraph Graph { get; }
    public int PartsCount { get; }

    public GraphPartition(IReadonlyGraph graph, int partsCount)
    {
        Graph = graph;
        PartsCount = partsCount;

        parts = [];
        random = new();

        createParts();
        InitParts();
    }

    private void createParts()
    {
        for (int i = 0; i < PartsCount; i++)
        {
            parts.Append(new GraphPart(Graph));
        }
    }

    private void InitParts()
    {
        var allVertices = Graph.Vertices.ToArray();
        ShuffleVertices(allVertices);
        DistributeVertices(allVertices);
    }

    private void ShuffleVertices(Vertex[] vertices)
    {
        for (int i = 0; i < vertices.Length - 1; i++)
        {
            int j = random.Next(i, vertices.Length);
            (vertices[i], vertices[j]) = (vertices[j], vertices[i]);
        }
    }

    private void DistributeVertices(Vertex[] vertices)
    {
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
}