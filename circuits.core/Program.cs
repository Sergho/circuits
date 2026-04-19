public class Program
{
    public static void Main()
    {
        var generator = new BipartiteGraphGenerator(10, 20, 40);
        var graph = generator.Generate();
        var logger = new GraphLogger("./graph.txt");
        
        logger.Log(graph);
    }
}