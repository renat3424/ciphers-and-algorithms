using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private ToolStripMenuItem temp;
        private ToolStripMenuItem temp1;
        Cypher cypher;
        
        public Form1()
        {
            InitializeComponent();
            russianToolStripMenuItem.Click += new System.EventHandler(ToolStripMenuItem_Click);

            englishToolStripMenuItem.Click += new System.EventHandler(ToolStripMenuItem_Click);
            VigenereToolStripMenuItem.Click += new System.EventHandler(CipherToolStripMenuItem_Click);

            ceasarToolStripMenuItem.Click += new System.EventHandler(CipherToolStripMenuItem_Click);
            cypher = new Ceasar();
            temp = englishToolStripMenuItem;
            temp1 = ceasarToolStripMenuItem;


        }

        private void CipherToolStripMenuItem_Click(object sender, EventArgs e)

        {



            temp1.CheckState = CheckState.Unchecked;

            temp1 = (ToolStripMenuItem)sender;



            temp1.CheckState = CheckState.Checked;
            if (temp == ceasarToolStripMenuItem)
            {

                cypher=new Ceasar();

            }
            else
            {

                cypher=new Vigenere();
            }
            richTextBox1.Text = "";
            richTextBox2.Text = "";
            richTextBox3.Text = "";
            textBox1.Text = "";

        }
        private void ToolStripMenuItem_Click(object sender, EventArgs e)

        {

           

            temp.CheckState = CheckState.Unchecked;

            temp = (ToolStripMenuItem)sender;

            

            temp.CheckState = CheckState.Checked;
            if (temp == englishToolStripMenuItem)
            {

                cypher.ChangeLang("EN");

            }
            else
            {

                cypher.ChangeLang("RU");
            }

            richTextBox2.Text = "";

        }
        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void languageToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string num=textBox1.Text;
            string text;
            try
            {
                
                if (richTextBox1.Text == "")
                {
                    throw new Exception();
                }

                text = richTextBox1.Text;
            }
            catch
            {
                MessageBox.Show("nothing is written!");
                return;

            }
            try
            {

                cypher.CheckKey(num);
                
            }
            catch(Exception v)
            {
                MessageBox.Show(v.Message);
                return;
            }
            try
            {
                
                if (temp == englishToolStripMenuItem)
                {
                    Regex regex = new Regex("^[a-zA-Z0-9. -_?]*$");

                    if (!regex.IsMatch(text))
                    {
                        throw new Exception();

                    }

                }
                else
                {
                    Regex regex = new Regex("^[а-яА-Я0-9. -_?]*$");
                    if (!regex.IsMatch(text))
                    {
                        throw new Exception();

                    }
                }
            }
            catch
            {

                MessageBox.Show("Your writing doesn't match language you chose");
                return;

            }

            richTextBox2.Text = "";

            richTextBox2.Text = cypher.Encode(text, num);



            

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string num = textBox1.Text;
            string text;
            try
            {

                if (richTextBox2.Text == "")
                {
                    throw new Exception();
                }

                text = richTextBox2.Text;
            }
            catch
            {
                MessageBox.Show("nothing was encoded yet!");
                return;

            }


            try
            {
                cypher.CheckKey(num);

            }
            catch(Exception v)
            {
                MessageBox.Show(v.Message);
                return;
            }

            richTextBox3.Text = "";
            richTextBox3.Text = cypher.Decode(text, num);


        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

        }

        private void вырезатьToolStripButton_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
        }

        private void открытьToolStripButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openfile = new OpenFileDialog();
            openfile.Title = "My open file dialog";
            if (openfile.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.Clear();
                using (StreamReader sr = new StreamReader(openfile.FileName))
                {
                    richTextBox1.Text = sr.ReadToEnd();
                    sr.Close();
                }
            }
        }

        private void algorithmToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
