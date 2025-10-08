using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataAutomate.Pro;



namespace DataAutomate
{
    public partial class MainForm : Form
    {
        private DataRunner? _runner;
        private CancellationTokenSource? _cts;

        public MainForm()
        {
            InitializeComponent();
            //WireEvents();
            SetDefaults();
        }

        void WireEvents()
        {
            btnExtract.Click     += btnExtract_Click;
            btnCancel.Click      += btnCancel_Click;
            btnTest.Click        += btnTest_Click;
            btnLoadCatalog.Click += btnLoadCatalog_Click;
            btnBrowseOut.Click   += btnBrowseOut_Click;
        }

        void SetDefaults()
        {
            if (cboProvider.Items.Count == 0)
                cboProvider.Items.AddRange(new object[] { "mssql", "postgres", "mysql", "sqlite" });
            if (cboProvider.SelectedIndex < 0) cboProvider.SelectedIndex = 0;

            if (string.IsNullOrWhiteSpace(txtOut.Text))
                txtOut.Text = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "warehouse");
        }

        void Log(string msg)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new Action(() => Log(msg)));
                return;
            }
            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {msg}{Environment.NewLine}");
        }

        void ToggleUi(bool enabled)
        {
            foreach (Control c in Controls) c.Enabled = enabled;
            btnCancel.Enabled = !enabled;
        }

        IEnumerable<TableIdent> SelectedTables()
        {
            foreach (TreeNode schemaNode in tvSchema.Nodes)
            {
                foreach (TreeNode tableNode in schemaNode.Nodes)
                {
                    if (tableNode.Checked && tableNode.Tag is TableIdent ti)
                        yield return ti;
                }
            }
        }

        async Task RunProAsync()
        {
            _runner = new DataRunner();
            _runner.Log += Log;

            var cfg = new ExtractSettings
            {
                Provider = cboProvider.Text,
                ConnectionString = txtConn.Text,
                OutputRoot = txtOut.Text,
                WriteCsv = true,
                CsvGzip = chkGzip.Checked,
                WriteParquet = chkParquet.Checked,
                SanitizeStrings = chkSanitize.Checked,
                AffixColumns = txtAffixCols.Text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
                AffixMinLen = (int)numMinAffix.Value,
                AffixMinFreq = (int)numMinFreq.Value,
                PageSize = 250_000,
                SampleRows = 5_000,
                PartitionByColumn = null,
                ValidateCounts = true,
                ComputeChecksums = true,
                Tables = SelectedTables().ToList()
            };

            Directory.CreateDirectory(cfg.OutputRoot);
            _cts = new CancellationTokenSource();
            ToggleUi(false);
            try
            {
                await _runner.RunAsync(cfg, _cts.Token);
                MessageBox.Show("Run OK", "AutoData Pro", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (OperationCanceledException) { Log("Cancelado."); }
            catch (Exception ex) { Log("Error: " + ex.Message); MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally { ToggleUi(true); _cts = null; }
        }

        async void btnTest_Click(object? s, EventArgs e)
        {
            try
            {
                using var db = DbUtils.Create(cboProvider.Text, txtConn.Text);
                await db.OpenAsync();
                Log("¡Conexión OK!");
            }
            catch (Exception ex)
            {
                Log("Error conexión: " + ex.Message);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        async void btnLoadCatalog_Click(object? s, EventArgs e)
        {
            tvSchema.Nodes.Clear();
            try
            {
                using var db = DbUtils.Create(cboProvider.Text, txtConn.Text);
                await db.OpenAsync();

                var tables = await DbUtils.ListTablesAsync(db);
                var bySchema = tables.GroupBy(t => t.Schema).OrderBy(g => g.Key);

                foreach (var g in bySchema)
                {
                    var sn = new TreeNode(g.Key) { Tag = g.Key };
                    foreach (var t in g.OrderBy(x => x.Name))
                        sn.Nodes.Add(new TreeNode(t.Name) { Tag = t });
                    tvSchema.Nodes.Add(sn);
                }
                tvSchema.ExpandAll();
                Log($"Catálogo cargado: {tables.Count} tablas.");
            }
            catch (Exception ex)
            {
                Log("Error catálogo: " + ex.Message);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        async void btnExtract_Click(object? sender, EventArgs e) => await RunProAsync();

        void btnBrowseOut_Click(object? s, EventArgs e)
        {
            using var dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() == DialogResult.OK) txtOut.Text = dlg.SelectedPath;
        }

        void btnCancel_Click(object? s, EventArgs e) => _cts?.Cancel();
    }
}
