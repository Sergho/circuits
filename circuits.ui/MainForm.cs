public class MainForm : Form
{
    private IGraph? currentGraph;
    private IGraphPartition? currentPartition;

    private ComboBox generatorTypeCombo = null!;
    private NumericUpDown param1UpDown = null!;
    private NumericUpDown param2UpDown = null!;
    private Label param1Label = null!;
    private Label param2Label = null!;
    private RadioButton klRadio = null!;
    private RadioButton fmRadio = null!;
    private Button runButton = null!;
    private Button compareButton = null!;
    private Button detailButton = null!;
    private GraphCanvas graphCanvas = null!;
    private ComparisonChart comparisonChart = null!;
    private TabControl tabControl = null!;
    private ToolStripStatusLabel cutSizeLabel = null!;
    private ToolStripStatusLabel timeLabel = null!;
    private ToolStripStatusLabel iterLabel = null!;
    private ToolStripStatusLabel partSizesLabel = null!;

    public MainForm()
    {
        Text = "Разбиение графа — Керниган–Лин / Фиддуча–Мэтьюс";
        Size = new Size(1100, 700);
        MinimumSize = new Size(900, 580);
        StartPosition = FormStartPosition.CenterScreen;
        BuildUI();
    }

    private void BuildUI()
    {
        var status = new StatusStrip { Dock = DockStyle.Bottom };
        cutSizeLabel  = new ToolStripStatusLabel("Межсоединений: —");
        timeLabel     = new ToolStripStatusLabel("Время: —");
        iterLabel     = new ToolStripStatusLabel("Итераций: —");
        partSizesLabel = new ToolStripStatusLabel("Части: —") { Spring = true, TextAlign = ContentAlignment.MiddleRight };
        status.Items.AddRange(new ToolStripItem[]
        {
            cutSizeLabel, new ToolStripSeparator(),
            timeLabel,    new ToolStripSeparator(),
            iterLabel,    new ToolStripSeparator(),
            partSizesLabel
        });
        Controls.Add(status);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Padding = new Padding(6),
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 262));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.Controls.Add(BuildLeftPanel(), 0, 0);

        tabControl = new TabControl { Dock = DockStyle.Fill };

        var tabGraph = new TabPage("Граф");
        graphCanvas = new GraphCanvas { Dock = DockStyle.Fill };
        tabGraph.Controls.Add(graphCanvas);

        var tabChart = new TabPage("Сравнение алгоритмов");
        comparisonChart = new ComparisonChart { Dock = DockStyle.Fill };
        tabChart.Controls.Add(comparisonChart);

        tabControl.TabPages.Add(tabGraph);
        tabControl.TabPages.Add(tabChart);
        layout.Controls.Add(tabControl, 1, 0);

        Controls.Add(layout);
    }

    private Panel BuildLeftPanel()
    {
        var panel = new Panel { Dock = DockStyle.Fill };
        var flow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Padding = new Padding(0, 0, 4, 0),
        };

        flow.Controls.Add(BuildGraphGroup());
        flow.Controls.Add(BuildAlgoGroup());

        runButton = new Button
        {
            Text = "Запустить",
            Width = 246, Height = 34,
            Margin = new Padding(0, 6, 0, 4),
        };
        runButton.Click += OnRun;
        flow.Controls.Add(runButton);

        compareButton = new Button
        {
            Text = "Сравнить алгоритмы",
            Width = 246, Height = 34,
        };
        compareButton.Click += OnCompare;
        flow.Controls.Add(compareButton);

        detailButton = new Button
        {
            Text = "Детальное сравнение (таблица)",
            Width = 246, Height = 34,
            Margin = new Padding(0, 4, 0, 0),
        };
        detailButton.Click += OnDetailedCompare;
        flow.Controls.Add(detailButton);

        panel.Controls.Add(flow);
        return panel;
    }

    private GroupBox BuildGraphGroup()
    {
        var g = new GroupBox { Text = "Параметры графа", Width = 246, Height = 190, Margin = new Padding(0, 0, 0, 4) };

        var genLabel = new Label { Text = "Тип генератора:", Left = 8, Top = 22, Width = 228, AutoSize = false };
        generatorTypeCombo = new ComboBox { Left = 8, Top = 40, Width = 228, DropDownStyle = ComboBoxStyle.DropDownList };
        generatorTypeCombo.Items.AddRange(new object[]
        {
            "Эрдёша–Реньи", "Регулярная решётка", "Двудольный граф", "Цепочка", "Клика"
        });
        generatorTypeCombo.SelectedIndex = 0;
        generatorTypeCombo.SelectedIndexChanged += OnGeneratorChanged;

        param1Label = new Label { Text = "Вершин:", Left = 8, Top = 72, Width = 120, AutoSize = false };
        param1UpDown = new NumericUpDown { Left = 8, Top = 90, Width = 110, Minimum = 2, Maximum = 1000, Value = 30 };

        param2Label = new Label { Text = "Вероятность ребра:", Left = 8, Top = 120, Width = 228, AutoSize = false };
        param2UpDown = new NumericUpDown
        {
            Left = 8, Top = 138, Width = 110,
            Minimum = 0, Maximum = 1, DecimalPlaces = 2, Increment = 0.05m, Value = 0.15m,
        };

        g.Controls.Add(genLabel);
        g.Controls.Add(generatorTypeCombo);
        g.Controls.Add(param1Label);
        g.Controls.Add(param1UpDown);
        g.Controls.Add(param2Label);
        g.Controls.Add(param2UpDown);
        return g;
    }

    private GroupBox BuildAlgoGroup()
    {
        var g = new GroupBox { Text = "Алгоритм", Width = 246, Height = 82, Margin = new Padding(0, 0, 0, 4) };

        klRadio = new RadioButton { Text = "Керниган–Лин (базовый)", Left = 8, Top = 20, Width = 228, Height = 22 };
        fmRadio = new RadioButton { Text = "Фиддуча–Мэтьюс (эвристический)", Left = 8, Top = 46, Width = 228, Height = 22, Checked = true };

        g.Controls.Add(klRadio);
        g.Controls.Add(fmRadio);
        return g;
    }

    private void OnGeneratorChanged(object? sender, EventArgs e)
    {
        switch (generatorTypeCombo.SelectedIndex)
        {
            case 0:
                param1Label.Text = "Вершин:";
                param1UpDown.DecimalPlaces = 0; param1UpDown.Increment = 1; param1UpDown.Minimum = 2; param1UpDown.Maximum = 1000; param1UpDown.Value = 30;
                param2Label.Text = "Вероятность ребра:";
                param2UpDown.DecimalPlaces = 2; param2UpDown.Increment = 0.05m; param2UpDown.Minimum = 0; param2UpDown.Maximum = 1; param2UpDown.Value = 0.15m;
                param2Label.Visible = true; param2UpDown.Visible = true;
                break;
            case 1:
                param1Label.Text = "Строк:";
                param1UpDown.DecimalPlaces = 0; param1UpDown.Increment = 1; param1UpDown.Minimum = 1; param1UpDown.Maximum = 200; param1UpDown.Value = 5;
                param2Label.Text = "Столбцов:";
                param2UpDown.DecimalPlaces = 0; param2UpDown.Increment = 1; param2UpDown.Minimum = 1; param2UpDown.Maximum = 200; param2UpDown.Value = 10;
                param2Label.Visible = true; param2UpDown.Visible = true;
                break;
            case 2:
                param1Label.Text = "Левая доля:";
                param1UpDown.DecimalPlaces = 0; param1UpDown.Increment = 1; param1UpDown.Minimum = 1; param1UpDown.Maximum = 500; param1UpDown.Value = 15;
                param2Label.Text = "Правая доля:";
                param2UpDown.DecimalPlaces = 0; param2UpDown.Increment = 1; param2UpDown.Minimum = 1; param2UpDown.Maximum = 500; param2UpDown.Value = 15;
                param2Label.Visible = true; param2UpDown.Visible = true;
                break;
            case 3:
            case 4:
                param1Label.Text = "Вершин:";
                param1UpDown.DecimalPlaces = 0; param1UpDown.Increment = 1; param1UpDown.Minimum = 2; param1UpDown.Maximum = 1000; param1UpDown.Value = 20;
                param2Label.Visible = false; param2UpDown.Visible = false;
                break;
        }
    }

    private IGraph BuildGraph()
    {
        return generatorTypeCombo.SelectedIndex switch
        {
            0 => new ErdosRenyiProbabilityGenerator((int)param1UpDown.Value, (double)param2UpDown.Value).Generate(),
            1 => new RegularGridGraphGenerator((int)param1UpDown.Value, (int)param2UpDown.Value).Generate(),
            2 => new BipartiteGraphGenerator((int)param1UpDown.Value, (int)param2UpDown.Value,
                     (int)param1UpDown.Value * (int)param2UpDown.Value / 2).Generate(),
            3 => new ChainGraphGenerator((int)param1UpDown.Value).Generate(),
            4 => new CliqueGraphGenerator((int)param1UpDown.Value).Generate(),
            _ => throw new InvalidOperationException()
        };
    }

    private void OnRun(object? sender, EventArgs e)
    {
        Cursor = Cursors.WaitCursor;
        runButton.Enabled = false;
        try
        {
            var graph = BuildGraph();
            IGraphPartition partition;
            int iters;

            var sw = Stopwatch.StartNew();
            if (klRadio.Checked)
            {
                var gen = new KernighanLinGraphPartitionGenerator(2);
                partition = gen.Generate(graph);
                iters = gen.LastIterationsCount;
            }
            else
            {
                var gen = new FiducciaMattheysesGraphPartitionGenerator();
                partition = gen.Generate(graph);
                iters = gen.LastIterationsCount;
            }
            sw.Stop();

            currentGraph = graph;
            currentPartition = partition;

            cutSizeLabel.Text  = $"Межсоединений: {partition.CrossEdgesCount}";
            timeLabel.Text     = $"Время: {sw.Elapsed.TotalMilliseconds:F2} мс";
            iterLabel.Text     = $"Итераций: {iters}";
            var parts = partition.Parts.ToArray();
            partSizesLabel.Text = $"Часть A: {parts[0].VerticesCount}   |   Часть B: {parts[1].VerticesCount}";

            graphCanvas.SetData(graph, partition);
            tabControl.SelectedTab = tabControl.TabPages[0];
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        finally
        {
            Cursor = Cursors.Default;
            runButton.Enabled = true;
        }
    }

    private void OnDetailedCompare(object? sender, EventArgs e)
    {
        new DetailedComparisonForm().ShowDialog(this);
    }

    private async void OnCompare(object? sender, EventArgs e)
    {
        Cursor = Cursors.WaitCursor;
        compareButton.Enabled = false;
        try
        {
            int[] testSizes = [10, 25, 50, 100, 175, 300, 500];
            double prob = generatorTypeCombo.SelectedIndex == 0 ? (double)param2UpDown.Value : 0.15;
            const int runs = 2;

            var klCuts    = new int[testSizes.Length];
            var fmCuts    = new int[testSizes.Length];
            var edgeCounts = new int[testSizes.Length];

            await Task.Run(() =>
            {
                for (int i = 0; i < testSizes.Length; i++)
                {
                    long klSum = 0, fmSum = 0, edgeSum = 0;
                    for (int r = 0; r < runs; r++)
                    {
                        var graph = new ErdosRenyiProbabilityGenerator(testSizes[i], prob).Generate();
                        edgeSum += graph.EdgesCount;
                        klSum   += new KernighanLinGraphPartitionGenerator(2).Generate(graph).CrossEdgesCount;
                        fmSum   += new FiducciaMattheysesGraphPartitionGenerator().Generate(graph).CrossEdgesCount;
                    }
                    klCuts[i]     = (int)(klSum    / runs);
                    fmCuts[i]     = (int)(fmSum    / runs);
                    edgeCounts[i] = (int)(edgeSum  / runs);
                }
            });

            comparisonChart.SetData(testSizes, klCuts, fmCuts, edgeCounts);
            tabControl.SelectedTab = tabControl.TabPages[1];
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        finally
        {
            Cursor = Cursors.Default;
            compareButton.Enabled = true;
        }
    }
}
