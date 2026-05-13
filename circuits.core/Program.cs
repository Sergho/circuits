public class Program
{
    public static void Main()
    {
        var generator = new FisherYatesGraphGenerator(1000, 10000);
        var graph = generator.Generate();
        var logger = new FileGraphLogger("./graph.txt");
        
        logger.Log(graph);

        var partitionGenerator = new KernighanLinGraphPartitionGenerator(2);
        var stats = new GraphPartitionStats(graph, partitionGenerator);
        Console.WriteLine(stats.CrossEdgesCount);
        Console.WriteLine(stats.GenerationTimeMs);
        Console.WriteLine(stats.IterationsCount);

        int partIndex = 1;
        foreach(var part in stats.Partition.Parts)
        {
            var partLogger = new FileGraphLogger($"./part-{partIndex}.txt");
            partLogger.Log(part);

            partIndex++;
        }
    }
}