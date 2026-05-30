public class DetailedComparisonForm : Form
{
    private DataGridView grid = null!;
    private ProgressBar progressBar = null!;
    private Button runButton = null!;
    private Label statusLabel = null!;
    private ComboBox generatorCombo = null!;
    private NumericUpDown param2UpDown = null!;
    private Label param2Label = null!;
    private RadioButton edgeProbRadio = null!;
    private RadioButton edgeCountRadio = null!;
    private TextBox sizesTextBox = null!;
    private AlgorithmMetricChart iterChart = null!;
    private AlgorithmMetricChart timeChart = null!;
    private ComparisonChart cutsChart = null!;
    private TabControl tabControl = null!;

    public DetailedComparisonForm()
    {
        Text = "Детальное сравнение алгоритмов";
        Size = new Size(1000, 620);
        MinimumSize = new Size(720, 450);
        StartPosition = FormStartPosition.CenterParent;
        BuildUI();
    }

    private void BuildUI()
    {
        var outer = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 6,
            Padding = new Padding(8),
        };
        outer.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        outer.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        outer.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        outer.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));
        outer.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        outer.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));

        outer.Controls.Add(BuildGeneratorPanel(), 0, 0);
        outer.Controls.Add(BuildSizesPanel(), 0, 1);

        runButton = new Button
        {
            Text = "Запустить тест (3 прогона на каждый размер)",
            Dock = DockStyle.Fill,
        };
        runButton.Click += OnRun;
        outer.Controls.Add(runButton, 0, 2);

        progressBar = new ProgressBar { Dock = DockStyle.Fill, Minimum = 0 };
        outer.Controls.Add(progressBar, 0, 3);

        tabControl = new TabControl { Dock = DockStyle.Fill };

        var tableTab = new TabPage("Таблица");
        grid = BuildGrid();
        tableTab.Controls.Add(grid);

        var chartTab = new TabPage("Графики");
        chartTab.Controls.Add(BuildChartsPanel());

        var cutsTab = new TabPage("Межсоединения");
        cutsChart = new ComparisonChart { Dock = DockStyle.Fill };
        cutsTab.Controls.Add(cutsChart);

        tabControl.TabPages.Add(tableTab);
        tabControl.TabPages.Add(chartTab);
        tabControl.TabPages.Add(cutsTab);
        outer.Controls.Add(tabControl, 0, 4);

        statusLabel = new Label
        {
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = Color.DimGray,
        };
        outer.Controls.Add(statusLabel, 0, 5);

        Controls.Add(outer);
    }

    private Control BuildChartsPanel()
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

        iterChart = new AlgorithmMetricChart { Dock = DockStyle.Fill };
        timeChart = new AlgorithmMetricChart { Dock = DockStyle.Fill };
        layout.Controls.Add(iterChart, 0, 0);
        layout.Controls.Add(timeChart, 0, 1);
        return layout;
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
            AutoSize = false, Width = 80, Height = 26,
            TextAlign = ContentAlignment.MiddleLeft,
        });

        generatorCombo = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Width = 200, Height = 26,
        };
        generatorCombo.Items.AddRange(new object[]
        {
            "Эрдёша–Реньи",
            "Регулярная решётка",
            "Двудольный граф",
            "Цепочка",
            "Клика",
            "Фишер–Йетс",
        });
        generatorCombo.SelectedIndex = 0;
        generatorCombo.SelectedIndexChanged += OnGeneratorComboChanged;
        flow.Controls.Add(generatorCombo);

        param2Label = new Label
        {
            Text = "  p =",
            AutoSize = false, Width = 50, Height = 26,
            TextAlign = ContentAlignment.MiddleLeft,
        };
        flow.Controls.Add(param2Label);

        param2UpDown = new NumericUpDown
        {
            Width = 70,
            Minimum = 0.01m, Maximum = 1m,
            DecimalPlaces = 2, Increment = 0.05m, Value = 0.15m,
        };
        flow.Controls.Add(param2UpDown);

        edgeProbRadio  = new RadioButton { Text = "вер.", Width = 54, Height = 26, TextAlign = ContentAlignment.MiddleLeft };
        edgeCountRadio = new RadioButton { Text = "рёбер", Width = 60, Height = 26, TextAlign = ContentAlignment.MiddleLeft, Checked = false };
        edgeProbRadio.Checked = true;
        edgeProbRadio.CheckedChanged  += OnEdgeModeChanged;
        edgeCountRadio.CheckedChanged += OnEdgeModeChanged;
        flow.Controls.Add(edgeProbRadio);
        flow.Controls.Add(edgeCountRadio);

        return flow;
    }

    private void OnGeneratorComboChanged(object? sender, EventArgs e)
    {
        int idx = generatorCombo.SelectedIndex;
        bool hasDensityParam = idx == 0 || idx == 2 || idx == 5;
        param2Label.Visible  = hasDensityParam;
        param2UpDown.Visible = hasDensityParam;
        edgeProbRadio.Visible  = hasDensityParam;
        edgeCountRadio.Visible = hasDensityParam;

        // Reset to probability mode when generator changes
        edgeProbRadio.Checked = true;
        if (hasDensityParam)
        {
            param2UpDown.DecimalPlaces = 2; param2UpDown.Increment = 0.05m;
            param2UpDown.Minimum = 0.01m;  param2UpDown.Maximum = 1m;
            param2UpDown.Value   = idx == 2 ? 0.30m : 0.15m;
            param2Label.Text = idx == 2 ? "  плотность =" : "  p =";
        }
    }

    private void OnEdgeModeChanged(object? sender, EventArgs e)
    {
        if (edgeCountRadio.Checked)
        {
            // Convert current density to a rough count using n=100 as reference
            int refMax = generatorCombo.SelectedIndex == 2 ? 2500 : 4950; // 50*50 or 100*99/2
            int defaultCount = Math.Max(1, (int)Math.Round((double)param2UpDown.Value * refMax));
            param2UpDown.DecimalPlaces = 0;
            param2UpDown.Increment = 10;
            param2UpDown.Minimum = 0;
            param2UpDown.Maximum = 50_000_000;
            param2UpDown.Value = defaultCount;
            param2Label.Text = "  рёбер =";
        }
        else
        {
            int idx = generatorCombo.SelectedIndex;
            param2UpDown.DecimalPlaces = 2; param2UpDown.Increment = 0.05m;
            param2UpDown.Minimum = 0.01m;  param2UpDown.Maximum = 1m;
            param2UpDown.Value = idx == 2 ? 0.30m : 0.15m;
            param2Label.Text = idx == 2 ? "  плотность =" : "  p =";
        }
    }

    private Panel BuildSizesPanel()
    {
        var flow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
        };

        flow.Controls.Add(new Label
        {
            Text = "Размеры n:",
            AutoSize = false, Width = 80, Height = 26,
            TextAlign = ContentAlignment.MiddleLeft,
        });

        sizesTextBox = new TextBox
        {
            Text = "10,25,50,100,200,400,600",
            Width = 300,
        };
        flow.Controls.Add(sizesTextBox);

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
            dg.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = header,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                FillWeight = weight,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter },
            });

        return dg;
    }

    private static readonly Color GreenText = Color.FromArgb(0, 130, 0);
    private static readonly Color RedText   = Color.FromArgb(190, 40, 40);

    private int[] ParseSizes()
    {
        return sizesTextBox.Text
            .Split(',')
            .Select(s => s.Trim())
            .Where(s => s.Length > 0)
            .Select(s => int.TryParse(s, out int n) ? n : 0)
            .Where(n => n > 1)
            .Distinct()
            .OrderBy(n => n)
            .ToArray();
    }

    private IGraph BuildGraphForSize(int n, int genIdx, double param2, bool exactCount)
    {
        switch (genIdx)
        {
            case 0:
                if (exactCount)
                    return new FisherYatesGraphGenerator(n, Math.Min((int)param2, n * (n - 1) / 2)).Generate();
                return new ErdosRenyiProbabilityGenerator(n, param2).Generate();
            case 1:
                int rows = Math.Max(1, (int)Math.Round(Math.Sqrt(n)));
                int cols = Math.Max(1, (int)Math.Ceiling((double)n / rows));
                return new RegularGridGraphGenerator(rows, cols).Generate();
            case 2:
                int left = n / 2, right = n - left;
                if (exactCount)
                    return new BipartiteGraphGenerator(left, right, Math.Min((int)param2, left * right)).Generate();
                return new BipartiteGraphGenerator(left, right, (int)Math.Round(param2 * left * right)).Generate();
            case 3: return new ChainGraphGenerator(n).Generate();
            case 4: return new CliqueGraphGenerator(n).Generate();
            case 5:
                if (exactCount)
                    return new FisherYatesGraphGenerator(n, Math.Min((int)param2, n * (n - 1) / 2)).Generate();
                return new FisherYatesGraphGenerator(n, (int)Math.Round(param2 * n * (n - 1) / 2)).Generate();
            default: throw new InvalidOperationException();
        }
    }

    private async void OnRun(object? sender, EventArgs e)
    {
        var sizes = ParseSizes();
        if (sizes.Length == 0)
        {
            MessageBox.Show("Введите корректные размеры (целые числа > 1, через запятую).", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        runButton.Enabled = false;
        grid.Rows.Clear();
        statusLabel.Text = "Выполняется...";

        const int runs = 3;
        progressBar.Maximum = sizes.Length * runs;
        progressBar.Value = 0;

        int genIdx = generatorCombo.SelectedIndex;
        double param2 = (double)param2UpDown.Value;
        bool exactCount = edgeCountRadio.Checked;

        var actualSizes = new int[sizes.Length];
        var klTimes  = new double[sizes.Length];
        var fmTimes  = new double[sizes.Length];
        var klIters  = new double[sizes.Length];
        var fmIters  = new double[sizes.Length];
        var klCuts     = new int[sizes.Length];
        var fmCuts     = new int[sizes.Length];
        var edgeCounts = new int[sizes.Length];

        await Task.Run(() =>
        {
            for (int si = 0; si < sizes.Length; si++)
            {
                int n = sizes[si];
                double klMs = 0, fmMs = 0;
                int klCut = 0, fmCut = 0, klMoves = 0, fmMoves = 0;
                int edges = 0, actualN = n;

                for (int r = 0; r < runs; r++)
                {
                    IGraph graph;
                    try { graph = BuildGraphForSize(n, genIdx, param2, exactCount); }
                    catch (Exception ex)
                    {
                        Invoke(() => MessageBox.Show(ex.Message, "Ошибка генератора", MessageBoxButtons.OK, MessageBoxIcon.Warning));
                        continue;
                    }

                    actualN = graph.VerticesCount;
                    edges   = graph.EdgesCount;

                    var sw = Stopwatch.StartNew();
                    var klGen  = new KernighanLinGraphPartitionGenerator(2);
                    var klPart = klGen.Generate(graph);
                    sw.Stop();
                    klMs    += sw.Elapsed.TotalMilliseconds;
                    klCut   += klPart.CrossEdgesCount;
                    klMoves += klGen.LastIterationsCount;

                    sw.Restart();
                    var fmGen  = new FiducciaMattheysesGraphPartitionGenerator();
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

                actualSizes[si] = actualN;
                klTimes[si]    = avgKlMs;
                fmTimes[si]    = avgFmMs;
                klIters[si]    = avgKlMoves;
                fmIters[si]    = avgFmMoves;
                klCuts[si]     = (int)Math.Round(avgKlCut);
                fmCuts[si]     = (int)Math.Round(avgFmCut);
                edgeCounts[si] = edges;

                Invoke(() =>
                {
                    int row = grid.Rows.Add(
                        actualN, edges,
                        avgKlCut.ToString("F1"), avgKlMs.ToString("F2"), avgKlMoves.ToString("F1"),
                        avgFmCut.ToString("F1"), avgFmMs.ToString("F2"), avgFmMoves.ToString("F1"),
                        (delta >= 0 ? "+" : "") + delta.ToString("F1") + "%"
                    );

                    var rowObj = grid.Rows[row];
                    if (avgFmCut < avgKlCut)      rowObj.Cells[5].Style.ForeColor = GreenText;
                    else if (avgKlCut < avgFmCut) rowObj.Cells[2].Style.ForeColor = GreenText;
                    if (avgFmMs < avgKlMs)        rowObj.Cells[6].Style.ForeColor = GreenText;
                    else if (avgKlMs < avgFmMs)   rowObj.Cells[3].Style.ForeColor = GreenText;
                    rowObj.Cells[8].Style.ForeColor = delta > 0 ? GreenText : delta < 0 ? RedText : Color.DimGray;
                });
            }

            Invoke(() =>
            {
                iterChart.SetData(actualSizes, klIters, fmIters, "Итерации (количество ходов)");
                timeChart.SetData(actualSizes, klTimes, fmTimes, "Время выполнения, мс");
                cutsChart.SetData(actualSizes, klCuts, fmCuts, edgeCounts);

                string genName = genIdx switch
                {
                    0 => $"Эрдёша–Реньи (p={param2:F2})",
                    1 => "Регулярная решётка",
                    2 => $"Двудольный граф (плотность {param2:F2})",
                    3 => "Цепочка",
                    4 => "Клика",
                    5 => $"Фишер–Йетс (плотность {param2:F2})",
                    _ => ""
                };
                statusLabel.Text = $"Готово. Генератор: {genName}. {runs} прогона на размер, средние значения.";
                runButton.Enabled = true;
            });
        });
    }
}
