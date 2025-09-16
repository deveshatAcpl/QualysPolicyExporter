using System.Drawing;
using System.Windows.Forms;

namespace QualysPolicyExporter.Forms
{
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null;
        
        // Main layout container
        private TableLayoutPanel mainTableLayout;
        
        // Section containers
        private GroupBox grpCredentials;
        private GroupBox grpExportSettings;
        private GroupBox grpAdvancedSettings;
        
        // Credentials section
        private Label lblUsername;
        private TextBox txtUsername;
        private Label lblPassword;
        private TextBox txtPassword;
        
        // Export settings section
        private Label lblExportPath;
        private TextBox txtExportPath;
        private Button btnBrowse;
        private Label lblInterval;
        private NumericUpDown numIntervalDays;
        private Label lblTime;
        private DateTimePicker timePicker;
        private Label lblDaysBeforeToday;
        private NumericUpDown numDaysBeforeToday;

        // Advanced settings section
        private CheckBox chkEnableProxy;
        private Label lblProxyUrl;
        private TextBox txtProxyUrl;
        private Label lblTechnologyIds;
        private TextBox txtTechnologyIds;
        private RadioButton rbIncludeTechIds;
        private RadioButton rbExcludeTechIds;
        private Label lblPolicyIds;
        private TextBox txtPolicyIds;
        
        // Action buttons
        private Panel pnlButtons;
        private Button btnSave;
        private Button btnCancel;
        
        // Modern color palette
        private static readonly Color PrimaryColor = Color.FromArgb(0, 120, 215);
        private static readonly Color SecondaryColor = Color.FromArgb(243, 243, 243);
        private static readonly Color AccentColor = Color.FromArgb(0, 99, 177);
        private static readonly Color BackgroundColor = Color.FromArgb(250, 250, 250);
        private static readonly Color TextColor = Color.FromArgb(32, 31, 30);
        private static readonly Color BorderColor = Color.FromArgb(200, 198, 196);

        private void InitializeComponent()
        {
            // Initialize controls
            InitializeControls();
            
            // Setup form properties
            SetupFormProperties();
            
            // Create layout structure
            CreateLayoutStructure();
            
            // Apply modern styling
            ApplyModernStyling();
            
            // Wire up events
            WireUpEvents();
            
            SuspendLayout();
            ResumeLayout(false);
            PerformLayout();
        }
        
        private void InitializeControls()
        {
            // Initialize layout containers
            mainTableLayout = new TableLayoutPanel();
            grpCredentials = new GroupBox();
            grpExportSettings = new GroupBox();
            grpAdvancedSettings = new GroupBox();
            pnlButtons = new Panel();
            
            // Credentials section
            lblUsername = new Label();
            txtUsername = new TextBox();
            lblPassword = new Label();
            txtPassword = new TextBox();
            
            // Export settings section
            lblExportPath = new Label();
            txtExportPath = new TextBox();
            btnBrowse = new Button();
            lblInterval = new Label();
            numIntervalDays = new NumericUpDown();
            lblTime = new Label();
            timePicker = new DateTimePicker();
            lblDaysBeforeToday = new Label();
            numDaysBeforeToday = new NumericUpDown();
            
            // Advanced settings section
            chkEnableProxy = new CheckBox();
            lblProxyUrl = new Label();
            txtProxyUrl = new TextBox();
            lblTechnologyIds = new Label();
            txtTechnologyIds = new TextBox();
            rbIncludeTechIds = new RadioButton();
            rbExcludeTechIds = new RadioButton();
            lblPolicyIds = new Label();
            txtPolicyIds = new TextBox();
            
            // Action buttons
            btnSave = new Button();
            btnCancel = new Button();
        }
        
        private void SetupFormProperties()
        {
            Text = "Qualys Policy Exporter - Settings";
            ClientSize = new Size(520, 680);
            BackColor = BackgroundColor;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            ShowIcon = false;
        }
        
