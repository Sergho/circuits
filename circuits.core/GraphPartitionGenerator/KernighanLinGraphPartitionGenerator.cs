public class KernighanLinGraphPartitionGenerator : GraphPartitionGenerator
{
    private readonly int partsCount;
    private readonly Random random;
    private Dictionary<Edge, int> cachedGains;

    public KernighanLinGraphPartitionGenerator(int partsCount)
    {
        this.partsCount = partsCount;
        random = new();
        cachedGains = [];
    }

    public GraphPartition Generate(Graph graph)
    {
        var partition = InitPartition(graph);
        PreCalculateGains(partition);

        const int maxIterationsCount = 200;
        for(int i = 0; i < maxIterationsCount; i++)
        {
            Console.WriteLine($"Iteration: {i}");
            var maxGainPair = GetMaxGainPair(partition);
            if (maxGainPair == null) break;

            var (firstVertex, secondVertex) = maxGainPair.Value;
            partition.SwapVertices(firstVertex, secondVertex);
            UpdateGains(graph, [firstVertex, secondVertex]);
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

    private void PreCalculateGains(GraphPartition partition)
    {
        foreach(var (first, second) in GetAllVertexPairs(partition))
        {
            cachedGains[new Edge(first, second)] = partition.CalculateGain(first, second);
        }
    }

    private (Vertex, Vertex)? GetMaxGainPair(GraphPartition partition)
    {
        int maxGain = 0;
        (Vertex, Vertex)? maxGainPair = null;
        foreach(var (first, second) in GetAllVertexPairs(partition))
        {
            var gain = CalculateGain(partition, first, second);
            if(gain > maxGain)
            {
                maxGain = gain;
                maxGainPair = (first, second);
            }
        }

        return maxGainPair;
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

    private void UpdateGains(Graph graph, Vertex[] changedVertices)
    {
        var affectedVertices = new HashSet<Vertex>(changedVertices);
        foreach(var changedVertex in changedVertices)
        {
            affectedVertices.UnionWith(graph.GetAdjacencyList(changedVertex));
        }

        foreach (var firstVertex in affectedVertices)
        {
            foreach (var secondVertex in affectedVertices)
            {
                cachedGains.Remove(new Edge(firstVertex, secondVertex));
            }
        }
    }

    private int CalculateGain(GraphPartition partition, Vertex first, Vertex second)
    {
        if (!cachedGains.ContainsKey(new Edge(first, second)))
        {
            cachedGains[new Edge(first, second)] = partition.CalculateGain(first, second);
        }

        return cachedGains[new Edge(first, second)];
    }
}