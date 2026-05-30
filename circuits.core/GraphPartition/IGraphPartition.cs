public interface IGraphPartition
{
    IGraph Graph { get; }
    int PartsCount { get; }
    IEnumerable<IGraphPart> Parts { get; }
    int CrossEdgesCount { get; }

    void SwapVertices(IVertex first, IVertex second);
    int CalculateGain(IVertex first, IVertex second);
    IGraphPart GetPart(IVertex vertex);
    int GetPartIndex(IVertex vertex);
    void MoveVertex(IVertex vertex, int toPartIndex);
}