using System;
using System.Windows.Forms;

namespace Safety_Equipment_Control
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // CHANGE HERE: Run LoginForm instead of Form1
            Application.Run(new LoginForm());
        }
    }
}