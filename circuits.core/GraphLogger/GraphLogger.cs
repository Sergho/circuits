using System.Text;

public class GraphLogger
{
    private readonly FileStream fileStream;
    private readonly StreamWriter streamWriter;
    private bool isDisposed;

    public string FilePath { get; }
    private string DirPath { get => Path.GetDirectoryName(FilePath) ?? string.Empty; }

    public GraphLogger(string filepath)
    {
        FilePath = filepath;
        fileStream = CreateFileStream();
        streamWriter = CreateStreamWriter();
        isDisposed = false;
    }

    private FileStream CreateFileStream()
    {
        CreateDirectory();

        return new FileStream(
            FilePath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 4096
        );
    }

    private void CreateDirectory()
    {
        if (string.IsNullOrEmpty(DirPath))
        {
            throw new ArgumentException("Unable to create directory with empty path");
        }

        if (!Directory.Exists(DirPath))
        {
            Directory.CreateDirectory(DirPath);
        }
    }

    private StreamWriter CreateStreamWriter()
    {
        return new StreamWriter(fileStream, Encoding.UTF8) { AutoFlush = true };
    }

    public void Log(Graph graph)
    {
        ThrowIfDisposed();

        try
        {
            LogGraphMeta(graph);
            LogGraphEdges(graph);
        }
        catch (IOException ex)
        {
            throw new InvalidOperationException($"Error during graph logging at {FilePath}", ex);
        }
    }

    private void ThrowIfDisposed()
    {
        if (isDisposed)
            throw new ObjectDisposedException(nameof(GraphLogger));
    }

    private void LogGraphMeta(Graph graph)
    {
        streamWriter.WriteLine($"{graph.VerticesCount} {graph.EdgesCount}");
    }

    private void LogGraphEdges(Graph graph)
    {
        foreach (var edge in graph)
        {
            streamWriter.WriteLine($"{edge.First.Index} {edge.Second.Index}");
        }
    }

    public void Dispose()
    {
        if (isDisposed) return;

        streamWriter?.Dispose();
        fileStream?.Dispose();
        isDisposed = true;

        GC.SuppressFinalize(this);
    }

    ~GraphLogger() => Dispose();
}