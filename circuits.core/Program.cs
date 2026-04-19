public class Program
{
    public static void Main()
    {
        var generator = new RandomGraphGenerator(10, 20);
        var graph = generator.Generate();
        var logger = new GraphLogger("./graph.txt");
        
        logger.Log(graph);
    }
}