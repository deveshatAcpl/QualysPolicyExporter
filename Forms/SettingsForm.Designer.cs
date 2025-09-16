namespace QualysPolicyExporter.Forms
{
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtExportPath;
        private Button btnBrowse;
        private Button btnSave;
        private NumericUpDown numIntervalDays;
        private DateTimePicker timePicker;
        private Label lblInterval;
        private Label lblTime;

        // Proxy Controls
        private CheckBox chkEnableProxy;
        private TextBox txtProxyUrl;
        private Label lblProxyUrl;

        // Custom Policy ID Controls
        private Label lblPolicyIds;
        private TextBox txtPolicyIds;

        // Technology ID Controls
        private Label lblTechnologyIds;
        private TextBox txtTechnologyIds;
        private RadioButton rbIncludeTechIds;
        private RadioButton rbExcludeTechIds;

        // Days Before Today Controls
        private Label lblDaysBeforeToday;
        private NumericUpDown numDaysBeforeToday;

        private void InitializeComponent()
        {
            txtUsername = new TextBox();
            txtPassword = new TextBox();
            txtExportPath = new TextBox();
            btnBrowse = new Button();
            btnSave = new Button();
            numIntervalDays = new NumericUpDown();
            timePicker = new DateTimePicker();
            lblInterval = new Label();
            lblTime = new Label();
            chkEnableProxy = new CheckBox();
            txtProxyUrl = new TextBox();
            lblProxyUrl = new Label();
            lblPolicyIds = new Label();
            txtPolicyIds = new TextBox();
            lblTechnologyIds = new Label();
            txtTechnologyIds = new TextBox();
            rbIncludeTechIds = new RadioButton();
            rbExcludeTechIds = new RadioButton();
            lblDaysBeforeToday = new Label();
            numDaysBeforeToday = new NumericUpDown();

            SuspendLayout();

            // Username
            txtUsername.Location = new Point(12, 12);
            txtUsername.Name = "txtUsername";
            txtUsername.PlaceholderText = "Username";
            txtUsername.Size = new Size(260, 31);
            txtUsername.TabIndex = 0;

            // Password
            txtPassword.Location = new Point(12, 49);
            txtPassword.Name = "txtPassword";
            txtPassword.PlaceholderText = "Password";
            txtPassword.Size = new Size(260, 31);
            txtPassword.TabIndex = 1;
            txtPassword.UseSystemPasswordChar = true;

            // Export Path
            txtExportPath.Location = new Point(12, 86);
            txtExportPath.Name = "txtExportPath";
            txtExportPath.PlaceholderText = "Export Path";
            txtExportPath.Size = new Size(179, 31);
            txtExportPath.TabIndex = 2;

            // Browse Button
            btnBrowse.Location = new Point(197, 86);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(75, 31);
            btnBrowse.TabIndex = 3;
            btnBrowse.Text = "Browse...";
            btnBrowse.Click += btnBrowse_Click;

            // Interval Label
            lblInterval.Location = new Point(12, 130);
            lblInterval.Name = "lblInterval";
            lblInterval.Size = new Size(260, 23);
            lblInterval.Text = "Export Interval (Days) [0 = Daily]";

            // Interval Numeric
            numIntervalDays.Location = new Point(12, 156);
            numIntervalDays.Minimum = 0;
            numIntervalDays.Maximum = 365;
            numIntervalDays.Value = 0;
            numIntervalDays.Size = new Size(134, 31);
            numIntervalDays.TabIndex = 4;

            // Time Label
            lblTime.Location = new Point(12, 194);
            lblTime.Name = "lblTime";
            lblTime.Size = new Size(120, 23);
            lblTime.Text = "Export Time:";

            // Time Picker
            timePicker.Format = DateTimePickerFormat.Time;
            timePicker.ShowUpDown = true;
            timePicker.Location = new Point(138, 192);
            timePicker.Name = "timePicker";
            timePicker.Size = new Size(134, 31);
            timePicker.TabIndex = 5;

            // Days Before Today Label
            lblDaysBeforeToday.Location = new Point(12, 235);
            lblDaysBeforeToday.Name = "lblDaysBeforeToday";
            lblDaysBeforeToday.Size = new Size(260, 23);
            lblDaysBeforeToday.Text = "Days before today:";

            // Days Before Today Numeric
            numDaysBeforeToday.Location = new Point(12, 260);
            numDaysBeforeToday.Minimum = 1;
            numDaysBeforeToday.Maximum = 365;
            numDaysBeforeToday.Value = 1;
            numDaysBeforeToday.Size = new Size(134, 31);
            numDaysBeforeToday.TabIndex = 6;

            // Enable Proxy Checkbox
            chkEnableProxy.Location = new Point(12, 300);
            chkEnableProxy.Name = "chkEnableProxy";
            chkEnableProxy.Size = new Size(260, 24);
            chkEnableProxy.TabIndex = 7;
            chkEnableProxy.Text = "Use Network Proxy";

            // Proxy URL Label
            lblProxyUrl.Location = new Point(12, 330);
            lblProxyUrl.Name = "lblProxyUrl";
            lblProxyUrl.Size = new Size(80, 23);
            lblProxyUrl.Text = "Proxy URL:";

            // Proxy URL Textbox
            txtProxyUrl.Location = new Point(12, 355);
            txtProxyUrl.Name = "txtProxyUrl";
            txtProxyUrl.PlaceholderText = "http://proxy:port";
            txtProxyUrl.Size = new Size(260, 31);
            txtProxyUrl.TabIndex = 8;

            // Technology IDs Label
            lblTechnologyIds.Text = "Technology IDs (comma separated):";
            lblTechnologyIds.Location = new Point(12, 395);
            lblTechnologyIds.AutoSize = true;

            //// Technology IDs TextBox
            txtTechnologyIds.Location = new Point(12, 420);
            txtTechnologyIds.Size = new Size(200, 31);
            txtTechnologyIds.TabIndex = 9;

            // Radio button - Include
            rbIncludeTechIds.Text = "Include";
            rbIncludeTechIds.Location = new Point(220, 422);
            rbIncludeTechIds.AutoSize = true;

            //// Radio button - Exclude
            rbExcludeTechIds.Text = "Exclude";
            rbExcludeTechIds.Location = new Point(290, 422);
            rbExcludeTechIds.AutoSize = true;
            rbExcludeTechIds.Checked = true; // Default

            // Policy IDs Label
            lblPolicyIds.Location = new Point(12, 460);
            lblPolicyIds.Name = "lblPolicyIds";
            lblPolicyIds.Size = new Size(150, 23);
            lblPolicyIds.Text = "Policy IDs (CSV):";

            // Policy IDs TextBox
            txtPolicyIds.Location = new Point(12, 485);
            txtPolicyIds.Name = "txtPolicyIds";
            txtPolicyIds.PlaceholderText = "1234,5678,91011";
            txtPolicyIds.Size = new Size(260, 31);
            txtPolicyIds.TabIndex = 10;

            // Save Button
            btnSave.Location = new Point(197, 530);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(75, 31);
            btnSave.TabIndex = 11;
            btnSave.Text = "Save";
            btnSave.Click += btnSave_Click;

            // SettingsForm
            ClientSize = new Size(380, 580);
            Controls.Add(txtUsername);
            Controls.Add(txtPassword);
            Controls.Add(txtExportPath);
            Controls.Add(btnBrowse);
            Controls.Add(lblInterval);
            Controls.Add(numIntervalDays);
            Controls.Add(lblTime);
            Controls.Add(timePicker);
            Controls.Add(lblDaysBeforeToday);
            Controls.Add(numDaysBeforeToday);
            Controls.Add(chkEnableProxy);
            Controls.Add(lblProxyUrl);
            Controls.Add(txtProxyUrl);
            Controls.Add(lblTechnologyIds);
            Controls.Add(txtTechnologyIds);
            Controls.Add(rbIncludeTechIds);
            Controls.Add(rbExcludeTechIds);
            Controls.Add(lblPolicyIds);
            Controls.Add(txtPolicyIds);
            Controls.Add(btnSave);
            Name = "SettingsForm";
            Text = "Exporter (V 1.0.1)";
            ResumeLayout(false);
            PerformLayout();
        }
    }

}


