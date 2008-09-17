using System;
using System.Collections.Generic;
using System.Text;

namespace MPlayerFront
{
    public class MFCommand
    {
        public event EventHandler CommandExit;
        public event EventHandler Output;

        private string cmd_ = null;
        private string arguments_ = null;
        private string cmdFileName_ = null;

        private System.Diagnostics.Process process_ = null;
        private System.IO.StringWriter sw_ = null;
        private System.Threading.Thread readThread_ = null;
        private bool saveOutput_ = true;

        public MFCommand(string cmd, string arguments)
        {
            cmd_ = cmd;
            arguments_ = arguments;
        }

        public MFCommand(string cmd, string arguments, bool saveOutput)
            : this(cmd, arguments)
        {
            saveOutput_ = saveOutput;
        }

        private static string GetCommandString(string cmd)
        {
            if (Program.Options.Host == null
                || Program.Options.Host.Length == 0
                || Program.Options.User == null
                || Program.Options.User.Length == 0)
            {
                return cmd;
            }

            StringBuilder sb = new StringBuilder();

            if (Program.Options.UseSSH)
            {
                sb.Append("-ssh ");
            }

            sb.Append(Program.Options.User);
            sb.Append("@");

            sb.Append(Program.Options.Host);
            sb.Append(" ");

            sb.Append("-pw ");
            sb.Append(Program.Options.Password);
            sb.Append(" ");

            if (Program.Options.KeyFile != null &&
                Program.Options.KeyFile.Trim().Length > 0)
            {
                sb.Append("-i ");
                sb.Append(Program.Options.KeyFile);
                sb.Append(" ");
            }

            sb.Append("-P ");
            sb.Append(Program.Options.Port);
            sb.Append(" ");

            sb.Append(cmd);

            return sb.ToString();
        }

        private void ReadData()
        {
            while (!process_.HasExited)
            {
                string line = process_.StandardOutput.ReadLine();

                while (line != null)
                {
                    if (saveOutput_)
                    {
                        sw_.WriteLine(line);
                    }

                    if (Output != null)
                    {
                        Output.BeginInvoke(line,
                            new EventArgs(),
                            new AsyncCallback(OutputCallback),
                            this);
                    }
                    else
                    {
                        System.Diagnostics.Trace.WriteLine(line);
                    }

                    line = process_.StandardOutput.ReadLine();
                }

                if (!process_.HasExited)
                {
                    System.Threading.Thread.Sleep(1000);
                }
            }
        }

        private void OutputCallback(IAsyncResult ar)
        {
            Output.EndInvoke(ar);
        }

        public bool Execute()
        {
            return Execute(false);
        }

        public bool Execute(bool debug)
        {
            if (process_ == null || process_.HasExited)
            {
                process_ = CreateProcess(cmd_, arguments_, debug);

                sw_ = new System.IO.StringWriter();

                bool result = process_.Start();

                readThread_ = new System.Threading.Thread(new System.Threading.ThreadStart(ReadData));
                readThread_.Start();

                return result;
            }

            return true;
        }

        private System.Diagnostics.Process CreateProcess(string cmd, string arguments, bool debug)
        {
            while (Program.Options.Host == null
                            || Program.Options.Host.Length == 0
                            || Program.Options.User == null
                            || Program.Options.User.Length == 0)
            {
                Program.MainFrm.settingsToolStripMenuItem_Click(this, new EventArgs());
            }

            System.Diagnostics.Process p = new System.Diagnostics.Process();

            string cmdStr = GetCommandString("");

            if (!debug)
            {
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            }

            p.StartInfo.FileName = Program.Options.PathToPLink;

            cmdFileName_ = CreateTempFileName(cmd, arguments);

            p.StartInfo.Arguments = cmdStr + " -m " +
                cmdFileName_;

            p.Exited += new EventHandler(Process_Exited);

            return p;
        }

        private string CreateTempFileName(string cmd, string arguments)
        {
            string fileName = System.IO.Path.GetTempFileName();

            using (System.IO.FileStream fs = 
                new System.IO.FileStream(fileName, 
                    System.IO.FileMode.OpenOrCreate,
                    System.IO.FileAccess.Write))
            {
                byte[] buf = Encoding.UTF8.GetBytes(cmd);
                fs.Write(buf,0,buf.Length);
                fs.Write(Encoding.UTF8.GetBytes(" "),0,1);

                buf = Encoding.UTF8.GetBytes(MFHelper.NormalizeFileName(arguments));

                fs.Write(buf,0,buf.Length);
            }

            return fileName;
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            try
            {
                System.IO.File.Delete(cmdFileName_);
            }
            catch
            {
            }

            if (CommandExit != null)
            {
                CommandExit.BeginInvoke(this, e,
                    new AsyncCallback(CommandExitCallback),
                    this);
            }
        }

        private void CommandExitCallback(IAsyncResult ar)
        {
            CommandExit.EndInvoke(ar);
        }

        public void WaitForExit()
        {
            if (process_ != null && !process_.HasExited)
            {
                process_.WaitForExit();
                readThread_.Join();
            }
        }

        public void WriteStandardInput(string data)
        {
            if (process_ != null && !process_.HasExited)
            {
                process_.StandardInput.Write(data);
            }
        }

        public string ReadStandardOutput()
        {
            return sw_.ToString();
        }

        public static string ExecuteCommand(string commands, string arguments)
        {
            MFCommand cmd = new MFCommand(commands, arguments);

            if (cmd.Execute())
            {
                cmd.WaitForExit();

                return cmd.ReadStandardOutput();
            }
            else
            {
                return "";
            }
        }
    }
}
