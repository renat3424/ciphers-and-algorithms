using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }
        BigInteger e;
        BigInteger n;
        string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
        private void button1_Click(object sender, EventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        fileContent = reader.ReadToEnd();
                    }
                }
            }


            textBox1.Text = fileContent;
            textBox2.Text = Encrypt(fileContent);
            
        }
        string Encrypt(string msg)
        {
            msg = GetHash(msg);
            string res = "";
            for (int i = 0; i < msg.Length; i++)
            {

                res += ModPow(alphabet.IndexOf(msg[i]), e, n) + " ";


            }
            return res;
        }
        int bitLength(BigInteger a)//подсчет количества бит числа
        {
            int BitLength = 0;
            while (a != 0)
            {
                BitLength++;
                a /= 2;
            }

            return BitLength;

        }
        BigInteger ModPow(BigInteger a, BigInteger e, BigInteger N)//быстрое возведение в степень
        {



            BigInteger num = 1;
            BigInteger res = a;
            for (int i = bitLength(e) - 2; i >= 0; i--)
            {



                res = (((e & (num << i)) > 0) ? (BigInteger.Pow(res, 2) * a) % N : BigInteger.Pow(res, 2) % N);

            }


            return res;




        }
        private string GetHash(string input)
        {
            var val = MD5.Create();
            var hash = val.ComputeHash(Encoding.UTF8.GetBytes(input));

            
            return Convert.ToBase64String(hash);

        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        IS proxy;
        private void Form1_Load(object sender, EventArgs ex)
        {
            var uri = new Uri("net.tcp://localhost:6565/S");

            var binding = new NetTcpBinding(SecurityMode.None);
            var channel = new ChannelFactory<IS>(binding);
            var endpoint = new EndpointAddress(uri);
            proxy = channel.CreateChannel(endpoint);
            e = proxy.GetE();
            n = proxy.GetN();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(proxy.CheckKey(textBox1.Text, textBox2.Text))
            {

                MessageBox.Show("signature is valid");
            }
            else
            {
                MessageBox.Show("signature is not valid");
            }
        }
    }
}
