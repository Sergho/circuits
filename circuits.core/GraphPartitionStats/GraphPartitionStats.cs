using System.Diagnostics;

public class GraphPartitionStats : IGraphPartitionStats
{    
    public IGraphPartition Partition { get; private set; }
    public int CrossEdgesCount { get; private set; }
    public int GenerationTimeMs { get; private set; }
    public int IterationsCount { get; private set; }

    public GraphPartitionStats(IGraphPartition partition)
    {
        Partition = partition;
        GenerationTimeMs = 0;
        IterationsCount = 0;

        CalculateStats();
    }

    public GraphPartitionStats(IGraph graph, IGraphPartitionGenerator partitionGenerator)
    {
        var (partition, generationTimeMs) = MeasureGenerationTimeMs(graph, partitionGenerator);

        Partition = partition;
        GenerationTimeMs = generationTimeMs;
        IterationsCount = partitionGenerator.LastIterationsCount;

        CalculateStats();
    }

    private (IGraphPartition, int) MeasureGenerationTimeMs(IGraph graph, IGraphPartitionGenerator partitionGenerator)
    {
        var stopwatch = Stopwatch.StartNew();
        IGraphPartition partition = GeneratePartition(graph, partitionGenerator);
        stopwatch.Stop();

        return (partition, (int)stopwatch.Elapsed.TotalMilliseconds);
    }

    private IGraphPartition GeneratePartition(IGraph graph, IGraphPartitionGenerator partitionGenerator)
    {
        return partitionGenerator.Generate(graph);
    }

    private void CalculateStats()
    {
        CrossEdgesCount = Partition.CrossEdgesCount;
    }
}