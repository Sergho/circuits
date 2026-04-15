public class Graph
{
    private List<int>[] adjacencyList;

    public int VerticesCount { get; private set; }
    public int EdgesCount { get; private set; }
    
    private Graph(int verticesCount)
    {
        VerticesCount = verticesCount;
        EdgesCount = 0;

        adjacencyList = [];
    } 

    public static Graph Empty(int verticesCount)
    {
        return new Graph(verticesCount);
    }

    public void AddEdge(int firstVertex, int secondVertex)
    {
        if (!adjacencyList[firstVertex].Contains(secondVertex))
        {
            adjacencyList[firstVertex].Add(secondVertex);
            adjacencyList[secondVertex].Add(firstVertex);

            EdgesCount++;
        }
    }
}