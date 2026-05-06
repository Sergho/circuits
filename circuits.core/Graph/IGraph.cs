public interface IGraph : IGraphLoggable
{
    IEnumerable<IVertex> Vertices { get; }

    bool HasVertex(IVertex vertex);
    bool HasEdge(IEdge edge);
    void AddEdge(IEdge edge);
    IEnumerable<IVertex> GetAdjacencyList(IVertex vertex);
}