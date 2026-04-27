public class KernighanLinGraphPartitionGenerator : GraphPartitionGenerator
{
    private readonly int partsCount;
    private readonly Random random;

    public KernighanLinGraphPartitionGenerator(int partsCount)
    {
        this.partsCount = partsCount;
        random = new();
    }

    public GraphPartition Generate(Graph graph)
    {
        var partition = InitPartition(graph);
        while(true)
        {
            var maxGainPair = GetMaxGainPair(partition);
            if (maxGainPair == null) break;

            var (vertexA, vertexB) = maxGainPair.Value;
            partition.SwapVertices(vertexA, vertexB);
        }
        

        return partition;
    }

    private GraphPartition InitPartition(Graph graph)
    {
        var partition = new GraphPartition(graph, partsCount);
        for (int i = 1; i <= graph.VerticesCount; i++)
        {
            int j = random.Next(i, graph.VerticesCount + 1);
            partition.SwapVertices(new Vertex(i), new Vertex(j));
        }

        return partition;
    }

    private IEnumerable<(Vertex, Vertex)> GetAllVertexPairs(GraphPartition partition)
    {
        var parts = partition.Parts.ToList();
        for (int firstPart = 0; firstPart < partsCount; firstPart++)
        {
            for (int secondPart = firstPart + 1; secondPart < partsCount; secondPart++)
            {
                foreach (var firstVertex in parts[firstPart].Vertices)
                {
                    foreach (var secondVertex in parts[secondPart].Vertices)
                    {
                        yield return (firstVertex, secondVertex);
                    }
                }
            }
        }
    }

    private (Vertex, Vertex)? GetMaxGainPair(GraphPartition partition)
    {
        int maxGain = 0;
        (Vertex, Vertex)? maxGainPair = null;
        foreach(var pair in GetAllVertexPairs(partition))
        {
            var gain = partition.CalculateGain(pair.Item1, pair.Item2);
            if(gain > maxGain)
            {
                maxGain = gain;
                maxGainPair = pair;
            }
        }

        return maxGainPair;
    }
}