        private void CreateLayoutStructure()
        {
            // Configure main table layout
            mainTableLayout.ColumnCount = 1;
            mainTableLayout.RowCount = 4;
            mainTableLayout.Dock = DockStyle.Fill;
            mainTableLayout.Padding = new Padding(20);
            mainTableLayout.BackColor = BackgroundColor;
            
            // Set row styles for proper spacing
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            
            // Configure group boxes
            SetupCredentialsGroup();
            SetupExportSettingsGroup();
            SetupAdvancedSettingsGroup();
            SetupButtonPanel();
            
            // Add sections to main layout
            mainTableLayout.Controls.Add(grpCredentials, 0, 0);
            mainTableLayout.Controls.Add(grpExportSettings, 0, 1);
            mainTableLayout.Controls.Add(grpAdvancedSettings, 0, 2);
            mainTableLayout.Controls.Add(pnlButtons, 0, 3);
            
            Controls.Add(mainTableLayout);
        }
        
        private void SetupCredentialsGroup()
        {
            grpCredentials.Text = "🔐 Credentials";
            grpCredentials.Size = new Size(460, 100);
            grpCredentials.Padding = new Padding(15);
            
            var layout = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 2,
                Dock = DockStyle.Fill,
                AutoSize = true
            };
            
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            
            // Username
            lblUsername.Text = "Username:";
            lblUsername.Anchor = AnchorStyles.Left;
            txtUsername.PlaceholderText = "Enter your username";
            txtUsername.Size = new Size(300, 23);
            txtUsername.TabIndex = 0;
            
            // Password
            lblPassword.Text = "Password:";
            lblPassword.Anchor = AnchorStyles.Left;
            txtPassword.PlaceholderText = "Enter your password";
            txtPassword.Size = new Size(300, 23);
            txtPassword.UseSystemPasswordChar = true;
            txtPassword.TabIndex = 1;
            
            layout.Controls.Add(lblUsername, 0, 0);
            layout.Controls.Add(txtUsername, 1, 0);
            layout.Controls.Add(lblPassword, 0, 1);
            layout.Controls.Add(txtPassword, 1, 1);
            
