namespace DataAutomate
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private ComboBox cboProvider;
        private TextBox txtConn;
        private Button btnTest;
        private Button btnLoadCatalog;
        private TreeView tvSchema;
        private Button btnExtract;
        private TextBox txtOut;
        private Button btnBrowseOut;
        private CheckBox chkGzip;
        private CheckBox chkSanitize;
        private Label lblProvider;
        private Label lblConn;
        private Label lblOut;
        private TextBox txtAffixCols;
        private Label lblAffixCols;
        private NumericUpDown numMinAffix;
        private Label lblMinAffix;
        private NumericUpDown numMinFreq;
        private Label lblMinFreq;
        private TextBox txtLog;
        private Button btnCancel;
        private CheckBox chkParquet;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            cboProvider = new ComboBox();
            txtConn = new TextBox();
            btnTest = new Button();
            btnLoadCatalog = new Button();
            tvSchema = new TreeView();
            btnExtract = new Button();
            txtOut = new TextBox();
            btnBrowseOut = new Button();
            chkGzip = new CheckBox();
            chkSanitize = new CheckBox();
            lblProvider = new Label();
            lblConn = new Label();
            lblOut = new Label();
            txtAffixCols = new TextBox();
            lblAffixCols = new Label();
            numMinAffix = new NumericUpDown();
            lblMinAffix = new Label();
            numMinFreq = new NumericUpDown();
            lblMinFreq = new Label();
            txtLog = new TextBox();
            btnCancel = new Button();
            chkParquet = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)numMinAffix).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numMinFreq).BeginInit();
            SuspendLayout();
            // 
            // cboProvider
            // 
            cboProvider.DropDownStyle = ComboBoxStyle.DropDownList;
            cboProvider.Items.AddRange(new object[] { "mssql", "postgres", "mysql", "sqlite" });
            cboProvider.Location = new System.Drawing.Point(20, 35);
            cboProvider.Name = "cboProvider";
            cboProvider.Size = new System.Drawing.Size(120, 23);
            // 
            // txtConn
            // 
            txtConn.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtConn.Location = new System.Drawing.Point(150, 35);
            txtConn.Name = "txtConn";
            txtConn.Size = new System.Drawing.Size(750, 23);
            // 
            // btnTest
            // 
            btnTest.Location = new System.Drawing.Point(20, 70);
            btnTest.Name = "btnTest";
            btnTest.Size = new System.Drawing.Size(120, 27);
            btnTest.Text = "Probar conexión";
            btnTest.UseVisualStyleBackColor = true;
            btnTest.Click += btnTest_Click;
            // 
            // btnLoadCatalog
            // 
            btnLoadCatalog.Location = new System.Drawing.Point(150, 70);
            btnLoadCatalog.Name = "btnLoadCatalog";
            btnLoadCatalog.Size = new System.Drawing.Size(140, 27);
            btnLoadCatalog.Text = "Cargar catálogo";
            btnLoadCatalog.UseVisualStyleBackColor = true;
            btnLoadCatalog.Click += btnLoadCatalog_Click;
            // 
            // tvSchema
            // 
            tvSchema.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            tvSchema.CheckBoxes = true;
            tvSchema.Location = new System.Drawing.Point(20, 110);
            tvSchema.Name = "tvSchema";
            tvSchema.Size = new System.Drawing.Size(350, 450);
            // 
            // txtOut
            // 
            txtOut.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtOut.Location = new System.Drawing.Point(150, 105);
            txtOut.Visible = false;
            // 
            // btnBrowseOut
            // 
            btnBrowseOut.Location = new System.Drawing.Point(20, 570);
            btnBrowseOut.Name = "btnBrowseOut";
            btnBrowseOut.Size = new System.Drawing.Size(120, 27);
            btnBrowseOut.Text = "Carpeta salida…";
            btnBrowseOut.UseVisualStyleBackColor = true;
            btnBrowseOut.Click += btnBrowseOut_Click;
            // 
            // chkGzip
            // 
            chkGzip.AutoSize = true;
            chkGzip.Location = new System.Drawing.Point(390, 110);
            chkGzip.Text = "CSV .gz";
            // 
            // chkParquet
            // 
            chkParquet.AutoSize = true;
            chkParquet.Location = new System.Drawing.Point(470, 110);
            chkParquet.Text = "Parquet";
            // 
            // chkSanitize
            // 
            chkSanitize.AutoSize = true;
            chkSanitize.Checked = true;
            chkSanitize.Location = new System.Drawing.Point(560, 110);
            chkSanitize.Text = "Sanear strings";
            // 
            // lblProvider
            // 
            lblProvider.AutoSize = true;
            lblProvider.Location = new System.Drawing.Point(20, 15);
            lblProvider.Text = "Provider";
            // 
            // lblConn
            // 
            lblConn.AutoSize = true;
            lblConn.Location = new System.Drawing.Point(150, 15);
            lblConn.Text = "Connection string";
            // 
            // lblOut
            // 
            lblOut.AutoSize = true;
            lblOut.Location = new System.Drawing.Point(150, 575);
            lblOut.Text = "Salida:";
            // 
            // txtOut
            // 
            txtOut = new TextBox();
            txtOut.Location = new System.Drawing.Point(200, 572);
            txtOut.Size = new System.Drawing.Size(520, 23);
            // 
            // Affix controls
            // 
            lblAffixCols = new Label() { Location = new System.Drawing.Point(390, 145), Text = "Affix cols (coma):" };
            txtAffixCols = new TextBox() { Location = new System.Drawing.Point(510, 142), Width = 390, PlaceholderText = "ej: first_name,last_name" };
            lblMinAffix = new Label() { Location = new System.Drawing.Point(390, 175), Text = "Min affix" };
            numMinAffix = new NumericUpDown() { Location = new System.Drawing.Point(450, 172), Minimum = 2, Maximum = 16, Value = 3 };
            lblMinFreq = new Label() { Location = new System.Drawing.Point(520, 175), Text = "Min freq" };
            numMinFreq = new NumericUpDown() { Location = new System.Drawing.Point(585, 172), Minimum = 2, Maximum = 1000, Value = 5 };
            // 
            // btnExtract
            // 
            btnExtract.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnExtract.Location = new System.Drawing.Point(730, 570);
            btnExtract.Size = new System.Drawing.Size(100, 27);
            btnExtract.Text = "Extraer";
            btnExtract.UseVisualStyleBackColor = true;
            btnExtract.Click += btnExtract_Click;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnCancel.Location = new System.Drawing.Point(840, 570);
            btnCancel.Size = new System.Drawing.Size(60, 27);
            btnCancel.Text = "Stop";
            btnCancel.Click += btnCancel_Click;
            // 
            // txtLog
            // 
            txtLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtLog.Location = new System.Drawing.Point(390, 205);
            txtLog.Multiline = true;
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.Size = new System.Drawing.Size(510, 355);

            // 
            // MainForm
            // 
            ClientSize = new System.Drawing.Size(920, 610);
            Controls.Add(cboProvider); Controls.Add(txtConn); Controls.Add(btnTest); Controls.Add(btnLoadCatalog);
            Controls.Add(tvSchema); Controls.Add(btnBrowseOut); Controls.Add(lblOut); Controls.Add(txtOut);
            Controls.Add(chkGzip); Controls.Add(chkParquet); Controls.Add(chkSanitize);
            Controls.Add(lblProvider); Controls.Add(lblConn);
            Controls.Add(lblAffixCols); Controls.Add(txtAffixCols); Controls.Add(lblMinAffix); Controls.Add(numMinAffix);
            Controls.Add(lblMinFreq); Controls.Add(numMinFreq);
            Controls.Add(btnExtract); Controls.Add(btnCancel); Controls.Add(txtLog);
            Text = "AutoData – Windows Forms";
            ((System.ComponentModel.ISupportInitialize)numMinAffix).EndInit();
            ((System.ComponentModel.ISupportInitialize)numMinFreq).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
