using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;
using System.Text.RegularExpressions;

namespace WindowsFormsApp4
{
    public partial class Form1 : Form
    {

        string RemovePunctuation(string Text)
        {

            string str = "";

            for (int i = 0; i < Text.Length; i++)
            {


                if (!char.IsPunctuation(Text[i]))
                {


                    str = str + Text[i];
                }
            }
            return str;
        }
        string alphabet;
       string RU = "0123456789абвгдеёжзийклмнопрстуфхцчшщъыьэюя ";
       string EN = "0123456789abcdefghijklmnopqrstuvwxyz ";
        BigInteger p, q, n, fn, e, d;
        private void button1_Click(object sender, EventArgs g)
        {
            int t;

           
            try
            {
                t = int.Parse(textBox4.Text);
                if (t < 4)
                {

                    throw new Exception("number of bits should be more than 4");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            p = GeneratePrime(t);
            q = GeneratePrime(t);
            while (p == q)
            {
                p = GeneratePrime(t);
                q = GeneratePrime(t);

            }
            fn = (p - 1) * (q - 1);

            n = p * q;
            int r;
            while (true)
            {
                r = ((2 * t / 3 < 2) ? 2 : 2 * t / 3);
                e = GeneratePrime(r);
                
                if (fn % e != 0)
                {

                    break;
                }

                if (r == 2)
                {
                    p = GeneratePrime(t);
                    q = GeneratePrime(t);
                    while (p == q)
                    {
                        p = GeneratePrime(t);
                        q = GeneratePrime(t);

                    }
                    fn = (p - 1) * (q - 1);

                    n = p * q;
                }
            }
            
            BigInteger x1;
            BigInteger x2;
            EuclidExtended(fn, e, out x1, out d, out x2);
            if (d < 0)
            {

                d = d + fn;
            }
            string Msg = textBox1.Text;
            Msg = Msg.ToLower();
            Msg = RemovePunctuation(Msg);
            try
            {
                if (Msg== "")
                {


                    throw new Exception("Nothing is written");
                }
                
                
                if (alphabet == EN)
                {
                    Regex regex = new Regex("^[a-z0-9 \0]*$");

                    if (!regex.IsMatch(Msg))
                    {
                        throw new Exception("Text has to be written in english");

                    }

                }
                else
                {
                    Regex regex = new Regex("^[а-я0-9 \0]*$");
                    if (!regex.IsMatch(Msg))
                    {
                        throw new Exception("Text has to be written in russian");

                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
           
           
            
            string res = "";
            for (int i = 0; i < Msg.Length; i++)
            {

                res +=ModPow(alphabet.IndexOf(Msg[i]), e, n) + " ";


            }
            textBox2.Text = res;
            textBox5.Text = p.ToString();
            textBox6.Text = q.ToString();
            textBox7.Text = n.ToString();
            textBox8.Text = fn.ToString();
            textBox9.Text = e.ToString();
            textBox10.Text = d.ToString();


        }
        private void button2_Click(object sender, EventArgs g)
        {
            if (textBox2.Text == "")
            {

                MessageBox.Show("Nothing was encrypted yet");

                
                return;
            }


           
            string res = textBox2.Text;
           
            string[] res1 = res.Split(' ');
            res = "";
            d = BigInteger.Parse(textBox10.Text);
           n = BigInteger.Parse(textBox7.Text);
            string x;
            BigInteger num;
            for (int i = 0; i < res1.Length; i++)
            {
                if (res1[i] != "")
                {   
                    x= res1[i].Replace(" ", string.Empty);

                    try
                    {
                        num=BigInteger.Parse(x);
                        if (num < 0)
                        {

                            MessageBox.Show("No negative numbers");
                        }
                   


                    num = ModPow(num, d, n);
                        if (num>=alphabet.Length)
                        {
                            num = num % alphabet.Length;
                        }
                    res += alphabet[(int)num];


                }
                    catch
                {
                    MessageBox.Show("No symbols, numbers only");
                    return;

                }
            }

            }

            textBox3.Text = res;
        }



        public Form1()
        {
            InitializeComponent();
            textBox1.ScrollBars = ScrollBars.Vertical;
            textBox2.ScrollBars = ScrollBars.Vertical;
            textBox3.ScrollBars = ScrollBars.Vertical;
            alphabet = EN;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs g)
        {
            alphabet =EN;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs g)
        {
            alphabet = RU;
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


      void EuclidExtended(BigInteger A, BigInteger B, out BigInteger x, out BigInteger y, out BigInteger C)
        {
            if (B == 0)
            {

                C = A;
                x = 1;
                y = 0;
                return;
            }

            EuclidExtended(B, (A % B), out x, out y, out C);


            BigInteger x1 = y;


            y = x - (A / B) * y;

            x = x1;





        }




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

      bool CheckComposite(BigInteger r, BigInteger a, BigInteger t, BigInteger s)
        {

            a = ModPow(a, t, r);
            if (a == 1 || a == r - 1)
            {
                return false;

            }


            for (int i = 1; i < s; i++)
            {

                a = ModPow(a, 2, r);
                if (a == r - 1)
                {
                    return false;
                }

            }


            return true;

        }

      bool IsPrime(BigInteger r)
        {


            if (r < 4)
            {

                return (r == 2 || r == 3);
            }

            if (r % 2 == 0)
            {

                return false;
            }
            double k = BigInteger.Log(r, 2);
            BigInteger t = r - 1;
            BigInteger s = 0;

            while ((t & 1) == 0)
            {

                t = t >> 1;
                s++;

            }


            BigInteger a;



            for (int i = 0; i < k; i++)
            {


                a = getRandom(bitLength(r - 1), 2, r - 1);

                if (r % a == 0)
                {

                    return false;
                }

                if (CheckComposite(r, a, t, s))
                {

                    return false;
                }
            }

            return true;


        }


       BigInteger getRandom(int length, BigInteger a, BigInteger b)
        {
            if (length < 8)
            {
                length = 8;
            }
            length = length / 8;
            Random random = new Random(Guid.NewGuid().GetHashCode());
            byte[] data = new byte[length];
            random.NextBytes(data);
            return a + BigInteger.Abs(new BigInteger(data)) % (b - a);
        }
       BigInteger GeneratePrime(int length)
        {

            BigInteger c = OddNum(length);
            while (!IsPrime(c))
            {

                c += 2;

            }

            return c;

        }



    }
}
