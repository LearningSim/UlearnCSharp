using System;
using System.Windows.Forms;

namespace TableParser
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            Console.WriteLine("\"\\\\\" b");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ParserMainForm());
        }
    }
}