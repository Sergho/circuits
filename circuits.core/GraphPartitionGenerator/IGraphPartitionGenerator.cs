public interface IGraphPartitionGenerator
{
    int LastIterationsCount { get; }
    IGraphPartition Generate(IGraph graph);
}