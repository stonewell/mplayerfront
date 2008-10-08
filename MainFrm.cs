using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MPlayerFront
{
    public partial class MainFrm : Form
    {
        private enum MFStateEnum
        {
            Playing,
            Paused,
            Stopped
        }

        private static readonly System.Xml.Serialization.XmlSerializer xs_ =
            new System.Xml.Serialization.XmlSerializer(typeof(MFPlayList));

        private OpenFileFrm openFileFrm_ = new OpenFileFrm();
        private SettingsFrm settingsFrm_ = new SettingsFrm();
        private string currentFile_ = null;
        private MFCommand command_ = null;
        private MFStateEnum state_ = MFStateEnum.Stopped;

        public MainFrm()
        {
            InitializeComponent();
        }

        public void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (settingsFrm_.ShowDialog(this) == DialogResult.OK)
            {
            }
        }

        private void addFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileFrm_.ShowDialog(this) == DialogResult.OK)
            {
                foreach (string file in openFileFrm_.SelectedFiles)
                {
                    if (!lvwPlayList.Items.ContainsKey(file))
                    {
                        this.lvwPlayList.Items.Add(file, file, -1);
                    }
                }

                lvwPlayList.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            }
        }

        private void removeFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<ListViewItem> items = new List<ListViewItem>();

            foreach (ListViewItem item in lvwPlayList.SelectedItems)
            {
                items.Add(item);
            }

            foreach (ListViewItem item in items)
            {
                lvwPlayList.Items.Remove(item);

                if (item.Text == currentFile_)
                {
                    currentFile_ = null;
                }
            }
        }

        private void playToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cursor oldCursor = Cursor;

            Cursor = Cursors.WaitCursor;

            try
            {
                if (state_ == MFStateEnum.Paused)
                {
                    pauseToolStripMenuItem_Click(sender, e);
                    return;
                }
                else if (state_ == MFStateEnum.Playing)
                {
                    stopToolStripMenuItem_Click(sender, e);
                }

                string playFile = null;

                if (currentFile_ == null)
                {
                    if (lvwPlayList.SelectedItems.Count > 0)
                    {
                        playFile = lvwPlayList.SelectedItems[0].Text;
                    }
                    else if (lvwPlayList.Items.Count > 0)
                    {
                        playFile = lvwPlayList.Items[0].Text;
                    }
                }
                else
                {
                    playFile = currentFile_;
                }

                if (playFile != null)
                {
                    if (!playFile.Equals(currentFile_))
                    {
                        UpdateCurrentPlayingFile(playFile);
                    }

                    command_ = new MFCommand(Program.Options.Controls.RealPlayFile,
                        currentFile_, false);

                    if (command_.Execute())
                    {
                        command_.CommandExit += new EventHandler(command__CommandExit);
                        command_.Output += new EventHandler(command__Output);

                        state_ = MFStateEnum.Playing;
                    }
                    else
                    {
                        command_.WaitForExit();
                        command_ = null;
                    }
                }
            }
            finally
            {
                Cursor = oldCursor;
            }
        }

        private void command__Output(object sender, EventArgs e)
        {
            if (lstOutput.InvokeRequired)
            {
                lstOutput.Invoke(new EventHandler(command__Output), sender, e);
            }
            else
            {
                int index = lstOutput.Items.Add(sender);
                lstOutput.SelectedIndex = index;
            }
        }

        private void command__CommandExit(object sender, EventArgs e)
        {
            if (command_!= null && sender == command_)
            {
                if (state_ == MFStateEnum.Playing)
                {
                    state_ = MFStateEnum.Stopped;

                    command_.WaitForExit();
                    command_ = null;

                    if (lvwPlayList.InvokeRequired)
                    {
                        lvwPlayList.Invoke(new EventHandler(nextFileToolStripMenuItem_Click), sender, e);
                    }
                    else
                    {
                        nextFileToolStripMenuItem_Click(sender, e);
                    }
                }
            }
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (command_ != null && state_ != MFStateEnum.Stopped)
            {
                if (state_ != MFStateEnum.Paused)
                {
                    state_ = MFStateEnum.Paused;
                }
                else
                {
                    state_ = MFStateEnum.Playing;
                }

                command_.WriteStandardInput(Program.Options.Controls.Pause);
            }
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cursor old = Cursor;

            Cursor = Cursors.WaitCursor;

            try
            {
                if (command_ != null)
                {
                    command_.CommandExit -= new EventHandler(command__CommandExit);
                    command_.Output -= new EventHandler(command__Output);

                    state_ = MFStateEnum.Stopped;

                    command_.WriteStandardInput(Program.Options.Controls.Stop);
                    command_.WaitForExit();

                    command_ = null;
                }
            }
            finally
            {
                Cursor = old;
            }
        }

        private void volumeUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (command_ != null)
            {
                command_.WriteStandardInput(Program.Options.Controls.VolumeUp);
            }
        }

        private void volumeDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (command_ != null)
            {
                command_.WriteStandardInput(Program.Options.Controls.VolumeDown);
            }
        }

        private void nextFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int index = GetCurrentPlayingIndex();

            index++;

            if (state_ != MFStateEnum.Stopped)
            {
                stopToolStripMenuItem_Click(sender, e);
            }

            if (index >= 0 && index < lvwPlayList.Items.Count)
            {
                UpdateCurrentPlayingFile(lvwPlayList.Items[index].Text);

                playToolStripMenuItem_Click(sender, e);
            }
        }

        private void previousFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int index = GetCurrentPlayingIndex();

            index--;

            if (state_ != MFStateEnum.Stopped)
            {
                stopToolStripMenuItem_Click(sender, e);
            }

            if (index >= 0 && index < lvwPlayList.Items.Count)
            {
                UpdateCurrentPlayingFile(lvwPlayList.Items[index].Text);

                playToolStripMenuItem_Click(sender, e);
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox about = new AboutBox();

            about.ShowDialog(this);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void MainFrm_Load(object sender, EventArgs e)
        {
            LoadDefaultPlayList();
        }

        private void LoadDefaultPlayList()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory +
                System.IO.Path.DirectorySeparatorChar +
                "playlist.config";

            LoadPlayList(path);
        }

        private void LoadPlayList(string path)
        {
            lvwPlayList.Items.Clear();

            try
            {
                if (System.IO.File.Exists(path))
                {
                    using (System.IO.Stream s = new System.IO.FileStream(path,
                        System.IO.FileMode.Open,
                        System.IO.FileAccess.Read))
                    {
                        MFPlayList playList = xs_.Deserialize(s) as MFPlayList;

                        foreach (string item in playList.Files)
                        {
                            if (!lvwPlayList.Items.ContainsKey(item))
                            {
                                this.lvwPlayList.Items.Add(item, item, -1);
                            }
                        }

                        lvwPlayList.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);

                        if (playList.CurrentFile != null && playList.CurrentFile.Length > 0)
                        {
                            foreach (ListViewItem item in lvwPlayList.Items)
                            {
                                if (item.Text.Equals(playList.CurrentFile))
                                {
                                    item.Selected = true;
                                    break;
                                }
                            }

                            UpdateCurrentPlayingFile(playList.CurrentFile);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,
                    "Invalid Play List file:" + path + ",Error:" + ex.Message,
                    "Load Play List Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void SaveDefaultPlayList()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory +
                System.IO.Path.DirectorySeparatorChar +
                "playlist.config";

            SavePlayList(path);
        }

        private void SavePlayList(string path)
        {
            MFPlayList playList = new MFPlayList();

            foreach (ListViewItem item in lvwPlayList.Items)
            {
                playList.Files.Add(item.Text);
            }

            playList.CurrentFile = currentFile_;

            try
            {
                using (System.IO.Stream s = new System.IO.FileStream(path,
                    System.IO.FileMode.Create,
                    System.IO.FileAccess.Write))
                {
                    xs_.Serialize(s, playList);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,
                    "Unable to save Play List file:" + path + ",Error:" + ex.Message,
                    "Save Play List Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

        }

        private void MainFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            stopToolStripMenuItem_Click(sender, new EventArgs());
            SaveDefaultPlayList();
        }

        private void lvwPlayList_DoubleClick(object sender, EventArgs e)
        {
            string selectedFile = null;
            foreach (ListViewItem item in lvwPlayList.SelectedItems)
            {
                selectedFile = item.Text;
                break;
            }

            if (selectedFile != null)
            {
                UpdateCurrentPlayingFile(selectedFile);
                playToolStripMenuItem_Click(this, e);
            }
        }

        private void audioDelayDecButton_Click(object sender, EventArgs e)
        {
            if (command_ != null)
            {
                command_.WriteStandardInput(Program.Options.Controls.AudioDelayDec);
            }
        }

        private void audioDelayIncButton_Click(object sender, EventArgs e)
        {
            if (command_ != null)
            {
                command_.WriteStandardInput(Program.Options.Controls.AudioDelayInc);
            }
        }

        private void UpdateCurrentPlayingFile(string playFile)
        {
            currentFile_ = playFile;

            foreach (ListViewItem item in lvwPlayList.Items)
            {
                if (item.Text.Equals(currentFile_))
                {
                    item.Selected = true;
                }
                else
                {
                    item.Selected = false;
                }
            }
        }


        private int GetCurrentPlayingIndex()
        {
            foreach (ListViewItem item in lvwPlayList.Items)
            {
                if (item.Text.Equals(currentFile_))
                {
                    return item.Index;
                }
            }

            return -1;
        }

        private void ClearPlayListMenuItem_Click(object sender, EventArgs e)
        {
            stopToolStripMenuItem_Click(sender, new EventArgs());
            UpdateCurrentPlayingFile(null);
            lvwPlayList.Items.Clear();
        }

        private void backward10MinsButton_Click(object sender, EventArgs e)
        {
            if (command_ != null)
            {
                command_.WriteStandardInput(Program.Options.Controls.Seeking.B10Mins);
            }
        }

        private void backward1MinsButton_Click(object sender, EventArgs e)
        {
            if (command_ != null)
            {
                command_.WriteStandardInput(Program.Options.Controls.Seeking.B1Mins);
            }
        }

        private void backward10SecsButton_Click(object sender, EventArgs e)
        {
            if (command_ != null)
            {
                command_.WriteStandardInput(Program.Options.Controls.Seeking.B10Secs);
            }
        }

        private void forward10SecsButton_Click(object sender, EventArgs e)
        {
            if (command_ != null)
            {
                command_.WriteStandardInput(Program.Options.Controls.Seeking.F10Secs);
            }
        }

        private void forward1MinsButton_Click(object sender, EventArgs e)
        {
            if (command_ != null)
            {
                command_.WriteStandardInput(Program.Options.Controls.Seeking.F1Mins);
            }
        }

        private void forward10MinsButton_Click(object sender, EventArgs e)
        {
            if (command_ != null)
            {
                command_.WriteStandardInput(Program.Options.Controls.Seeking.F10Mins);
            }
        }

        private void loadPlayListMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                stopToolStripMenuItem_Click(sender, new EventArgs());
                LoadPlayList(openFileDialog1.FileName);
            }
        }

        private void savePlayListAsMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                SavePlayList(saveFileDialog1.FileName);
            }
        }

        private void showOutputWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lstOutput.Visible = !lstOutput.Visible;
            showOutputWindowToolStripMenuItem.Checked = lstOutput.Visible;
        }

        private void MainFrm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
        }

        private void MainFrm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                pauseToolStripMenuItem_Click(sender, new EventArgs());
            }
        }
    }
}
