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

namespace MillerRabinTest
{
    public partial class Form1 : Form
    {
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

                c+= 2;

            }

            return c;

        }
        public Form1()
        {
            InitializeComponent();
        }

        private void richTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {

            e.Handled = char.IsLetter(e.KeyChar) || char.IsPunctuation(e.KeyChar);

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        { 
                e.Handled = char.IsLetter(e.KeyChar) || char.IsPunctuation(e.KeyChar);
               
          
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int a;

            try
            {
                a = int.Parse(richTextBox1.Text);

                if (a<2)
                {

                    label1.Text="Числа больше или равны двум";
                    return;
                }

                textBox1.Text = GeneratePrime(a).ToString();
            }
            catch
            {
                label1.Text = "Проверьте данные";
                 
            }


           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            BigInteger a;
            try
            {
                a = BigInteger.Parse(textBox1.Text);

                if (a < 2)
                {

                    label1.Text = "Числа больше или равны двум";
                    return;
                }

                if (IsPrime(a))
                {
                    label1.Text = "Число простое";

                }
                else
                {
                    label1.Text = "Число составное";
                }
            }
            catch
            {
                label1.Text = "Проверьте данные";

            }

        }
    }
}
