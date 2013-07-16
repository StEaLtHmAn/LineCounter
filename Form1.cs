using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace LineCounter
{
    public partial class Form1 : Form
    {
        string fileType = "*";
        string SingleComment = "";
        string MultiComment = "";
        string EndMultiComment = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = textBox1.Text;
            dialog.ShowNewFolderButton = true;
            if (dialog.ShowDialog() == DialogResult.OK)
                textBox1.Text = dialog.SelectedPath;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                backgroundWorker1.CancelAsync();
                string[] files = Directory.GetFiles(textBox1.Text, "*." + fileType, SearchOption.AllDirectories);
                progressBar1.Value = 0;
                progressBar1.Maximum = files.Length;
                backgroundWorker1.RunWorkerAsync(files);
                button3.Enabled = true;
                panel2.Enabled = false;
                button1.Enabled = false;
                button2.Enabled = false;
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid Files Selected!");
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            int lines = 0;
            int BlankLines = 0;
            int Comments = 0;
            string line;
            bool comment = false;
            foreach (string file in (string[])e.Argument)
            {
                if (backgroundWorker1.CancellationPending)
                    break;
                StreamReader stream = new StreamReader(file);
                while ((line = stream.ReadLine()) != null)
                {
                    lines++;
                    if (line.Trim() == "")
                        BlankLines++;
                    //start multi-comment
                    if (MultiComment.Length != 0 && line.Contains(MultiComment))
                        comment = true;
                    //add comment
                    if (comment)
                        Comments++;
                    else if (SingleComment.Length!=0 && line.Trim().StartsWith(SingleComment))
                        Comments++;
                    //stop multi-comment
                    if (EndMultiComment.Length != 0 && line.Contains(EndMultiComment) && comment)
                        comment = false;
                }
                stream.Close();
                backgroundWorker1.ReportProgress(0, new int[] { lines, BlankLines, Comments, ((string[])e.Argument).Length });
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value++;
            label9.Text = "Files: " + ((int[])e.UserState)[3];
            label1.Text = "Total lines: " + ((int[])e.UserState)[0];
            label4.Text = "Blank lines: " + ((int[])e.UserState)[1];
            label5.Text = "Commented lines: " + ((int[])e.UserState)[2];
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            panel1.Enabled = false;
            RadioButton ctrl = (RadioButton)sender;
            if (ctrl.Text == "C#")
            {
                fileType = "cs";
                SingleComment = "//";
                MultiComment = "/*";
                EndMultiComment = "*/";
            }
            else if (ctrl.Text == "Java")
            {
                fileType = "java";
                SingleComment = "//";
                MultiComment = "/*";
                EndMultiComment = "*/";
            }
            else if (ctrl.Text == "All")
            {
                fileType = "*";
                SingleComment = "";
                MultiComment = "";
                EndMultiComment = "";
            }
            else if (ctrl.Text == "Text")
            {
                fileType = "txt";
                SingleComment = "";
                MultiComment = "";
                EndMultiComment = "";
            }
            else if (ctrl.Text == "HTML")
            {
                fileType = "html";
                SingleComment = "";
                MultiComment = "<!--";
                EndMultiComment = "-->";
            }
            else if (ctrl.Text == "XML")
            {
                fileType = "xml";
                SingleComment = "";
                MultiComment = "<!--";
                EndMultiComment = "-->";
            }
            else if (ctrl.Text == "Other:")
            {
                panel1.Enabled = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            button3.Enabled = false;
            panel2.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = true;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            fileType = textBox2.Text;
            SingleComment = textBox3.Text;
            MultiComment = textBox4.Text;
            EndMultiComment = textBox5.Text;
        }
    }
}
