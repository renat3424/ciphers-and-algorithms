using SimpleTCP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;
namespace WindowsFormsApp1
{

    public partial class Form1 : Form
    {

        BigInteger OddNum(int r)
        {
            BigInteger num = BigInteger.Pow(2, r - 1) + 1;
            Random rnum = new Random(Guid.NewGuid().GetHashCode());
            for (int i = r - 2; i > 0; i--)
            {

                num += BigInteger.Pow((rnum.Next() % 2) * 2, i);

            }

            return BigInteger.Abs(num);

        }

        BigInteger GeneratePrime(int length)
        {

            BigInteger c = OddNum(length);
            while (!solovoyStrassen(c, 100))
            {

                c += 2;

            }

            return c;

        }
        public int Jacobi1(BigInteger a, BigInteger n)
        {


            if (BigInteger.GreatestCommonDivisor(a, n) != 1)
            {

                return 0;
            }

            int r = 1;



            while (a != 0)
            {


                int t = 0;

                while (a % 2 == 0)
                {



                    a = a / 2;
                    t = t + 1;

                }

                if (t % 2 != 0)
                {


                    if (n % 8 == 3 || n % 8 == 5)
                    {

                        r = -1 * r;
                    }
                }


                if (a % 4 == 3 && n % 4 == 3)
                {

                    r = -1 * r;


                }

                BigInteger c = a;
                a = n % c;
                n = c;
            }
            return r;



        }






        int bitLength(BigInteger a)
        {
            int BitLength = 0;
            while (a != 0)
            {
                BitLength++;
                a /= 2;
            }

            return BitLength;

        }
        BigInteger ModPow(BigInteger a, BigInteger e, BigInteger N)
        {

            BigInteger b = 1;
            int r = bitLength(e);
            BigInteger res = 1;
            for (int i = r - 1; i >= 0; i--)
            {


                res = (BigInteger.Pow(res, 2) * BigInteger.Pow(a, (int)(((b << i) & e) >> i))) % N;


            }

            return res;

        }

        bool solovoyStrassen(BigInteger p, BigInteger iteration)
        {
            if (p < 2)
                return false;
            if (p != 2 && p % 2 == 0)
                return false;


            for (BigInteger i = 0; i < iteration; i++)
            {


                BigInteger a = (2 + i) % (p - 1) + 1;
                BigInteger jacobian = ((p + Jacobi1(a, p)) % p);
                BigInteger mod = ModPow(a, (p - 1) / 2, p);

                if (jacobian == 0 || mod != jacobian)
                {
                    return false;
                }
            }
            return true;
        }

        bool TrueFalse(BigInteger p, BigInteger g)
        {

            for (BigInteger k = 2; k < p - 1; k++)
            {
                if (ModPow(g, k, p) == 1)
                {
                    return false;
                }

            }
            return true;

        }
        BigInteger PrimeGen(BigInteger p, int len)
        {

            for (BigInteger g = OddNum(len); g < p - 1; g++)
            {


                if (TrueFalse(p, g))
                {
                    return g;
                }
            }
            return 0;

        }

        static byte[] SBlock(BigInteger Key)
        {

            byte[] S = new byte[256];
            byte[] T = new byte[256];
            byte[] keyArr = Key.ToByteArray();
            for (int i = 0; i < 256; i++)
            {

                S[i] = (byte)i;
                T[i] = keyArr[i % keyArr.Length];
            }
            int j = 0;
            byte var;
            for (int i = 0; i < 256; i++)
            {

                j = (j + S[i] + T[i]) % 256;
                var = S[i];
                S[i] = S[j];
                S[j] = var;
            }
            return S;

        }
        static byte[] Xor(byte[] a, byte[] b)
        {

            byte[] result = new byte[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                result[i] = (byte)(a[i] ^ b[i]);
            }

            return result;



        }
        static byte[] GetKey(string text, byte[] S)
        {

            int i = 0;
            int j = 0;
            int len = System.Text.Encoding.Default.GetBytes(text).Length;
            byte[] key = new byte[len];
            byte var;
            while (len > 0)
            {
                i = (i + 1) % 256;
                j = (j + S[i]) % 256;
                var = S[i];
                S[i] = S[j];
                S[j] = var;
                key[key.Length - len] = S[(S[i] + S[j]) % 256];

                len = len - 1;
            }

            return key;

        }
        public Form1()
        {
            InitializeComponent();
        }
        SimpleTcpClient client;
        string txtHost = "127.0.0.1";
        string txtPort = "8910";
        BigInteger B;
        BigInteger p;
        BigInteger g;
        BigInteger b;
        BigInteger A;
      

        private void Client_DataReceived(object sender, SimpleTCP.Message e)
        {
            string msg = e.MessageString.Remove(e.MessageString.Length - 1);
           
        }
        private void connect_Click(object sender, EventArgs e)
        {
            connect.Enabled = false;
            IPAddress ip = IPAddress.Parse(txtHost);
            client.Connect(txtHost, Convert.ToInt32(txtPort));
            textBox1.Text+= "Ready!\r\n";
            var reply = client.WriteLineAndGetReply("key", TimeSpan.FromSeconds(5));
            if (reply != null)
            {
                string[] Agp = reply.MessageString.Split(' ');
                A = BigInteger.Parse(Agp[0]);
                g = BigInteger.Parse(Agp[1]);
                p = BigInteger.Parse(Agp[2].Split('\u0013')[0]);
                textBox1.Text += "A=" + A + " \r\n";
                textBox1.Text += "p=" + p + " \r\n";
                textBox1.Text += "g=" + g + " \r\n";
                

                b = GeneratePrime(64);
                textBox1.Text += "b=" + b + " \r\n";

                B = ModPow(g, b, p);
                textBox1.Text += "B=" + B + " \r\n";
                client.WriteLine("B=" + B.ToString());
                textBox1.Text += "K=" + ModPow(A, b, p) + " \r\n";

            }





        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            client = new SimpleTcpClient();
            client.StringEncoder = Encoding.UTF8;
            client.DataReceived += Client_DataReceived;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
