public class AlgorithmMetricChart : Control
{
    private int[]? sizes;
    private double[]? klValues;
    private double[]? fmValues;
    private string metric = "";

    private static readonly Color KLColor = Color.FromArgb(70, 130, 200);
    private static readonly Color FMColor = Color.FromArgb(210, 70, 60);

    public AlgorithmMetricChart()
    {
        DoubleBuffered = true;
        BackColor = Color.White;
    }

    public void SetData(int[] s, double[] kl, double[] fm, string metricName)
    {
        sizes = s; klValues = kl; fmValues = fm; metric = metricName;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        if (sizes == null)
        {
            using var f = new Font("Segoe UI", 10);
            string hint = "Запустите тест для построения графика";
            var sz = g.MeasureString(hint, f);
            g.DrawString(hint, f, Brushes.Gray, (Width - sz.Width) / 2f, (Height - sz.Height) / 2f);
            return;
        }

        DrawChart(g);
    }

    private void DrawChart(Graphics g)
    {
        const int ml = 72, mr = 20, mt = 32, mb = 46;
        int cw = Width - ml - mr;
        int ch = Height - mt - mb;
        if (cw < 10 || ch < 10) return;

        using var titleFont = new Font("Segoe UI", 9, FontStyle.Bold);
        using var axisFont  = new Font("Segoe UI", 7.5f);
        using var gridPen   = new Pen(Color.FromArgb(220, 220, 220));
        using var bgBrush   = new SolidBrush(Color.FromArgb(250, 250, 250));

        var ts = g.MeasureString(metric, titleFont);
        g.DrawString(metric, titleFont, Brushes.Black, (Width - ts.Width) / 2f, 6);

        g.FillRectangle(bgBrush, ml, mt, cw, ch);
        g.DrawRectangle(Pens.LightGray, ml, mt, cw, ch);

        double maxY = Math.Max(klValues!.Concat(fmValues!).DefaultIfEmpty(1).Max(), 1);

        for (int i = 0; i <= 5; i++)
        {
            int y = mt + ch * i / 5;
            g.DrawLine(gridPen, ml, y, ml + cw, y);
            double val = maxY * (5 - i) / 5;
            string vs = val >= 100 ? val.ToString("F0") : val.ToString("F1");
            var vsz = g.MeasureString(vs, axisFont);
            g.DrawString(vs, axisFont, Brushes.DimGray, ml - vsz.Width - 3, y - vsz.Height / 2f);
        }

        float denom = sizes!.Length > 1 ? sizes.Length - 1 : 1;
        for (int i = 0; i < sizes.Length; i++)
        {
            float xp = ml + i / denom * cw;
            g.DrawLine(gridPen, (int)xp, mt, (int)xp, mt + ch);
            var nStr = $"n={sizes[i]}";
            var nSz = g.MeasureString(nStr, axisFont);
            g.DrawString(nStr, axisFont, Brushes.DimGray, xp - nSz.Width / 2f, mt + ch + 4);
        }

        DrawSeries(g, klValues, maxY, ml, mt, cw, ch, KLColor);
        DrawSeries(g, fmValues, maxY, ml, mt, cw, ch, FMColor);
        DrawLegend(g, ml + cw - 180, mt + 6);
    }

    private void DrawSeries(Graphics g, double[] vals, double maxY, int ml, int mt, int cw, int ch, Color color)
    {
        if (sizes!.Length == 0) return;
        using var pen   = new Pen(color, 2.5f);
        using var brush = new SolidBrush(color);
        float denom = sizes.Length > 1 ? sizes.Length - 1 : 1;
        var pts = new PointF[sizes.Length];
        for (int i = 0; i < sizes.Length; i++)
            pts[i] = new PointF(ml + i / denom * cw, mt + ch - (float)(vals[i] / maxY) * ch);
        if (pts.Length >= 2) g.DrawLines(pen, pts);
        foreach (var p in pts) g.FillEllipse(brush, p.X - 4, p.Y - 4, 8, 8);
    }

    private static void DrawLegend(Graphics g, int x, int y)
    {
        using var font  = new Font("Segoe UI", 8);
        using var klPen = new Pen(KLColor, 2.5f);
        using var fmPen = new Pen(FMColor, 2.5f);
        using var klBr  = new SolidBrush(KLColor);
        using var fmBr  = new SolidBrush(FMColor);

        g.DrawLine(klPen, x, y + 8, x + 18, y + 8);
        g.FillEllipse(klBr, x + 5, y + 4, 8, 8);
        g.DrawString("Керниган–Лин", font, Brushes.Black, x + 24, y);

        g.DrawLine(fmPen, x, y + 22, x + 18, y + 22);
        g.FillEllipse(fmBr, x + 5, y + 18, 8, 8);
        g.DrawString("Фиддуча–Мэтьюс", font, Brushes.Black, x + 24, y + 14);
    }
}
