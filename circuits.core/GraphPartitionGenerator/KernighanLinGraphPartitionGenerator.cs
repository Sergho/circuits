public class KernighanLinGraphPartitionGenerator : IGraphPartitionGenerator
{
    private readonly int partsCount;
    private readonly Random random;
    private readonly Dictionary<Edge, int> cachedGains;
    
    public int LastIterationsCount { get; private set; }

    public KernighanLinGraphPartitionGenerator(int partsCount)
    {
        this.partsCount = partsCount;
        random = new();
        cachedGains = [];

        LastIterationsCount = 0;
    }

    public IGraphPartition Generate(IGraph graph)
    {
        var partition = CreatePartition(graph);

        InitGainsCache(partition);
        InitPartition(partition);

        LastIterationsCount = 0;
        while(true)
        {
            var maxGainPair = GetMaxGainPair();
            if (maxGainPair == null) break;

            var (firstVertex, secondVertex) = maxGainPair.Value;
            SwapVertices(partition, firstVertex, secondVertex);
            LastIterationsCount++;
        }
        

        return partition;
    }

    private IGraphPartition CreatePartition(IGraph graph)
    {
        return new GraphPartition(graph, partsCount);
    }

    private IGraphPartition InitPartition(IGraphPartition partition)
    {
        for (int i = 1; i < partition.Graph.VerticesCount; i++)
        {
            int j = random.Next(i + 1, partition.Graph.VerticesCount);

            var firstVertex = new Vertex(i);
            var secondVertex = new Vertex(j);

            if (partition.GetPart(firstVertex) == partition.GetPart(secondVertex)) continue;

            SwapVertices(partition, firstVertex, secondVertex);
        }

        return partition;
    }

    private void InitGainsCache(IGraphPartition partition)
    {
        foreach(var (first, second) in GetAllVertexPairs(partition))
        {
            var gain = partition.CalculateGain(first, second);
            cachedGains.Add(new Edge(first, second), gain);
        }
    }

    private IEnumerable<(IVertex, IVertex)> GetAllVertexPairs(IGraphPartition partition)
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

    private (IVertex, IVertex)? GetMaxGainPair()
    {
        int maxGain = 0;
        (IVertex, IVertex)? maxGainPair = null;
        foreach(var (edge, gain) in cachedGains)
        {
            if(gain > maxGain)
            {
                maxGain = gain;
                maxGainPair = (edge.First, edge.Second);
            }
        }

        return maxGainPair;
    }

    private void SwapVertices(IGraphPartition partition, IVertex first, IVertex second)
    {
        partition.SwapVertices(first, second);

        UpdateGainsForVertex(partition, first);
        UpdateGainsForVertex(partition, second);
    }

    private void UpdateGainsForVertex(IGraphPartition partition, IVertex vertex)
    {
        foreach(var otherVertex in partition.Graph.Vertices)
        {
            if (otherVertex.Equals(vertex)) continue;

            var edgeKey = new Edge(vertex, otherVertex);
            if (cachedGains.ContainsKey(edgeKey))
            {
                cachedGains.Remove(edgeKey);
            } else
            {
                cachedGains.Add(edgeKey, partition.CalculateGain(vertex, otherVertex));
            }
        }
    }
}