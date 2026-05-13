public interface IGraphPartitionStats
{
    int CrossEdgesCount { get; }
    int GenerationTimeMs { get; }
    int IterationsCount { get; }
    IGraphPartition Partition { get; }
}