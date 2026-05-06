public class FiducciaMattheysesGraphPartitionGenerator : GraphPartitionGenerator
{
    private int iterationsCount;
    private readonly Random random;

    public int LastIterationsCount => iterationsCount;
    public int IterationsCount => iterationsCount;

    public FiducciaMattheysesGraphPartitionGenerator()
    {
        random = new();
    }

    public GraphPartition Generate(Graph graph)
    {
        var partition = InitPartition(graph);
        iterationsCount = 0;
        int applied;
        while ((applied = RunPass(partition, graph)) > 0)
            iterationsCount += applied;
        return partition;
    }

    private GraphPartition InitPartition(Graph graph)
    {
        var partition = new GraphPartition(graph, 2);
        for (int i = 1; i <= graph.VerticesCount; i++)
        {
            int j = random.Next(i, graph.VerticesCount + 1);
            partition.SwapVertices(new Vertex(i), new Vertex(j));
        }
        return partition;
    }

    private int RunPass(GraphPartition partition, Graph graph)
    {
        var vertices = graph.Vertices.ToList();
        var D = ComputeAllD(vertices, partition, graph);
        var locked = new HashSet<Vertex>(vertices.Count);
        var moves = new List<(Vertex vertex, int fromPart, int toPart, int cumGain)>();
        int cumGain = 0;
        int[] sizes = partition.Parts.Select(p => p.VerticesCount).ToArray();
        int[] initSizes = (int[])sizes.Clone();

        while (locked.Count < vertices.Count)
        {
            Vertex? best = null;
            int bestD = int.MinValue;

            foreach (var v in vertices)
            {
                if (locked.Contains(v)) continue;
                int fp = partition.GetPartIndex(v);
                if (sizes[fp] < sizes[1 - fp]) continue;
                if (D[v] > bestD)
                {
                    bestD = D[v];
                    best = v;
                }
            }

            if (best == null) break;

            int fromPart = partition.GetPartIndex(best);
            int toPart = 1 - fromPart;
            partition.MoveVertex(best, toPart);
            sizes[fromPart]--;
            sizes[toPart]++;

            foreach (var neighbor in graph.GetAdjacencyList(best))
            {
                if (!locked.Contains(neighbor))
                    D[neighbor] += partition.GetPartIndex(neighbor) == toPart ? -2 : 2;
            }

            cumGain += bestD;
            locked.Add(best);
            moves.Add((best, fromPart, toPart, cumGain));
        }

        int maxGain = 0;
        int bestPrefix = 0;
        int[] prefixSizes = (int[])initSizes.Clone();

        for (int i = 0; i < moves.Count; i++)
        {
            prefixSizes[moves[i].fromPart]--;
            prefixSizes[moves[i].toPart]++;
            if (Math.Abs(prefixSizes[0] - prefixSizes[1]) <= 1 && moves[i].cumGain > maxGain)
            {
                maxGain = moves[i].cumGain;
                bestPrefix = i + 1;
            }
        }

        for (int i = moves.Count - 1; i >= 0; i--)
            partition.MoveVertex(moves[i].vertex, moves[i].fromPart);

        if (maxGain <= 0) return 0;

        for (int i = 0; i < bestPrefix; i++)
            partition.MoveVertex(moves[i].vertex, moves[i].toPart);

        return bestPrefix;
    }

    private static Dictionary<Vertex, int> ComputeAllD(List<Vertex> vertices, GraphPartition partition, Graph graph)
    {
        var D = new Dictionary<Vertex, int>(vertices.Count);
        foreach (var v in vertices)
        {
            int partIndex = partition.GetPartIndex(v);
            int ext = 0, intr = 0;
            foreach (var neighbor in graph.GetAdjacencyList(v))
            {
                if (partition.GetPartIndex(neighbor) == partIndex) intr++;
                else ext++;
            }
            D[v] = ext - intr;
        }
        return D;
    }
}
