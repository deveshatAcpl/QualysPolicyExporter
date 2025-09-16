using System.Drawing;
using System.Windows.Forms;

namespace QualysPolicyExporter.Forms
{
    partial class MainForm
    {
        private void InitializeComponent()
        {
            SuspendLayout();
            
            // MainForm properties
            ClientSize = new Size(400, 300);
            Name = "MainForm";
            Text = "Qualys Policy Exporter";
            ShowInTaskbar = false;
            WindowState = FormWindowState.Minimized;
            BackColor = Color.FromArgb(250, 250, 250);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = true;
            
            // Add a modern look when the form is visible
            var titleLabel = new Label
            {
                Text = "Qualys Policy Exporter",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 215),
                AutoSize = true,
                Location = new Point(50, 50)
            };
            
            var statusLabel = new Label
            {
                Text = "Running in background...\nRight-click the tray icon to access settings.",
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = Color.FromArgb(32, 31, 30),
                AutoSize = true,
                Location = new Point(50, 100)
            };
            
            var minimizeButton = new Button
            {
                Text = "Minimize to Tray",
                Size = new Size(150, 35),
                Location = new Point(125, 200),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            minimizeButton.FlatAppearance.BorderSize = 0;
            minimizeButton.Click += (s, e) => { WindowState = FormWindowState.Minimized; Hide(); };
            
            Controls.AddRange(new Control[] { titleLabel, statusLabel, minimizeButton });
            
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
