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
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid Files Selected!");
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            int lines = 0;
            foreach (string file in (string[])e.Argument)
            {
                if (backgroundWorker1.CancellationPending)
                    break;
                lines += File.ReadLines(file).Count();
                backgroundWorker1.ReportProgress(lines);
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value++;
            label1.Text = "Number of lines: " + e.ProgressPercentage;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton ctrl = (RadioButton)sender;
            if (ctrl.Text == "C#")
                fileType = "cs";
            else if (ctrl.Text == "Java")
                fileType = "java";
            else if (ctrl.Text == "All")
                fileType = "*";
            else if (ctrl.Text == "Text")
                fileType = "txt";
            else if (ctrl.Text == "Other")
                fileType = textBox2.Text;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            button3.Enabled = false;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            fileType = textBox2.Text;
            radioButton5.Checked = true;
        }
    }
}
