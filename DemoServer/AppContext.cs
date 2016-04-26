using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using DemoServer.Forms;
using DemoShared;
using TcpInteract;
using TcpInteract.DebugTools;

namespace DemoServer
{
    class AppContext : ApplicationContext
    {
        private readonly ServerForm form = new ServerForm();
        private readonly MessengerServer server;
        private const string INDENT = "    ";
        private const string SEPARATOR = "=============================================================================";

        public AppContext()
        {
#if DEBUG
            var cmds = Utilities.GetConflictingCommands();
            if (cmds.Any()) Debugger.Break();
#endif
            MainForm = form;
            server = new MessengerServer(2059);
            server.Pusher.Bind<InstantMessageContent>(e => form.WriteLine(e.ToString()));
            server.Pusher.Bind<LoginContent>(e => form.WriteLine($@"{e.ClientName} has logged in."));
            server.Pusher.Bind<LogoutContent>(e => form.WriteLine($@"{e.ClientName} has logged out. Reason: {e.Reason}."));
            server.Pusher.Bind<ConnectionRefusedContent>(e =>
            {
                if (e.Reason == ConnectionRefusedReason.EmptyName)
                {
                    form.WriteLine("A client tried to login with an empty name.");
                }
                else
                {
                    form.WriteLine($@"{e.ClientName} has been refused, reasons: {e.Reason}.");
                }
            });

            form.CommandSubmitted += ConsoleCommandSubmitted;
            server.Start();
        }

        private void ConsoleCommandSubmitted(object sender, string cmd)
        {
            cmd = cmd.TrimStart();
            if (cmd.Length == 0) return;
            string doesNotTakeArgs = null;
            string missingArgs = null;

            foreach (var method in GetConsoleCommandMethods())
            {
                var parameters = method.GetParameters();

                if (parameters.Length > 1)
                {
                    throw new Exception($@"The command method ""{method.Name}"" has too many parameters.");
                }

                if (parameters.Length == 1 && parameters[0].ParameterType != typeof(string))
                {
                    throw new Exception($@"The command method ""{method.Name}"" has a non-string parameter.");
                }

                string methCmdName = method.Name.ToLower().Remove(method.Name.Length - 3, 3);

                if (cmd.StartsWith(methCmdName, StringComparison.OrdinalIgnoreCase))
                {
                    string data = cmd.Remove(0, methCmdName.Length);

                    if (data.StartsWith(" "))
                    {
                        // Remove 1 space if space is present.
                        data = data.Remove(0, 1);
                    }

                    if (data.Length > 0 && parameters.Length == 0)
                    {
                        doesNotTakeArgs = methCmdName;
                        continue;
                    }

                    if (data.Length == 0 && parameters.Length == 1)
                    {
                        missingArgs = methCmdName;
                        continue;
                    }

                    object[] args = parameters.Length == 1 ? new object[] { data } : null;
                    method.Invoke(this, args);
                    return;
                }
            }

            // Command didn't execute properly. Check to see if any methods correspond
            // to one of the potential issues.
            if (doesNotTakeArgs != null)
            {
                form.WriteLine($@"The ""{doesNotTakeArgs}"" command does not take an argument.");
            }
            else if (missingArgs != null)
            {
                form.WriteLine($@"The ""{missingArgs}"" command needs an argument.");
            }
            else
            {
                form.WriteLine("Command is invalid.");
            }
        }

        private IEnumerable<MethodInfo> GetConsoleCommandMethods()
        {
            return GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(m => m.Name.EndsWith("Cmd"));
        }

        #region Console Command methods
        [Description("Clears the output window.")]
        private void ClearCmd()
        {
            form.ClearOutput();
        }

        [Description("Lists all of the clients that are connected.")]
        private void ListClientsCmd()
        {
            if (server.ClientNames.Count > 0)
            {
                WriteSeperator();
                form.WriteLine("Client Names:");
                form.WriteLines(server.ClientNames);
                WriteSeperator();
            }
            else
            {
                form.WriteLine("No clients are connected.");
            }
        }

