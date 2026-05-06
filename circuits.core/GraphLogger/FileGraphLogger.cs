using System.Text;

public class FileGraphLogger : IGraphLogger
{
    private FileStream fileStream;
    private StreamWriter streamWriter;
    private bool isDisposed;
    private string filePath;

    public FileGraphLogger(string filepath)
    {
        filePath = filepath;
        fileStream = CreateFileStream();
        streamWriter = CreateStreamWriter();
        isDisposed = false;
    }

    private FileStream CreateFileStream()
    {
        CreateDirectory();

        return new FileStream(
            filePath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 4096
        );
    }

    private void CreateDirectory()
    {
        if (string.IsNullOrEmpty(GetDirPath()))
        {
            throw new ArgumentException("Unable to create directory with empty path");
        }

        if (!Directory.Exists(GetDirPath()))
        {
            Directory.CreateDirectory(GetDirPath());
        }
    }

    private string GetDirPath()
    {
        return Path.GetDirectoryName(filePath) ?? string.Empty;
    }

    private StreamWriter CreateStreamWriter()
    {
        return new StreamWriter(fileStream, Encoding.UTF8) { AutoFlush = true };
    }

    public void Log(IGraphLoggable graph)
    {
        ThrowIfDisposed();

        try
        {
            LogGraphMeta(graph);
            LogGraphEdges(graph);
        }
        catch (IOException ex)
        {
            throw new InvalidOperationException($"Error during graph logging at {filePath}", ex);
        }
    }

    private void ThrowIfDisposed()
    {
        if (isDisposed)
            throw new ObjectDisposedException(nameof(FileGraphLogger));
    }

    private void LogGraphMeta(IGraphLoggable graph)
    {
        streamWriter.WriteLine($"{graph.VerticesCount} {graph.EdgesCount}");
    }

    private void LogGraphEdges(IGraphLoggable graph)
    {
        foreach (var edge in graph.Edges)
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

    ~FileGraphLogger() => Dispose();
}