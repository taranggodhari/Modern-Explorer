using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Windows;
using System.Diagnostics;

namespace TarangGodhariM17_Lab1_Ex2
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
            PopulateTreeView();
        }

     
       
        private void PopulateTreeView()
        {
            
            TreeNode rootNode;
            //
      

            DirectoryInfo info = new DirectoryInfo(@"D:\Docs");
            //MessageBox.Show(info.Name);
            if (info.Exists)
            {
                //set the above directory as root node
                rootNode = new TreeNode(info.Name);
                rootNode.Tag = info;
                //get the directories under root node
                GetDirectories(info.GetDirectories(), rootNode);
                treeView1.Nodes.Add(rootNode);

            }
           

        }
        private void GetDirectories(DirectoryInfo[] subDirs,
           TreeNode nodeToAddTo)
        {
            try
            {
                TreeNode aNode;
                DirectoryInfo[] subSubDirs;
                foreach (DirectoryInfo subDir in subDirs)
                {

                    aNode = new TreeNode(subDir.Name, 0, 0);
                    aNode.Tag = subDir;
                    aNode.ImageKey = "folder";
                    subSubDirs = subDir.GetDirectories();
                    //call recursively the method to get directories of each node.
                    if (subSubDirs.Length != 0)
                    {
                        GetDirectories(subSubDirs, aNode);
                    }
                    nodeToAddTo.Nodes.Add(aNode);

                }
            }
            // access denied
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Warning: Some files may not be " +
                   "visible due to permission settings",
                   "Attention", 0, MessageBoxIcon.Warning);
            }
            catch (PathTooLongException)
            {
                //MessageBox.Show("Warning: Some files may  have " +
                //   "a long path",
                //   "Attention", 0, MessageBoxIcon.Warning);
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        { 
            TreeNode newSelected = e.Node;
            listView1.Items.Clear();
            DirectoryInfo nodeDirInfo = (DirectoryInfo)newSelected.Tag;
            ListViewItem.ListViewSubItem[] subItems;
            ListViewItem item = null;

            foreach (DirectoryInfo dir in nodeDirInfo.GetDirectories())
            {
                item = new ListViewItem(dir.Name, 0);
                subItems = new ListViewItem.ListViewSubItem[]
                    {new ListViewItem.ListViewSubItem(item, "Directory"),
                     new ListViewItem.ListViewSubItem(item,
                        dir.LastAccessTime.ToShortDateString())};
                item.SubItems.AddRange(subItems);
                listView1.Items.Add(item);

            }
            foreach (FileInfo file in nodeDirInfo.GetFiles())
            {
                item = new ListViewItem(file.Name, 1);
                subItems = new ListViewItem.ListViewSubItem[]
                    { new ListViewItem.ListViewSubItem(item, "File"),
                     new ListViewItem.ListViewSubItem(item,
                        file.LastAccessTime.ToShortDateString()),
                     new ListViewItem.ListViewSubItem(item,file.Extension)};

                item.SubItems.AddRange(subItems);
                listView1.Items.Add(item);

            }

            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode CurrentNode = e.Node;
            string fullpath = CurrentNode.FullPath;
            addressBar.Text = "D:\\" + fullpath;
            addressBar.Items.Add(addressBar.Text);
            
        }
        //Address Bar
        private void addressBar_SelectedIndexChanged(object sender, EventArgs e)
        {
            string currentpath = addressBar.SelectedItem.ToString();
           
        }

        //listview Details,SmallIcon and List Views
        private void toolStripButtonDetails_Click(object sender, EventArgs e)
        {
            listView1.View = View.Details;
        }

        private void toolStripButtonList_Click(object sender, EventArgs e)
        {
            listView1.View = View.List;
        }

        private void toolStripButtonSmallIcon_Click(object sender, EventArgs e)
        {
            listView1.View = View.SmallIcon;

        }
		
		//Key Down Events
 private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F2 && listView1.SelectedItems.Count > 0)
            {
                listView1.SelectedItems[0].BeginEdit();
            }
            if (e.KeyData == Keys.Delete && listView1.SelectedItems.Count > 0)
            {
                if(MessageBox.Show("Are you sure you want to delete this file?","Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes);
                { if (listView1.SelectedItems.Count == 0)
                        return;
                    int index = listView1.SelectedIndices[0];
                    string strItem = listView1.Items[index].Text;
                    string fullpath = addressBar.Text + "\\" + strItem;
                    System.IO.File.Delete(fullpath);
                }
            }
        }

        //
        //Logic For Renaming Starts Here
        //
        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
                 listView1.SelectedItems[0].BeginEdit();
        }
       
      
        private void listView1_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            //MessageBox.Show(,e.Label);
            //  get full path of directory or file
            try
            {

                listView1.BeginUpdate();
                if (listView1.SelectedItems.Count == 0)
                    return;
                int index = listView1.SelectedIndices[0];
                string strItem = listView1.Items[index].Text;
                string oldFullPath = addressBar.Text + "\\" + strItem;
                string newFullPath = addressBar.Text + "\\" + e.Label;
                //MessageBox.Show(oldFullPath);
                if (System.IO.Directory.Exists(oldFullPath))
                {
                    System.IO.Directory.Move(oldFullPath, newFullPath);
                }
                else
                {
                    System.IO.File.Move(oldFullPath, newFullPath);
                }

            }
            catch (IOException ex)
            {
                //MessageBox.Show("");
            }
            finally
            {
                listView1.EndUpdate();
            }
        }

        //Rename Logic ends here

        //
        //Copy Logic Starts Here
        //
        private void toolStripButtonCopy_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems.Count == 0)
                    return;
                int index = listView1.SelectedIndices[0];
                string strItem = listView1.Items[index].Text;
                Clipboard.Clear();
                Clipboard.SetDataObject(addressBar.Text + "\\" + strItem, false, 5, 200);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //Copy Logic Ends Here

        //
        //Paste Logic Starts Here
        //
        private void toolStripButtonPaste_Click(object sender, EventArgs e)
        {
           try
            {
               
                string oldpath = (String)Clipboard.GetData(DataFormats.Text);
                string filename = Path.GetFileName(oldpath);
                string newpath = addressBar.Text + "\\" + filename;
               // MessageBox.Show(oldpath);
                System.IO.File.Copy(oldpath, newpath, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }
        //Paste Logic Ends Here

            //
            //Delete Logic Starts Here
            //

        private void toolStripButtonDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this file?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) ;
            {
                if (listView1.SelectedItems.Count == 0)
                    return;
                int index = listView1.SelectedIndices[0];
                string strItem = listView1.Items[index].Text;
                string fullpath = addressBar.Text + "\\" + strItem;
                System.IO.File.Delete(fullpath);
            }
        }
       
        //Delete Logic Ends Here

        //
        //Method to Copy Directory
        //
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }


        //Status Bar Item Selected Test Change
        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            int itemsInList = listView1.SelectedItems.Count;
            
            itemSelectedLable.Text = "Item Selected " + itemsInList.ToString();
          
        }


        //About Button Click
        private void toolStripButtonAbout_Click(object sender, EventArgs e)
        {
            Form f2 = new Form();
            f2.Text = "About";
            f2.Size = new Size(250, 150);
            f2.StartPosition = FormStartPosition.CenterScreen;
            f2.Icon = Icon.FromHandle(((Bitmap)imageList1.Images[2]).GetHicon());
            Label aboutLabel = new Label();
            aboutLabel.Size = new Size(200, 200);
            aboutLabel.Font = new Font("Verdana", 10, FontStyle.Bold);
            aboutLabel.Text = "Application Developer: Tarang Godhari \n\nIcons By: ";
            LinkLabel linklabel = new System.Windows.Forms.LinkLabel();

            linklabel.Location = new Point(0, 70);
            linklabel.Text = "Icons8";
            linklabel.Font = new Font("Verdana", 10, FontStyle.Bold);
            linklabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);

            f2.Controls.Add(linklabel);
            f2.Controls.Add(aboutLabel);
            f2.Show();
        }

        //Link Lable Click event
        private void linkLabel_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {

            System.Diagnostics.Process.Start("https://icons8.com/icon/13754/Message");
        }

     
    }
}
