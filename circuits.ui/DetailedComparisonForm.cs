public class DetailedComparisonForm : Form
{
    private DataGridView grid = null!;
    private ProgressBar progressBar = null!;
    private Button runButton = null!;
    private Label statusLabel = null!;
    private ComboBox generatorCombo = null!;
    private NumericUpDown probUpDown = null!;
    private Label probLabel = null!;

    public DetailedComparisonForm()
    {
        Text = "Детальное сравнение алгоритмов";
        Size = new Size(900, 520);
        MinimumSize = new Size(720, 400);
        StartPosition = FormStartPosition.CenterParent;
        BuildUI();
    }

    private void BuildUI()
    {
        var outer = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 5,
            Padding = new Padding(8),
        };
        outer.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        outer.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        outer.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));
        outer.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        outer.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));

        outer.Controls.Add(BuildGeneratorPanel(), 0, 0);

        runButton = new Button
        {
            Text = "Запустить тест (3 прогона на каждый размер)",
            Dock = DockStyle.Fill,
        };
        runButton.Click += OnRun;
        outer.Controls.Add(runButton, 0, 1);

        progressBar = new ProgressBar { Dock = DockStyle.Fill, Minimum = 0 };
        outer.Controls.Add(progressBar, 0, 2);

        grid = BuildGrid();
        outer.Controls.Add(grid, 0, 3);

        statusLabel = new Label
        {
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = Color.DimGray,
        };
        outer.Controls.Add(statusLabel, 0, 4);

        Controls.Add(outer);
    }

    private Panel BuildGeneratorPanel()
    {
        var flow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
        };

        flow.Controls.Add(new Label
        {
            Text = "Генератор:",
            AutoSize = false,
            Width = 80,
            Height = 26,
            TextAlign = ContentAlignment.MiddleLeft,
        });

        generatorCombo = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Width = 180,
            Height = 26,
        };
        generatorCombo.Items.AddRange(new object[]
        {
            "Эрдёша–Реньи",
            "Двудольный граф",
            "Цепочка",
        });
        generatorCombo.SelectedIndex = 0;
        generatorCombo.SelectedIndexChanged += (_, _) =>
        {
            bool er = generatorCombo.SelectedIndex == 0;
            probLabel.Visible = er;
            probUpDown.Visible = er;
        };
        flow.Controls.Add(generatorCombo);

        probLabel = new Label
        {
            Text = "  p =",
            AutoSize = false,
            Width = 36,
            Height = 26,
            TextAlign = ContentAlignment.MiddleLeft,
        };
        flow.Controls.Add(probLabel);

        probUpDown = new NumericUpDown
        {
            Width = 64,
            Height = 26,
            Minimum = 0.01m,
            Maximum = 1m,
            DecimalPlaces = 2,
            Increment = 0.05m,
            Value = 0.15m,
        };
        flow.Controls.Add(probUpDown);

        return flow;
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
            ColumnHeadersDefaultCellStyle =
            {
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(240, 240, 245),
            },
            EnableHeadersVisualStyles = false,
        };

        (string Header, float Weight)[] cols =
        [
            ("Вершин",         45),
            ("Рёбер",          50),
            ("KL: Межсоед.",   80),
            ("KL: Время (мс)", 82),
            ("KL: Ходов",      68),
            ("FM: Межсоед.",   80),
            ("FM: Время (мс)", 82),
            ("FM: Ходов",      68),
            ("FM лучше на",    68),
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
        const int runs = 3;

        progressBar.Maximum = sizes.Length * runs;
        progressBar.Value = 0;

        int genIndex = generatorCombo.SelectedIndex;
        double prob = (double)probUpDown.Value;

        await Task.Run(() =>
        {
            foreach (int n in sizes)
            {
                double klMs = 0, fmMs = 0;
                int klCut = 0, fmCut = 0;
                int klMoves = 0, fmMoves = 0;
                int edges = 0;

                for (int r = 0; r < runs; r++)
                {
                    var graph = genIndex switch
                    {
                        0 => new ErdosRenyiProbabilityGenerator(n, prob).Generate(),
                        1 => new BipartiteGraphGenerator(n / 2, n - n / 2,
                                 (int)(n / 2.0 * (n - n / 2.0) * 0.3)).Generate(),
                        2 => new ChainGraphGenerator(n).Generate(),
                        _ => throw new InvalidOperationException(),
                    };
                    edges = graph.EdgesCount;

                    var sw = Stopwatch.StartNew();
                    var klGen = new KernighanLinGraphPartitionGenerator(2);
                    var klPart = klGen.Generate(graph);
                    sw.Stop();
                    klMs    += sw.Elapsed.TotalMilliseconds;
                    klCut   += klPart.CrossEdgesCount;
                    klMoves += klGen.LastIterationsCount;

                    sw.Restart();
                    var fmGen = new FiducciaMattheysesGraphPartitionGenerator();
                    var fmPart = fmGen.Generate(graph);
                    sw.Stop();
                    fmMs    += sw.Elapsed.TotalMilliseconds;
                    fmCut   += fmPart.CrossEdgesCount;
                    fmMoves += fmGen.LastIterationsCount;

                    Invoke(() => progressBar.Value++);
                }

                double avgKlCut   = (double)klCut   / runs;
                double avgFmCut   = (double)fmCut   / runs;
                double avgKlMs    = klMs    / runs;
                double avgFmMs    = fmMs    / runs;
                double avgKlMoves = (double)klMoves / runs;
                double avgFmMoves = (double)fmMoves / runs;
                double delta = avgKlCut > 0 ? (avgKlCut - avgFmCut) / avgKlCut * 100.0 : 0.0;

                Invoke(() =>
                {
                    int row = grid.Rows.Add(
                        n,
                        edges,
                        avgKlCut.ToString("F1"),
                        avgKlMs.ToString("F2"),
                        avgKlMoves.ToString("F1"),
                        avgFmCut.ToString("F1"),
                        avgFmMs.ToString("F2"),
                        avgFmMoves.ToString("F1"),
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
                string genName = genIndex switch
                {
                    0 => $"Эрдёша–Реньи (p={prob})",
                    1 => "Двудольный граф (плотность 30%)",
                    2 => "Цепочка",
                    _ => ""
                };
                statusLabel.Text = $"Готово. Генератор: {genName}. {runs} прогона на размер, средние значения. " +
                                   "«Ходов» = число применённых перестановок вершин/пар.";
                runButton.Enabled = true;
            });
        });
    }
}
