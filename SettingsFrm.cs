using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MPlayerFront
{
    public partial class SettingsFrm : Form
    {
        public SettingsFrm()
        {
            InitializeComponent();
        }

        private void SettingsFrm_Load(object sender, EventArgs e)
        {
            txtHost.Text = Program.Options.Host;
            txtUser.Text = Program.Options.User;
            txtPassword.Text = Program.Options.Password;
            chkUseSSH.Checked = Program.Options.UseSSH;
            txtPlinkPath.Text = Program.Options.PathToPLink;
            txtPort.Text = Program.Options.Port.ToString();
            txtKeyFile.Text = Program.Options.KeyFile;

            txtListFiles.Text = Program.Options.Commands.ListFiles;
            txtCdDirectory.Text = Program.Options.Commands.CDDirectory;
            txtCurrentDirectory.Text = Program.Options.Commands.CurrentDirectory;

            txtPlayFile.Text = Program.Options.Controls.PlayFile;
            txtPlayList.Text = Program.Options.Controls.PlayList;
            txtPause.Text = Program.Options.Controls.Pause;
            txtStop.Text = Program.Options.Controls.Stop;
            txtVolumeDown.Text = Program.Options.Controls.VolumeDown;
            txtVolumeUp.Text = Program.Options.Controls.VolumeUp;
            //txtListForward.Text = Program.Options.Controls.PlayListForward;
            //txtListBackward.Text = Program.Options.Controls.PlayListBackwords;

            //txtF10Secs.Text = Program.Options.Controls.Seeking.F10Secs;
            //txtB10Secs.Text = Program.Options.Controls.Seeking.B10Secs;
            //txtF10Mins.Text = Program.Options.Controls.Seeking.F10Mins;
            //txtB10Mins.Text = Program.Options.Controls.Seeking.B10Mins;
            //txtF1Mins.Text = Program.Options.Controls.Seeking.F1Mins;
            //txtB1Mins.Text = Program.Options.Controls.Seeking.B1Mins;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

            Program.Options.Host = txtHost.Text;
            Program.Options.User = txtUser.Text;
            Program.Options.Password = txtPassword.Text;
            Program.Options.PathToPLink = txtPlinkPath.Text;

            Program.Options.UseSSH = chkUseSSH.Checked;

            Program.Options.KeyFile = txtKeyFile.Text.Trim();
            try
            {
                Program.Options.Port = int.Parse(txtPort.Text.Trim());
            }
            catch
            {
                Program.Options.Port = 22;
            }

            Program.Options.Commands.ListFiles = txtListFiles.Text;
            Program.Options.Commands.CDDirectory = txtCdDirectory.Text;
            Program.Options.Commands.CurrentDirectory = txtCurrentDirectory.Text;

            Program.Options.Controls.PlayFile = txtPlayFile.Text;
            Program.Options.Controls.PlayList = txtPlayList.Text;
            Program.Options.Controls.Pause = txtPause.Text;
            Program.Options.Controls.Stop = txtStop.Text;
            Program.Options.Controls.VolumeDown = txtVolumeDown.Text;
            Program.Options.Controls.VolumeUp = txtVolumeUp.Text;
            //Program.Options.Controls.PlayListForward = txtListForward.Text;
            //Program.Options.Controls.PlayListBackwords = txtListBackward.Text;

            //Program.Options.Controls.Seeking.F10Secs = txtF10Secs.Text;
            //Program.Options.Controls.Seeking.B10Secs = txtB10Secs.Text;
            //Program.Options.Controls.Seeking.F10Mins = txtF10Mins.Text;
            //Program.Options.Controls.Seeking.B10Mins = txtB10Mins.Text;
            //Program.Options.Controls.Seeking.F1Mins = txtF1Mins.Text;
            //Program.Options.Controls.Seeking.B1Mins = txtB1Mins.Text;

            Program.SaveOptions();

            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "plink.exe";
            openFileDialog1.Filter = "plink.exe|plink.exe";

            if (DialogResult.OK == openFileDialog1.ShowDialog(this))
            {
                txtPlinkPath.Text = openFileDialog1.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "Key File |*.ppk";

            if (DialogResult.OK == openFileDialog1.ShowDialog(this))
            {
                txtKeyFile.Text = openFileDialog1.FileName;
            }
        }

        private void txtPort_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                int.Parse(txtPort.Text.Trim());
            }
            catch
            {
                e.Cancel = true;
            }
        }
    }
}