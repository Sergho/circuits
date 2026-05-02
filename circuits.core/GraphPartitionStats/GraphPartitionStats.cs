public class GraphPartitionStats
{
    private readonly GraphPartition partition;
    
    public int CrossEdgesCount { get; private set; }

    public GraphPartitionStats(GraphPartition partition)
    {
        this.partition = partition;

        calculateStats();
    }

    private void calculateStats()
    {
        CrossEdgesCount = partition.GetCrossEdgesCount();
    }
}