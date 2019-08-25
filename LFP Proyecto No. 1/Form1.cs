using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LFP_Proyecto_No._1
{
    public partial class Form1 : Form
    {
        int tabContador = 2;
        public Form1()
        {
            InitializeComponent();
        }

        private void NuevaPestañaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.tabContador++;
            var tabPage = new TabPage("Pestaña " + tabContador);
            tabControl1.Controls.Add(tabPage);
            var richTextBox = new RichTextBox();
            richTextBox.Width = 530;
            richTextBox.Height = 570;
            tabPage.Controls.Add(richTextBox);
        }

        private void AbrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var filePath = string.Empty;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "org files (*.org)|*.org";

            Console.WriteLine(openFileDialog);
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
            }

            if (File.Exists(filePath))
            {
                StreamReader streamReader = new StreamReader(filePath);
                string line;
                foreach (Control c in tabControl1.SelectedTab.Controls)
                {
                    RichTextBox richTextBox = c as RichTextBox;
                    try
                    {
                        line = streamReader.ReadLine();
                        while (line != null)
                        {
                            richTextBox.AppendText(line + "\n");
                            line = streamReader.ReadLine();
                        }
                    }
                    catch (Exception)
                    {
                        alertMessage("Ha ocurrido un error.");
                    }
                }
            }
        }

        public void alertMessage(String mensaje)
        {
            MessageBox.Show(mensaje, "Error",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
