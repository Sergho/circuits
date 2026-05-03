public interface GraphPartitionGenerator
{
    int LastIterationsCount { get; }
    GraphPartition Generate(Graph graph);
}