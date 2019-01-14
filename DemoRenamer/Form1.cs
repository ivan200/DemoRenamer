using DemoRenamer.DemoParser.huffman;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace DemoRenamer
{
    public partial class Form1 : Form
    {
        FileInfo openDemoFile;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var pName = Properties.Settings.Default.fileName;
            if (pName.Length > 0)
            {
                openDemoFile = new FileInfo(pName);
                if (openDemoFile.Exists)
                {
                    textBox1.Text = pName;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK && openFileDialog1.CheckFileExists)
            {
                textBox1.Text = openFileDialog1.FileName;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
            {
                openDemoFile = new FileInfo(textBox1.Text);
            }

            if (!openDemoFile.Exists)
            {
                MessageBox.Show("File does not exist\n\n" + textBox1.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var prop = Properties.Settings.Default;
            prop.fileName = textBox1.Text;
            prop.Save();

            Q3HuffmanMapper.init();

            var cfg = Q3DemoParser.getFriendlyConfig(openDemoFile.FullName);

            if (cfg == null)
            {
                MessageBox.Show("ERROR");
            }
            else {
                MessageBox.Show(cfg.ToString());
            }
            
        }
    }
}
