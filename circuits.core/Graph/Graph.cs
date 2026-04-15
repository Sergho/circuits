using System.Collections;

public class Graph : IEnumerable<(int, int)>
{
    private readonly List<int>[] adjacencyList;

    public int VerticesCount { get; private set; }
    public int EdgesCount { get; private set; }

    public static Graph Empty(int verticesCount)
    {
        return new Graph(verticesCount);
    }
    
    private Graph(int verticesCount)
    {
        VerticesCount = verticesCount;
        EdgesCount = 0;

        adjacencyList = getEmptyAdjencyList();
    }

    private List<int>[] getEmptyAdjencyList()
    {
        var result = new List<int>[VerticesCount];

        for (int i = 0; i < VerticesCount; i++)
        {
            adjacencyList[i] = [];
        }

        return result;
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

    public IEnumerator<(int, int)> GetEnumerator()
    {
        for (int vertexIndex = 0; vertexIndex < VerticesCount; vertexIndex++)
        {
            foreach (var neighbor in adjacencyList[vertexIndex])
            {
                if (vertexIndex >= neighbor) continue;
                
                yield return (vertexIndex, neighbor);
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}