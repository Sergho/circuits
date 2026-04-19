public class Program
{
    public static void Main()
    {
        var generator = new ChainGraphGenerator(10);
        var graph = generator.Generate();
        var logger = new GraphLogger("./graph.txt");
        
        logger.Log(graph);
    }
}