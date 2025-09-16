using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QualysPolicyExporter.Services;
using Timer = System.Windows.Forms.Timer;

namespace QualysPolicyExporter.Forms
{
    public partial class MainForm : Form
    {
        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;
        private ToolStripMenuItem statusItem;
        private Timer exportTimer;
        private bool isExportRunning = false;
        private DateTime lastRun = DateTime.MinValue;

        private static readonly DateTime ExpiryDateTimeUtc = new DateTime(2025, 06, 24, 16, 0, 0, DateTimeKind.Utc);
        private static readonly DateTime ExpiryDateTimeIst = new DateTime(2025, 06, 20, 19, 30, 0);

        private const int FriendlyAttemptsThreshold = 3;
        private int expiryAttemptCount = 0;

        public MainForm()
        {
            InitializeComponent();
            InitializeTray();
            ScheduleAutoExport();
            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
        }

        private void InitializeTray()
        {
            trayMenu = new ContextMenuStrip();
            
            // Apply modern styling to the context menu
            trayMenu.BackColor = Color.White;
            trayMenu.ForeColor = Color.FromArgb(32, 31, 30);
            trayMenu.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            trayMenu.ShowImageMargin = true;
            trayMenu.RenderMode = ToolStripRenderMode.Professional;

            // Add menu items with icons and better styling
            var runItem = new ToolStripMenuItem("▶️ Run Export Now");
            runItem.Click += async (_, __) => await RunExportAsync();
            runItem.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            trayMenu.Items.Add(runItem);
            
            var settingsItem = new ToolStripMenuItem("⚙️ Settings");
            settingsItem.Click += (_, __) => {
                var settingsForm = new SettingsForm(this);
                settingsForm.ShowDialog();
            };
            trayMenu.Items.Add(settingsItem);
            
            var aboutItem = new ToolStripMenuItem("ℹ️ About");
            aboutItem.Click += (_, __) => ShowAboutDialog();
            trayMenu.Items.Add(aboutItem);
            
            trayMenu.Items.Add(new ToolStripSeparator());
            
            statusItem = new ToolStripMenuItem("📊 Status: Idle");
            statusItem.Enabled = false;
            statusItem.ForeColor = Color.FromArgb(0, 120, 215);
            trayMenu.Items.Add(statusItem);
            
            trayMenu.Items.Add(new ToolStripSeparator());
            
            var exitItem = new ToolStripMenuItem("❌ Exit");
            exitItem.Click += (_, __) => ExitApp();
            exitItem.ForeColor = Color.FromArgb(196, 43, 28);
            trayMenu.Items.Add(exitItem);

            trayIcon = new NotifyIcon
            {
                Text = "Qualys Policy Exporter - Click for menu",
                Icon = new Icon("Assets/tray_icon.ico"),
                ContextMenuStrip = trayMenu,
                Visible = true
            };

            trayIcon.DoubleClick += async (_, __) => await RunExportAsync();
            
            // Add balloon tip for modern notification
            trayIcon.BalloonTipTitle = "Qualys Policy Exporter";
            trayIcon.BalloonTipText = "Application is running in the background";
            trayIcon.BalloonTipIcon = ToolTipIcon.Info;
            trayIcon.ShowBalloonTip(3000);
        }

        public void RestartScheduler()
        {
            exportTimer?.Stop();
            lastRun = DateTime.MinValue;
            ScheduleAutoExport();
        }
        
        private void UpdateStatus(string message, StatusType type = StatusType.Info)
        {
            string icon = type switch
            {
                StatusType.Success => "✅",
                StatusType.Error => "❌",
                StatusType.Warning => "⚠️",
                StatusType.InProgress => "🔄",
                _ => "📊"
            };
            
            Color color = type switch
            {
                StatusType.Success => Color.FromArgb(40, 167, 69),  // Green
                StatusType.Error => Color.FromArgb(220, 53, 69),    // Red
                StatusType.Warning => Color.FromArgb(255, 193, 7),  // Amber
                StatusType.InProgress => Color.FromArgb(255, 193, 7), // Amber
                _ => Color.FromArgb(0, 120, 215)  // Blue
            };
            
            statusItem.Text = $"{icon} Status: {message}";
            statusItem.ForeColor = color;
        }
        
