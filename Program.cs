using System;
using System.Windows.Forms;
using QualysPolicyExporter.Forms;

namespace QualysPolicyExporter
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
