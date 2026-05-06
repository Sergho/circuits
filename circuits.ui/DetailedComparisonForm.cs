public class DetailedComparisonForm : Form
{
    private DataGridView grid = null!;
    private ProgressBar progressBar = null!;
    private Button runButton = null!;
    private Label statusLabel = null!;

    public DetailedComparisonForm()
    {
        Text = "Детальное сравнение алгоритмов";
        Size = new Size(860, 480);
        MinimumSize = new Size(700, 380);
        StartPosition = FormStartPosition.CenterParent;
        BuildUI();
    }

    private void BuildUI()
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Padding = new Padding(8),
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));

        runButton = new Button
        {
            Text = "Запустить тест (3 прогона на каждый размер)",
            Dock = DockStyle.Fill,
        };
        runButton.Click += OnRun;
        layout.Controls.Add(runButton, 0, 0);

        progressBar = new ProgressBar { Dock = DockStyle.Fill, Minimum = 0 };
        layout.Controls.Add(progressBar, 0, 1);

        grid = BuildGrid();
        layout.Controls.Add(grid, 0, 2);

        statusLabel = new Label
        {
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = Color.DimGray,
        };
        layout.Controls.Add(statusLabel, 0, 3);

        Controls.Add(layout);
    }

    private DataGridView BuildGrid()
    {
        var dg = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            RowHeadersVisible = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            DefaultCellStyle = { Font = new Font("Segoe UI", 9) },
            ColumnHeadersDefaultCellStyle = { Font = new Font("Segoe UI", 9, FontStyle.Bold), BackColor = Color.FromArgb(240, 240, 245) },
            EnableHeadersVisualStyles = false,
        };

        (string Header, float Weight)[] cols =
        [
            ("Вершин",        45),
            ("Рёбер",         45),
            ("KL: Межсоед.",  80),
            ("KL: Время (мс)", 80),
            ("KL: Итерации",  75),
            ("FM: Межсоед.",  80),
            ("FM: Время (мс)", 80),
            ("FM: Итерации",  75),
            ("FM лучше на",   70),
        ];

        foreach (var (header, weight) in cols)
        {
            dg.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = header,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                FillWeight = weight,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter },
            });
        }

        return dg;
    }

    private static readonly Color GreenText = Color.FromArgb(0, 130, 0);
    private static readonly Color RedText   = Color.FromArgb(190, 40, 40);

    private async void OnRun(object? sender, EventArgs e)
    {
        runButton.Enabled = false;
        grid.Rows.Clear();
        statusLabel.Text = "Выполняется...";

        int[] sizes = [10, 25, 50, 100, 200, 400, 600];
        const double prob = 0.15;
        const int runs = 3;

        progressBar.Maximum = sizes.Length * runs;
        progressBar.Value = 0;

        await Task.Run(() =>
        {
            foreach (int n in sizes)
            {
                long klMs = 0, fmMs = 0;
                int klCut = 0, fmCut = 0;
                int klIter = 0, fmIter = 0;
                int edges = 0;

                for (int r = 0; r < runs; r++)
                {
                    var graph = new ErdosRenyiProbabilityGenerator(n, prob).Generate();
                    edges = graph.EdgesCount;

                    var sw = Stopwatch.StartNew();
                    var klGen = new KernighanLinGraphPartitionGenerator(2);
                    var klPart = klGen.Generate(graph);
                    sw.Stop();
                    klMs   += sw.ElapsedMilliseconds;
                    klCut  += klPart.CutSize;
                    klIter += klGen.IterationsCount;

                    sw.Restart();
                    var fmGen = new FiducciaMattheysesGraphPartitionGenerator();
                    var fmPart = fmGen.Generate(graph);
                    sw.Stop();
                    fmMs   += sw.ElapsedMilliseconds;
                    fmCut  += fmPart.CutSize;
                    fmIter += fmGen.IterationsCount;

                    Invoke(() => progressBar.Value++);
                }

                double avgKlCut  = (double)klCut  / runs;
                double avgFmCut  = (double)fmCut  / runs;
                double avgKlMs   = (double)klMs   / runs;
                double avgFmMs   = (double)fmMs   / runs;
                double avgKlIter = (double)klIter / runs;
                double avgFmIter = (double)fmIter / runs;
                double delta = avgKlCut > 0 ? (avgKlCut - avgFmCut) / avgKlCut * 100.0 : 0.0;

                Invoke(() =>
                {
                    int row = grid.Rows.Add(
                        n,
                        edges,
                        avgKlCut.ToString("F1"),
                        avgKlMs.ToString("F1"),
                        avgKlIter.ToString("F1"),
                        avgFmCut.ToString("F1"),
                        avgFmMs.ToString("F1"),
                        avgFmIter.ToString("F1"),
                        (delta >= 0 ? "+" : "") + delta.ToString("F1") + "%"
                    );

                    var r = grid.Rows[row];

                    if (avgFmCut < avgKlCut)      r.Cells[5].Style.ForeColor = GreenText;
                    else if (avgKlCut < avgFmCut) r.Cells[2].Style.ForeColor = GreenText;

                    if (avgFmMs < avgKlMs)        r.Cells[6].Style.ForeColor = GreenText;
                    else if (avgKlMs < avgFmMs)   r.Cells[3].Style.ForeColor = GreenText;

                    r.Cells[8].Style.ForeColor = delta > 0 ? GreenText : delta < 0 ? RedText : Color.DimGray;
                });
            }

            Invoke(() =>
            {
                statusLabel.Text = $"Готово. Каждый размер тестировался {runs} раза на графах Эрдёша–Реньи (p = {prob}). " +
                                   "Зелёным выделено лучшее значение по столбцу.";
                runButton.Enabled = true;
            });
        });
    }
}