        [Description("Sends a goodbye message to the clients, then stops the server.")]
        private void StopCmd(string message)
        {
            server.Stop(message);

            if (String.IsNullOrWhiteSpace(message))
                form.WriteLine(@"Server stopped.");
            else
                form.WriteLine($@"Server stopped with message ""{message}"".");
        }

        [Description("Stops the server without sending a message.")]
        private void StopCmd()
        {
            StopCmd(null);
        }

        [Description("Starts the server.")]
        private void StartCmd()
        {
            if (server.Enabled)
            {
                form.WriteLine("Server already started.");
            }
            else
            {
                server.Start();
                form.WriteLine("Server started.");
            }
        }

        [Description("Sends a goodbye message to the clients, then restarts the server.")]
        private void RestartCmd(string message)
        {
            StopCmd(message);
            StartCmd();
        }

        [Description("Restarts the server without sending a message.")]
        private void RestartCmd()
        {
            RestartCmd(null);
        }

        [Description("Displays all of the available console commands.")]
        private void HelpCmd()
        {
            WriteSeperator();
            form.WriteLine("Command Information:");

            foreach (var method in GetConsoleCommandMethods())
            {
                var attrib = method.GetCustomAttribute<DescriptionAttribute>();
                bool takesArgument = method.GetParameters().Length == 1;

                if (attrib != null)
                {
                    string command = method.Name.Remove(method.Name.Length - 3, 3);
                    string argIndicator = takesArgument ? "{argument} " : string.Empty;
                    form.WriteLine($@"{INDENT} {command} {argIndicator}- {attrib.Description}");
                }
            }

            WriteSeperator();
        }

        [Description(@"Kicks the specified client. Ex. ""kick Bob"".")]
        private void KickCmd(string argument)
        {
            try
            {
                server.KickClient(argument);
            }
            catch (ArgumentException)
            {
                form.WriteLine($@"Client ""{argument}"" not found.");
            }
        }

        [Description(@"Gets significant information about the specified client.")]
        private void ClientInfoCmd(string clientName)
        {
            var info = server.GetClientInfo(clientName);

            if (info == null)
            {
                form.WriteLine("Specified client does not exist.");
            }
            else
            {
                WriteSeperator();
                form.WriteLines(info.GetLines());
                WriteSeperator();
            }
        }

        [Description(@"Writes the remote or public IP of this network.")]
        private async void MyIpCmd()
        {
            string address = await Networking.GetPublicIpStringTaskAsync();
            form.WriteLine(address);
        }

        [Description(@"Gets significant information about all of the connected clients.")]
        private void ClientInfoCmd()
        {
            if (server.ClientNames.Count == 0)
            {
                form.WriteLine("No clients connected.");
                return;
            }

            WriteSeperator();

            foreach (var clientName in server.ClientNames)
            {
                var info = server.GetClientInfo(clientName);

                if (info != null)
                {
                    foreach (var line in info.GetLines())
                    {
                        // Indent all but the first line.
                        string indent = line.StartsWith("Name") ? string.Empty : INDENT;
                        form.WriteLine(indent + line);
                    }
                }
            }

            WriteSeperator();
        }
        #endregion

        /// <summary>
        /// Writes a separator to the output window if a separator does not already exist at the
        /// end of the output text.
        /// </summary>
        private void WriteSeperator()
        {
            // Do not layer separators, only one is needed.
            bool isSeperated = false;

            for (int i = form.OutputTextBox.Lines.Length - 1; i > 0; i--)
            {
                string line = form.OutputTextBox.Lines[i];

                if (String.IsNullOrWhiteSpace(line))
                    continue;

                if (line.Contains(SEPARATOR))
                {
                    isSeperated = true;
                }
                else
                {
                    break;
                }
            }

            if (!isSeperated)
                form.WriteLine(SEPARATOR);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                server.Stop();
                form.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
