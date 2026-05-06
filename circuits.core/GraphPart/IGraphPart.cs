public interface IGraphPart : IGraphLoggable
{
    IEnumerable<IVertex> Vertices { get; }

    bool HasVertex(IVertex vertex);
    void AssignVertex(IVertex vertex);
    void RemoveVertex(IVertex vertex);
}