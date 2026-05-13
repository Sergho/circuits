public class GraphCanvas : Control
{
    private IGraph? graph;
    private IGraphPartition? partition;
    private readonly Dictionary<IVertex, PointF> positions = new();

    private static readonly Color ColorPartA = Color.FromArgb(70, 130, 200);
    private static readonly Color ColorPartB = Color.FromArgb(210, 70, 60);
    private static readonly Color ColorCutEdge = Color.FromArgb(240, 150, 30);
    private static readonly Color ColorInternalEdge = Color.FromArgb(200, 200, 200);
    private const int R = 11;

    public GraphCanvas()
    {
        DoubleBuffered = true;
        BackColor = Color.White;
    }

    public void SetData(IGraph g, IGraphPartition p)
    {
        graph = g;
        partition = p;
        ComputeLayout();
        Invalidate();
    }

    private void ComputeLayout()
    {
        positions.Clear();
        if (graph == null || partition == null || Width < 10 || Height < 10) return;

        var parts = partition.Parts.ToArray();
        var colA = parts[0].Vertices.OrderBy(v => v.Index).ToList();
        var colB = parts[1].Vertices.OrderBy(v => v.Index).ToList();

        PlaceColumn(colA, Width * 0.25f);
        PlaceColumn(colB, Width * 0.75f);
    }

    private void PlaceColumn(List<IVertex> verts, float x)
    {
        if (verts.Count == 0) return;
        float margin = 30f;
        float step = (Height - 2 * margin) / Math.Max(verts.Count, 1);
        for (int i = 0; i < verts.Count; i++)
            positions[verts[i]] = new PointF(x, margin + step * i + step / 2f);
    }

    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        ComputeLayout();
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        if (graph == null || partition == null)
        {
            DrawCentered(g, "Нажмите «Запустить» для отображения графа");
            return;
        }

        if (graph.VerticesCount > 100)
        {
            DrawCentered(g, $"Граф слишком большой для визуализации (n = {graph.VerticesCount} > 100)");
            return;
        }

        DrawEdges(g);
        DrawVertices(g);
        DrawLegend(g);
    }

    private void DrawCentered(Graphics g, string text)
    {
        using var font = new Font("Segoe UI", 11);
        var sz = g.MeasureString(text, font);
        g.DrawString(text, font, Brushes.Gray, (Width - sz.Width) / 2f, (Height - sz.Height) / 2f);
    }

    private void DrawEdges(Graphics g)
    {
        using var cutPen = new Pen(ColorCutEdge, 1.8f);
        using var intPen = new Pen(ColorInternalEdge, 1f);

        foreach (var edge in graph!.Edges)
        {
            if (!positions.TryGetValue(edge.First, out var p1)) continue;
            if (!positions.TryGetValue(edge.Second, out var p2)) continue;
            bool cut = partition!.GetPartIndex(edge.First) != partition.GetPartIndex(edge.Second);
            g.DrawLine(cut ? cutPen : intPen, p1, p2);
        }
    }

    private void DrawVertices(Graphics g)
    {
        using var labelFont = new Font("Segoe UI", 7, FontStyle.Bold);
        foreach (var (vertex, pos) in positions)
        {
            int idx = partition!.GetPartIndex(vertex);
            var fill = idx == 0 ? ColorPartA : ColorPartB;
            var rect = new RectangleF(pos.X - R, pos.Y - R, R * 2, R * 2);
            using var brush = new SolidBrush(fill);
            g.FillEllipse(brush, rect);
            g.DrawEllipse(Pens.DimGray, rect);
            string label = vertex.Index.ToString();
            var ts = g.MeasureString(label, labelFont);
            g.DrawString(label, labelFont, Brushes.White, pos.X - ts.Width / 2f, pos.Y - ts.Height / 2f);
        }
    }

    private void DrawLegend(Graphics g)
    {
        int x = 10, y = 10, box = 13, lh = 20;
        using var font = new Font("Segoe UI", 8);

        void Item(Color color, string text, bool line)
        {
            if (line)
            {
                using var pen = new Pen(color, 2);
                g.DrawLine(pen, x, y + lh / 2, x + box, y + lh / 2);
            }
            else
            {
                using var br = new SolidBrush(color);
                g.FillRectangle(br, x, y + (lh - box) / 2, box, box);
                g.DrawRectangle(Pens.DimGray, x, y + (lh - box) / 2, box, box);
            }
            g.DrawString(text, font, Brushes.Black, x + box + 4, y + 2);
            y += lh;
        }

        Item(ColorPartA, "Часть A", false);
        Item(ColorPartB, "Часть B", false);
        Item(ColorInternalEdge, "Внутренние рёбра", true);
        Item(ColorCutEdge, "Межсоединения", true);
    }
}
