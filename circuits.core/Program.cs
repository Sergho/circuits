public class Program
{
    public static void Main()
    {
        var generator = new RegularGridGraphGenerator(10, 20);
        var graph = generator.Generate();
        var logger = new GraphLogger("./graph.txt");
        
        logger.Log(graph);

        var partitionGenerator = new KernighanLinGraphPartitionGenerator(2);
        var partition = partitionGenerator.Generate(graph);
        int partIndex = 1;
        foreach(var part in partition.Parts)
        {
            var partLogger = new GraphLogger($"./part-{partIndex}.txt");
            partLogger.Log(part);

            partIndex++;
        }
    }
}