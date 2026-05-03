using System.Diagnostics;

public class GraphPartitionStats
{    
    public GraphPartition Partition { get; private set; }
    public int CrossEdgesCount { get; private set; }
    public int GenerationTimeMs { get; private set; }
    public int IterationsCount { get; private set; }

    public GraphPartitionStats(GraphPartition partition)
    {
        Partition = partition;
        GenerationTimeMs = 0;
        IterationsCount = 0;

        CalculateStats();
    }

    public GraphPartitionStats(Graph graph, GraphPartitionGenerator partitionGenerator)
    {
        var (partition, generationTimeMs) = MeasureGenerationTimeMs(graph, partitionGenerator);

        Partition = partition;
        GenerationTimeMs = generationTimeMs;
        IterationsCount = partitionGenerator.LastIterationsCount;

        CalculateStats();
    }

    private (GraphPartition, int) MeasureGenerationTimeMs(Graph graph, GraphPartitionGenerator partitionGenerator)
    {
        var stopwatch = Stopwatch.StartNew();
        GraphPartition partition = GeneratePartition(graph, partitionGenerator);
        stopwatch.Stop();

        return (partition, (int)stopwatch.Elapsed.TotalMilliseconds);
    }

    private GraphPartition GeneratePartition(Graph graph, GraphPartitionGenerator partitionGenerator)
    {
        return partitionGenerator.Generate(graph);
    }

    private void CalculateStats()
    {
        CrossEdgesCount = Partition.GetCrossEdgesCount();
    }
}