            grpCredentials.Controls.Add(layout);
        }
        
        private void SetupExportSettingsGroup()
        {
            grpExportSettings.Text = "⚙️ Export Settings";
            grpExportSettings.Size = new Size(460, 200);
            grpExportSettings.Padding = new Padding(15);
            
            var layout = new TableLayoutPanel
            {
                ColumnCount = 3,
                RowCount = 5,
                Dock = DockStyle.Fill,
                AutoSize = true
            };
            
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            
            // Export Path
            lblExportPath.Text = "Export Path:";
            lblExportPath.Anchor = AnchorStyles.Left;
            txtExportPath.PlaceholderText = "Select export directory";
            txtExportPath.Size = new Size(250, 23);
            txtExportPath.TabIndex = 2;
            btnBrowse.Text = "Browse...";
            btnBrowse.Size = new Size(80, 29);
            btnBrowse.TabIndex = 3;
            
            // Interval
            lblInterval.Text = "Interval (Days):";
            lblInterval.Anchor = AnchorStyles.Left;
            numIntervalDays.Minimum = 0;
            numIntervalDays.Maximum = 365;
            numIntervalDays.Value = 0;
            numIntervalDays.Size = new Size(80, 23);
            numIntervalDays.TabIndex = 4;
            
            // Time
            lblTime.Text = "Export Time:";
            lblTime.Anchor = AnchorStyles.Left;
            timePicker.Format = DateTimePickerFormat.Time;
            timePicker.ShowUpDown = true;
            timePicker.Size = new Size(100, 23);
            timePicker.TabIndex = 5;
            
            // Days Before Today
            lblDaysBeforeToday.Text = "Days Before:";
            lblDaysBeforeToday.Anchor = AnchorStyles.Left;
            numDaysBeforeToday.Minimum = 1;
            numDaysBeforeToday.Maximum = 365;
            numDaysBeforeToday.Value = 1;
            numDaysBeforeToday.Size = new Size(80, 23);
            numDaysBeforeToday.TabIndex = 6;
            
            layout.Controls.Add(lblExportPath, 0, 0);
            layout.Controls.Add(txtExportPath, 1, 0);
            layout.Controls.Add(btnBrowse, 2, 0);
            layout.Controls.Add(lblInterval, 0, 1);
            layout.Controls.Add(numIntervalDays, 1, 1);
            layout.Controls.Add(lblTime, 0, 2);
            layout.Controls.Add(timePicker, 1, 2);
            layout.Controls.Add(lblDaysBeforeToday, 0, 3);
            layout.Controls.Add(numDaysBeforeToday, 1, 3);
            
            grpExportSettings.Controls.Add(layout);
        }
        
        private void SetupAdvancedSettingsGroup()
        {
            grpAdvancedSettings.Text = "🔧 Advanced Settings";
            grpAdvancedSettings.Size = new Size(460, 220);
            grpAdvancedSettings.Padding = new Padding(15);
            
            var layout = new TableLayoutPanel
            {
                ColumnCount = 3,
                RowCount = 6,
                Dock = DockStyle.Fill,
                AutoSize = true
            };
            
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            
            // Proxy settings
            chkEnableProxy.Text = "Use Network Proxy";
            chkEnableProxy.TabIndex = 7;
            lblProxyUrl.Text = "Proxy URL:";
            lblProxyUrl.Anchor = AnchorStyles.Left;
            txtProxyUrl.PlaceholderText = "http://proxy:port";
            txtProxyUrl.Size = new Size(250, 23);
            txtProxyUrl.TabIndex = 8;
            
            // Technology IDs
            lblTechnologyIds.Text = "Technology IDs:";
            lblTechnologyIds.Anchor = AnchorStyles.Left;
            txtTechnologyIds.PlaceholderText = "Comma separated IDs";
            txtTechnologyIds.Size = new Size(150, 23);
            txtTechnologyIds.TabIndex = 9;
            
            var techPanel = new Panel { Size = new Size(100, 50), AutoSize = true };
            rbIncludeTechIds.Text = "Include";
            rbIncludeTechIds.Location = new Point(0, 0);
            rbIncludeTechIds.AutoSize = true;
            rbExcludeTechIds.Text = "Exclude";
            rbExcludeTechIds.Location = new Point(0, 20);
            rbExcludeTechIds.AutoSize = true;
            rbExcludeTechIds.Checked = true;
            techPanel.Controls.AddRange(new Control[] { rbIncludeTechIds, rbExcludeTechIds });
            
            // Policy IDs
            lblPolicyIds.Text = "Policy IDs:";
            lblPolicyIds.Anchor = AnchorStyles.Left;
            txtPolicyIds.PlaceholderText = "1234,5678,91011";
            txtPolicyIds.Size = new Size(250, 23);
            txtPolicyIds.TabIndex = 10;
            
            layout.Controls.Add(chkEnableProxy, 0, 0);
            layout.SetColumnSpan(chkEnableProxy, 3);
            layout.Controls.Add(lblProxyUrl, 0, 1);
            layout.Controls.Add(txtProxyUrl, 1, 1);
            layout.Controls.Add(lblTechnologyIds, 0, 2);
            layout.Controls.Add(txtTechnologyIds, 1, 2);
            layout.Controls.Add(techPanel, 2, 2);
            layout.Controls.Add(lblPolicyIds, 0, 3);
            layout.Controls.Add(txtPolicyIds, 1, 3);
            
            grpAdvancedSettings.Controls.Add(layout);
        }
        
        private void SetupButtonPanel()
        {
            pnlButtons.Size = new Size(460, 60);
            pnlButtons.Padding = new Padding(0, 20, 0, 0);
            
            // Cancel Button
            btnCancel.Text = "Cancel";
            btnCancel.Size = new Size(100, 35);
            btnCancel.Location = new Point(250, 20);
            btnCancel.TabIndex = 12;
            btnCancel.DialogResult = DialogResult.Cancel;
            
            // Save Button
            btnSave.Text = "Save Settings";
            btnSave.Size = new Size(120, 35);
            btnSave.Location = new Point(360, 20);
            btnSave.TabIndex = 11;
            btnSave.DialogResult = DialogResult.OK;
            
            pnlButtons.Controls.AddRange(new Control[] { btnCancel, btnSave });
        }
        
        private void ApplyModernStyling()
        {
            // Apply styling to group boxes
            foreach (Control control in Controls)
            {
                if (control is GroupBox groupBox)
                {
                    ApplyGroupBoxStyling(groupBox);
                }
            }
            
            // Apply styling to all text boxes
            ApplyTextBoxStyling(txtUsername);
            ApplyTextBoxStyling(txtPassword);
            ApplyTextBoxStyling(txtExportPath);
            ApplyTextBoxStyling(txtProxyUrl);
            ApplyTextBoxStyling(txtTechnologyIds);
            ApplyTextBoxStyling(txtPolicyIds);
            
            // Apply styling to all buttons
            ApplyButtonStyling(btnBrowse, false);
            ApplyButtonStyling(btnCancel, false);
            ApplyButtonStyling(btnSave, true);
            
            // Apply styling to labels
            ApplyLabelStyling(lblUsername);
            ApplyLabelStyling(lblPassword);
            ApplyLabelStyling(lblExportPath);
            ApplyLabelStyling(lblInterval);
            ApplyLabelStyling(lblTime);
            ApplyLabelStyling(lblDaysBeforeToday);
            ApplyLabelStyling(lblProxyUrl);
            ApplyLabelStyling(lblTechnologyIds);
            ApplyLabelStyling(lblPolicyIds);
            
            // Apply styling to checkboxes and radio buttons
            ApplyCheckBoxStyling(chkEnableProxy);
            ApplyRadioButtonStyling(rbIncludeTechIds);
            ApplyRadioButtonStyling(rbExcludeTechIds);
        }
        
        private void ApplyGroupBoxStyling(GroupBox groupBox)
        {
            groupBox.ForeColor = TextColor;
            groupBox.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            groupBox.FlatStyle = FlatStyle.Flat;
            groupBox.BackColor = Color.White;
            groupBox.Margin = new Padding(0, 0, 0, 15);
        }
        
        private void ApplyTextBoxStyling(TextBox textBox)
        {
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.BackColor = Color.White;
            textBox.ForeColor = TextColor;
            textBox.Font = new Font("Segoe UI", 9F);
            textBox.Margin = new Padding(3, 8, 3, 8);
        }
        
        private void ApplyButtonStyling(Button button, bool isPrimary)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            button.Cursor = Cursors.Hand;
            
            if (isPrimary)
            {
                button.BackColor = PrimaryColor;
                button.ForeColor = Color.White;
                button.FlatAppearance.BorderColor = AccentColor;
                button.FlatAppearance.MouseOverBackColor = AccentColor;
            }
            else
            {
                button.BackColor = SecondaryColor;
                button.ForeColor = TextColor;
                button.FlatAppearance.BorderColor = BorderColor;
                button.FlatAppearance.MouseOverBackColor = Color.FromArgb(230, 230, 230);
            }
            
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(200, 200, 200);
        }
        
        private void ApplyLabelStyling(Label label)
        {
            label.ForeColor = TextColor;
            label.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            label.AutoSize = true;
        }
        
        private void ApplyCheckBoxStyling(CheckBox checkBox)
        {
            checkBox.ForeColor = TextColor;
            checkBox.Font = new Font("Segoe UI", 9F);
            checkBox.FlatStyle = FlatStyle.System;
        }
        
        private void ApplyRadioButtonStyling(RadioButton radioButton)
        {
            radioButton.ForeColor = TextColor;
            radioButton.Font = new Font("Segoe UI", 9F);
            radioButton.FlatStyle = FlatStyle.System;
        }
        
        private void WireUpEvents()
        {
            btnBrowse.Click += btnBrowse_Click;
            btnSave.Click += btnSave_Click;
        }
    }

}


