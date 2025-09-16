using System;
using System.Windows.Forms;

namespace QualysPolicyExporter.Forms
{
    public partial class SettingsForm : Form
    {
        private readonly MainForm _mainForm;

        public SettingsForm(MainForm mainForm)
        {
            _mainForm = mainForm;
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            txtUsername.Text = Properties.Settings.Default.Username;
            txtPassword.Text = Properties.Settings.Default.Password;
            txtExportPath.Text = Properties.Settings.Default.ExportPath;
            //chkFilter.Checked = Properties.Settings.Default.FilterPassedToFailedOnly;
            numIntervalDays.Value = Properties.Settings.Default.ExportIntervalDays >= 0 ? Properties.Settings.Default.ExportIntervalDays : 1;
            timePicker.Value = DateTime.Today.Add(Properties.Settings.Default.ExportTime);

            chkEnableProxy.Checked = Properties.Settings.Default.EnableProxy;
            txtProxyUrl.Text = Properties.Settings.Default.ProxyUrl;

            // Load custom policy IDs
            txtPolicyIds.Text = Properties.Settings.Default.SelectedPolicyIds;


            txtTechnologyIds.Text = Properties.Settings.Default.TechnologyIds;
            rbIncludeTechIds.Checked = Properties.Settings.Default.TechIdMode == "Include";
            rbExcludeTechIds.Checked = Properties.Settings.Default.TechIdMode == "Exclude";

            // Load DaysBeforeToday
            numDaysBeforeToday.Value = Properties.Settings.Default.DaysBeforeToday > 0
                ? Properties.Settings.Default.DaysBeforeToday
                : 1;

        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using var folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                txtExportPath.Text = folderDialog.SelectedPath;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Username = txtUsername.Text;
            Properties.Settings.Default.Password = txtPassword.Text;
            Properties.Settings.Default.ExportPath = txtExportPath.Text;
            //Properties.Settings.Default.FilterPassedToFailedOnly = chkFilter.Checked;
            Properties.Settings.Default.ExportIntervalDays = (int)numIntervalDays.Value;
            Properties.Settings.Default.ExportTime = timePicker.Value.TimeOfDay;
            Properties.Settings.Default.EnableProxy = chkEnableProxy.Checked;
            Properties.Settings.Default.ProxyUrl = txtProxyUrl.Text;

            // Save custom policy IDs
            Properties.Settings.Default.SelectedPolicyIds = txtPolicyIds.Text.Trim();

            Properties.Settings.Default.TechnologyIds = txtTechnologyIds.Text.Trim();
            Properties.Settings.Default.TechIdMode = rbIncludeTechIds.Checked ? "Include" : "Exclude";
            Properties.Settings.Default.DaysBeforeToday = (int)numDaysBeforeToday.Value;


            Properties.Settings.Default.Save();


            _mainForm.RestartScheduler();

            MessageBox.Show("Settings saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }
    }
}
