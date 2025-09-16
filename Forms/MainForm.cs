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

            trayMenu.Items.Add("Run Export Now", null, async (_, __) => await RunExportAsync());
            trayMenu.Items.Add("Settings", null, (_, __) => new SettingsForm(this).ShowDialog());
            trayMenu.Items.Add(new ToolStripSeparator());
            statusItem = new ToolStripMenuItem("Status: Idle");
            trayMenu.Items.Add(statusItem);
            trayMenu.Items.Add(new ToolStripSeparator());
            trayMenu.Items.Add("Exit", null, (_, __) => ExitApp());

            trayIcon = new NotifyIcon
            {
                Text = "Qualys Policy Exporter",
                Icon = new Icon("Assets/tray_icon.ico"),
                ContextMenuStrip = trayMenu,
                Visible = true
            };

            trayIcon.DoubleClick += async (_, __) => await RunExportAsync();
        }

        public void RestartScheduler()
        {
            exportTimer?.Stop();
            lastRun = DateTime.MinValue;
            ScheduleAutoExport();
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
            statusItem.Text = "Status: Exporting...";
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
                trayIcon.ShowBalloonTip(3000, "Export Complete", $"CSV saved at: {filePath}", ToolTipIcon.Info);
                statusItem.Text = $"Status: Last export at {lastRun:T}";
            }
            catch (Exception ex)
            {
                trayIcon.ShowBalloonTip(3000, "Export Failed", ex.Message, ToolTipIcon.Error);
                statusItem.Text = $"Status: Failed at {DateTime.Now:T}";

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
