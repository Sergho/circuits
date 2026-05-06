public class ComparisonChart : Control
{
    private int[]? sizes;
    private int[]? klCuts;
    private int[]? fmCuts;
    private int[]? edgeCounts;

    private static readonly Color KLColor = Color.FromArgb(70, 130, 200);
    private static readonly Color FMColor = Color.FromArgb(210, 70, 60);

    public ComparisonChart()
    {
        DoubleBuffered = true;
        BackColor = Color.White;
    }

    public void SetData(int[] s, int[] kl, int[] fm, int[] edges)
    {
        sizes      = s;
        klCuts     = kl;
        fmCuts     = fm;
        edgeCounts = edges;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        if (sizes == null || klCuts == null || fmCuts == null)
        {
            using var f = new Font("Segoe UI", 11);
            var s = g.MeasureString("Нажмите «Сравнить алгоритмы» для построения графика", f);
            g.DrawString("Нажмите «Сравнить алгоритмы» для построения графика", f, Brushes.Gray,
                (Width - s.Width) / 2f, (Height - s.Height) / 2f);
            return;
        }

        DrawChart(g);
    }

    private void DrawChart(Graphics g)
    {
        const int ml = 65, mr = 20, mt = 45, mb = 70;
        int cw = Width - ml - mr;
        int ch = Height - mt - mb;
        if (cw < 10 || ch < 10) return;

        using var titleFont  = new Font("Segoe UI", 11, FontStyle.Bold);
        using var axisFont   = new Font("Segoe UI", 8);
        using var edgeFont   = new Font("Segoe UI", 7.5f);
        using var labelFont  = new Font("Segoe UI", 9);
        using var gridPen    = new Pen(Color.FromArgb(220, 220, 220));
        using var bgBrush    = new SolidBrush(Color.FromArgb(250, 250, 250));
        using var edgeBrush  = new SolidBrush(Color.FromArgb(160, 160, 160));

        string title = "Зависимость числа межсоединений от размера графа";
        var ts = g.MeasureString(title, titleFont);
        g.DrawString(title, titleFont, Brushes.Black, (Width - ts.Width) / 2f, 8);

        g.FillRectangle(bgBrush, ml, mt, cw, ch);
        g.DrawRectangle(Pens.LightGray, ml, mt, cw, ch);

        int maxY = Math.Max(klCuts!.Max(), fmCuts!.Max());
        maxY = Math.Max(maxY, 1);

        for (int i = 0; i <= 5; i++)
        {
            int y = mt + ch * i / 5;
            g.DrawLine(gridPen, ml, y, ml + cw, y);
            int val = maxY * (5 - i) / 5;
            var vs  = val.ToString();
            var vsz = g.MeasureString(vs, axisFont);
            g.DrawString(vs, axisFont, Brushes.DimGray, ml - vsz.Width - 3, y - vsz.Height / 2f);
        }

        float nLineY = mt + ch + 4;
        float mLineY = nLineY + g.MeasureString("0", axisFont).Height + 1;

        for (int i = 0; i < sizes!.Length; i++)
        {
            float xp = ml + (float)i / (sizes.Length - 1) * cw;
            g.DrawLine(gridPen, (int)xp, mt, (int)xp, mt + ch);

            var nStr = $"n={sizes[i]}";
            var nSz  = g.MeasureString(nStr, axisFont);
            g.DrawString(nStr, axisFont, Brushes.DimGray, xp - nSz.Width / 2f, nLineY);

            if (edgeCounts != null)
            {
                var mStr = $"m={edgeCounts[i]}";
                var mSz  = g.MeasureString(mStr, edgeFont);
                g.DrawString(mStr, edgeFont, edgeBrush, xp - mSz.Width / 2f, mLineY);
            }
        }

        string axisHint = "n — число вершин,  m — число рёбер";
        var asz = g.MeasureString(axisHint, edgeFont);
        g.DrawString(axisHint, edgeFont, edgeBrush, ml + (cw - asz.Width) / 2f, Height - 16);

        var state = g.Save();
        g.TranslateTransform(12, mt + ch / 2f);
        g.RotateTransform(-90);
        g.DrawString("Межсоединений", labelFont, Brushes.DimGray, -52, -7);
        g.Restore(state);

        DrawSeries(g, klCuts, maxY, ml, mt, cw, ch, KLColor);
        DrawSeries(g, fmCuts, maxY, ml, mt, cw, ch, FMColor);
        DrawLegend(g, ml + cw - 185, mt + 10);
    }

    private void DrawSeries(Graphics g, int[] cuts, int maxY, int ml, int mt, int cw, int ch, Color color)
    {
        if (sizes!.Length < 2) return;
        using var pen   = new Pen(color, 2.5f);
        using var brush = new SolidBrush(color);
        var pts = new PointF[sizes.Length];
        for (int i = 0; i < sizes.Length; i++)
            pts[i] = new PointF(ml + (float)i / (sizes.Length - 1) * cw,
                                mt + ch - (float)cuts[i] / maxY * ch);
        g.DrawLines(pen, pts);
        foreach (var p in pts)
            g.FillEllipse(brush, p.X - 4, p.Y - 4, 8, 8);
    }

    private static void DrawLegend(Graphics g, int x, int y)
    {
        using var font  = new Font("Segoe UI", 9);
        using var klBr  = new SolidBrush(KLColor);
        using var fmBr  = new SolidBrush(FMColor);
        using var klPen = new Pen(KLColor, 2.5f);
        using var fmPen = new Pen(FMColor, 2.5f);

        g.DrawLine(klPen, x, y + 8, x + 20, y + 8);
        g.FillEllipse(klBr, x + 6, y + 4, 8, 8);
        g.DrawString("Керниган–Лин", font, Brushes.Black, x + 26, y);

        g.DrawLine(fmPen, x, y + 30, x + 20, y + 30);
        g.FillEllipse(fmBr, x + 6, y + 26, 8, 8);
        g.DrawString("Фиддуча–Мэтьюс", font, Brushes.Black, x + 26, y + 22);
    }
}