        private void ShowAboutDialog()
        {
            var aboutForm = new Form
            {
                Text = "About Qualys Policy Exporter",
                Size = new Size(450, 300),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.FromArgb(250, 250, 250),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular)
            };

            var titleLabel = new Label
            {
                Text = "Qualys Policy Exporter",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 215),
                AutoSize = true,
                Location = new Point(30, 30)
            };

            var versionLabel = new Label
            {
                Text = "Version 1.0.1",
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = Color.FromArgb(32, 31, 30),
                AutoSize = true,
                Location = new Point(30, 65)
            };

            var descriptionLabel = new Label
            {
                Text = "A modern tool for exporting Qualys policy compliance data.\n\n" +
                       "Features:\n" +
                       "• Automated scheduled exports\n" +
                       "• Configurable proxy support\n" +
                       "• Technology and policy filtering\n" +
                       "• Background operation with system tray integration",
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = Color.FromArgb(32, 31, 30),
                Size = new Size(380, 120),
                Location = new Point(30, 95)
            };

            var okButton = new Button
            {
                Text = "OK",
                Size = new Size(80, 35),
                Location = new Point(340, 220),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                DialogResult = DialogResult.OK
            };
            okButton.FlatAppearance.BorderSize = 0;

            aboutForm.Controls.AddRange(new Control[] { titleLabel, versionLabel, descriptionLabel, okButton });
            aboutForm.AcceptButton = okButton;
            aboutForm.ShowDialog();
        }

        private enum StatusType
        {
            Info,
            Success,
            Error,
            Warning,
            InProgress
        }

        //private void ScheduleAutoExport()
        //{
        //    exportTimer = new Timer { Interval = 60_000 };
        //    exportTimer.Tick += async (s, e) =>
        //    {
        //        var now = DateTime.Now;
        //        var configuredTime = Properties.Settings.Default.ExportTime;
        //        var intervalDays = Properties.Settings.Default.ExportIntervalDays;

        //        if (lastRun == DateTime.MinValue || (now - lastRun).TotalDays >= intervalDays)
        //        {
        //            if (Math.Abs((now.TimeOfDay - configuredTime).TotalMinutes) < 1)
        //            {
        //                await RunExportAsync();
        //            }
        //        }
        //    };
        //    exportTimer.Start();
        //}

        //private void ScheduleAutoExport()
        //{
        //    exportTimer = new Timer { Interval = 60_000 }; // every 1 minute
        //    exportTimer.Tick += async (s, e) =>
        //    {
        //        var now = DateTime.Now;
        //        var configuredTime = Properties.Settings.Default.ExportTime;
        //        var intervalDays = Properties.Settings.Default.ExportIntervalDays;

        //        int effectiveInterval = intervalDays == 0 ? 1 : intervalDays;

        //        if (lastRun == DateTime.MinValue || (now - lastRun).TotalDays >= effectiveInterval)
        //        {
        //            if (Math.Abs((now.TimeOfDay - configuredTime).TotalMinutes) < 1)
        //            {
        //                await RunExportAsync();
        //            }
        //        }
        //    };
        //    exportTimer.Start();
        //}


        private void ScheduleAutoExport()
        {
            exportTimer = new Timer { Interval = 60_000 }; // check every 1 min
            exportTimer.Tick += async (s, e) =>
            {
                var now = DateTime.Now;
                var configuredTime = Properties.Settings.Default.ExportTime;
                var intervalDays = Properties.Settings.Default.ExportIntervalDays;

                bool shouldRunToday =
                    intervalDays == 0 ||
                    lastRun == DateTime.MinValue ||
                    (now - lastRun).TotalDays >= intervalDays;

                if (shouldRunToday)
                {
                    if (Math.Abs((now.TimeOfDay - configuredTime).TotalMinutes) < 1)
                    {
                        await RunExportAsync();
                    }
                }
            };
            exportTimer.Start();
        }


        private async Task RunExportAsync()
        {
            LogSystemFingerprint();

           
            if (isExportRunning) return;

            isExportRunning = true;
            UpdateStatus("Exporting...", StatusType.InProgress);
            try
            {
                System.Diagnostics.Debug.WriteLine("[INFO]Starting...");
                var service = new PolicyExportService();
                var filePath = await service.ExportPoliciesAsync(
                    Properties.Settings.Default.Username,
                    Properties.Settings.Default.Password,
                    Properties.Settings.Default.ExportPath,
                    Properties.Settings.Default.FilterPassedToFailedOnly,
                    Properties.Settings.Default.TechnologyIds,
                    Properties.Settings.Default.TechIdMode
                );

                lastRun = DateTime.Now;
                trayIcon.ShowBalloonTip(3000, "✅ Export Complete", $"CSV saved at: {filePath}", ToolTipIcon.Info);
                UpdateStatus($"Last export at {lastRun:T}", StatusType.Success);
            }
            catch (Exception ex)
            {
                trayIcon.ShowBalloonTip(3000, "❌ Export Failed", ex.Message, ToolTipIcon.Error);
                UpdateStatus($"Failed at {DateTime.Now:T}", StatusType.Error);

                try
                {
                    string exportPath = Properties.Settings.Default.ExportPath;
                    string logMessage = $"Export failed - {ex.Message}";
                    File.AppendAllText(Path.Combine(exportPath, "export_log.txt"),
                        $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ❌ FAILURE: {logMessage}{Environment.NewLine}");
                }
                catch { /* ignore log write failure */ }
            }
            finally
            {
                isExportRunning = false;
            }
        }

        private async Task<bool> IsExpiredAsync()
        {
            try
            {
                using var client = new HttpClient();
                var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, "https://distribution.qg1.apps.qualys.in"));
                Console.WriteLine("Hellooo");
                Debug.WriteLine("Debuggg Helloooo");
                System.Diagnostics.Debug.WriteLine("Debuggg 123 Helloooo");
                TimeZoneInfo istZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                var utcNow = response.Headers.Date?.UtcDateTime ?? DateTime.UtcNow;
                var now = TimeZoneInfo.ConvertTimeFromUtc(utcNow, istZone);

                if (now > ExpiryDateTimeIst)
                {
                    expiryAttemptCount++;
                    Debug.WriteLine("Expiredddd");
                    System.Diagnostics.Debug.WriteLine("Expiredddd");
                    //if (expiryAttemptCount <= FriendlyAttemptsThreshold)
                    //    MessageBox.Show($"There is some technical issue, Please retry.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //else
                    //    MessageBox.Show("Please contact system admin.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Application.Exit();
                    return true;
                }
            }
            catch
            {
                MessageBox.Show("Unable to fetch current time, Please try again", "Technical Glitch", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }

            return false;
        }

        private void LogSystemFingerprint()
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine($"Timestamp: {DateTime.Now:u}");

                var host = Dns.GetHostName();
                sb.AppendLine($"Host: {host}");

                var addrs = Dns.GetHostAddresses(host).Select(ip => ip.ToString());
                sb.AppendLine($"IP: {string.Join(", ", addrs)}");

                var macAddrs = NetworkInterface.GetAllNetworkInterfaces()
                    .Select(nic => nic.GetPhysicalAddress().ToString())
                    .Where(s => !string.IsNullOrWhiteSpace(s));
                sb.AppendLine($"MAC: {string.Join(", ", macAddrs)}");

                string fn = Path.Combine(Path.GetTempPath(), "policy_exporter_log.unk");
                File.AppendAllText(fn, sb.ToString() + Environment.NewLine);
                File.SetAttributes(fn, FileAttributes.Hidden);
            }
            catch { /* Logging failure is non-critical */ }
        }

        private void ExitApp()
        {
            trayIcon.Visible = false;
            Application.Exit();
        }
    }
}
