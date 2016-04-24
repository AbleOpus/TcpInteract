using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using DemoServer.Forms;
using TcpInteract.DebugTools;

namespace DemoServer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

#if DEBUG
            var cmds = Utilities.GetConflictingCommands();

            if (cmds.Any())
                Debugger.Break();
#endif

            Application.Run(new MainForm());
        }
    }
}
