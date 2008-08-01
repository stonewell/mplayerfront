using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace MPlayerFront
{
    public partial class OpenFileFrm : Form
    {
        private Regex rx = new Regex(@"^\d{2}:\d{2}"); 

        private string currentDirectory_ = null;

        private List<string> selectedFiles_ = new List<string>();

        public OpenFileFrm()
        {
            InitializeComponent();
        }

        private void OpenFileFrm_Load(object sender, EventArgs e)
        {
            string pwdResult =
                MFHelper.NormalizeString(
                    MFCommand.ExecuteCommand(
                        Program.Options.Commands.CurrentDirectory, ""));

            selectedFiles_.Clear();

            RefreshContent(pwdResult);
        }

        private void ParseContent(string content)
        {
            treeView1.Nodes.Clear();
            listView1.Items.Clear();

            treeView1.Nodes.Add("..");

            using (System.IO.StringReader sr = new System.IO.StringReader(content))
            {
                string line = sr.ReadLine();

                while (line != null)
                {
                    ParseLine(line);

                    line = sr.ReadLine();
                }
            }

            listView1.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);

            if (listView1.Columns[0].Width < listView1.Width - 10)
            {
                listView1.Columns[0].Width = listView1.Width - 10;
            }
        }

        private void ParseLine(string line)
        {
            string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.None);

            if (parts.Length > 7)
            {
                bool isDirectory = parts[0][0] == 'd';
                bool isLink = parts[0][0] == 'l';

                StringBuilder sb = new StringBuilder();
                bool collect = false;

                for (int i = 0; i < parts.Length; i++)
                {
                    if (!collect && rx.IsMatch(parts[i]))
                    {
                        collect = true;
                    }
                    else if (collect)
                    {
                        if (sb.Length > 0)
                            sb.Append(" ");

                        sb.Append(parts[i]);
                    }
                }

                string fileName = sb.ToString();

                if (isDirectory)
                {
                    AddDirectory(fileName);
                }
                else if (isLink)
                {
                    string[] a =
                        fileName.Split(new string[] { "->" }, 
                            StringSplitOptions.RemoveEmptyEntries);

                    if (IsDirectory(a[1].Trim()))
                    {
                        AddDirectory(a[0].Trim());
                    }
                    else
                    {
                        AddFile(a[0].Trim());
                    }
                }
                else
                {
                    AddFile(fileName);
                }
            }
        }

        private bool IsDirectory(string p)
        {
            string fileName = p;

            if (!fileName.StartsWith("/"))
            {
                fileName = currentDirectory_ + "/" + fileName;
            }

            string content =
                MFCommand.ExecuteCommand(Program.Options.Commands.StatFiletype,
                    fileName);

            return content.Equals("directory");
        }

        private void AddDirectory(string fileName)
        {
            treeView1.Nodes.Add(fileName);
        }

        private void AddFile(string fileName)
        {
            listView1.Items.Add(fileName);
        }

        private void UpdateCurrentDirectory()
        {
            currentDirectory_ = RemoveTrailSlash(currentDirectory_);

            if (!cboCurrentDirectory.Items.Contains(currentDirectory_))
                cboCurrentDirectory.Items.Add(currentDirectory_);

            cboCurrentDirectory.Text = currentDirectory_;
        }

        private static string RemoveTrailSlash(string currentDirectory)
        {
            while (currentDirectory.Length > 1 && currentDirectory[currentDirectory.Length - 1] == '/')
            {
                currentDirectory = 
                    currentDirectory.Remove(currentDirectory.Length - 1);
            }

            return currentDirectory;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            btnAddAllSelected_Click(sender, e);

            DialogResult = DialogResult.OK;

            Close();
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            btnOK_Click(this, e);
        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                string directoryName = currentDirectory_;

                if (treeView1.SelectedNode.Text.Equals(".."))
                {
                    string[] parts = currentDirectory_.Split('/');

                    if (parts.Length > 1)
                    {
                        StringBuilder sb = new StringBuilder();

                        for (int i = 0; i < parts.Length - 1; i++)
                        {
                            sb.Append(parts[i]);
                            sb.Append("/");
                        }

                        directoryName = sb.ToString();
                    }
                }
                else
                {
                    directoryName = 
                        currentDirectory_ + "/" + treeView1.SelectedNode.Text;
                }

                RefreshContent(directoryName);
            }
        }

        public List<string> SelectedFiles
        {
            get
            {
                return selectedFiles_;
            }
        }

        private void cboCurrentDirectory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!cboCurrentDirectory.SelectedItem.Equals(currentDirectory_))
            {
                RefreshContent(cboCurrentDirectory.SelectedItem as string);
            }
        }

        private void RefreshContent(string directory)
        {
            Cursor oldCursor = this.Cursor;
            
            this.Cursor = Cursors.WaitCursor;

            currentDirectory_ = directory;
            UpdateCurrentDirectory();

            string content =
                MFCommand.ExecuteCommand(Program.Options.Commands.ListFiles,
                    currentDirectory_);

            ParseContent(content);

            this.Cursor = oldCursor;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshContent(cboCurrentDirectory.SelectedItem as string);
        }

        private void btnAddAllSelected_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;

            foreach (ListViewItem item in listView1.SelectedItems)
            {
                string path = 
                    currentDirectory_ + "/" + item.Text;

                if (!selectedFiles_.Contains(path))
                    selectedFiles_.Add(path);
            }
        }

        private void cboCurrentDirectory_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                RefreshContent(cboCurrentDirectory.Text as string);
            }
        }

        private void OpenFileFrm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (cboCurrentDirectory.Focused)
                {
                    return;
                }
                else
                {
                    btnOK_Click(sender, new EventArgs());
                }
            }
        }
    }
}