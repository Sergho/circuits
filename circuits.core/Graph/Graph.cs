public class Graph
{
    private List<int>[] adjacencyList;

    public int verticesCount { get; private set; }
    public int edgesCount { get; private set; }
    
    private Graph(int verticesCount)
    {
        this.verticesCount = verticesCount;
        edgesCount = 0;

        adjacencyList = [];
    } 

    public static Graph empty(int verticesCount)
    {
        return new Graph(verticesCount);
    }

    public void addEdge(int firstVertex, int secondVertex)
    {
        if (!adjacencyList[firstVertex].Contains(secondVertex))
        {
            adjacencyList[firstVertex].Add(secondVertex);
            adjacencyList[secondVertex].Add(firstVertex);

            edgesCount++;
        }
    }
}