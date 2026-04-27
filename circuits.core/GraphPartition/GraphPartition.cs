public class GraphPartition
{
    private readonly GraphPart[] parts;

    public Graph Graph { get; }
    public int PartsCount { get; }

    public GraphPartition(Graph graph, int partsCount)
    {
        Graph = graph;
        PartsCount = partsCount;

        parts = [];

        CreateParts();
        DistributeVertices();
    }

    private void CreateParts()
    {
        for (int i = 0; i < PartsCount; i++)
        {
            parts.Append(new GraphPart(Graph));
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
}