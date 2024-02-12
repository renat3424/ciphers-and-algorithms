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

namespace WindowsFormsApp5
{


    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            iter = 0;
        }
        BigInteger iter;
        string alph = "АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдежзийклмнопрстуфхцчшщъыьэюя";
  
       string ConvertFromWin(BigInteger n)
        {
            string str = "";
            while (n != 0)
            {
                try
                {
                    str = alph[(int)(n % 100 - 16)] + str;
                }
                catch {
                    str = " " + str;
                }
                n =n/100;
                 
            }
            return str;

        }
        public  BigInteger getRandom(int length, BigInteger a, BigInteger b)
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
        public  BigInteger Pollard(BigInteger x, BigInteger N)
        {

           
            BigInteger  y = 1, i = 0, p = 2, res = BigInteger.GreatestCommonDivisor(N, BigInteger.Abs(x - y));

            while (res == 1)
            {

                if (i == p)
                {
                    y = x;
                    p = p * 2;
                }

                x = (x * x + 1) % N;
                i++;
                res = BigInteger.GreatestCommonDivisor(N, BigInteger.Abs(x - y));

            }
            iter = i;
            return res;
        }

        public BigInteger PollardAlgorithm(BigInteger N)
        {







            Task<BigInteger>[] tasks = new Task<BigInteger>[2];




            tasks[0] = Task.Run(() => Pollard((N - 2) / 4, N));
            //tasks[1] = Task.Run(() => Pollard(N, (N-2)/4));
            tasks[1] = Task.Run(() => Pollard(3 * (N - 2) / 4, N));

            
           
            int ind = Task.WaitAny(tasks);




            return tasks[ind].Result;
        }

        HashSet<BigInteger> GenerateSet(BigInteger st, BigInteger end)
        {


            HashSet<BigInteger> list = new HashSet<BigInteger>();

            for (BigInteger i = st; i <= end; i++)
            {

                list.Add(i);

            }

            return list;

        }

        static bool CheckComposite(BigInteger r, BigInteger a, BigInteger t, BigInteger s)
        {

            a = BigInteger.ModPow(a, t, r);
            if (a == 1 || a == r - 1)
            {
                return false;

            }


            for (int i = 1; i < s; i++)
            {

                a = BigInteger.ModPow(a, 2, r);
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


        HashSet<BigInteger> PrimeNums(BigInteger st, BigInteger end)
        {



            HashSet<BigInteger> list = GenerateSet(st, end);


            for (BigInteger i = 2; BigInteger.Pow(i, 2) <= end; i++)
            {



                if (IsPrime(i))
                {

                    for (BigInteger j = BigInteger.Pow(i, 2); j <= end; j = j + i)
                    {

                        list.Remove(j);

                    }
                }
            }



            return list;





        }

        public BigInteger Factor1(BigInteger N, BigInteger B1)
        {

            if (N == 1)
            {
                return N;
            }
            else if (N % 2 == 0)
            {

                return 2;

            }

            HashSet<BigInteger> s = PrimeNums(2, B1);
            BigInteger[] primes = new BigInteger[s.Count];
            s.CopyTo(primes);

            BigInteger c = 2, pp = 0;
            BigInteger res = 1;
            foreach (BigInteger p in primes)
            {
                pp = p;
                while (pp <= B1)
                {
                    c = BigInteger.ModPow(c, p, N);
                    pp = pp * p;

                }



            }




            BigInteger g = BigInteger.GreatestCommonDivisor(c - 1, N);

            if (g != 1 && g != N)
            {
                return g;
            }
            s = PrimeNums(B1 + 1, BigInteger.Pow(B1, 2));
            primes = new BigInteger[s.Count];
            s.CopyTo(primes);

            BigInteger temp = c, d = 0;




            for (int i = 0; i < primes.Length; i++)
            {
                c = BigInteger.ModPow(c, primes[i], N);
                g = BigInteger.GreatestCommonDivisor(c - 1, N);

                if (g != 1 && g != N)
                {

                    return g;
                }

            }




            return g;
        }
        ///r:System.Numerics.dll p.cs

        public BigInteger Factor(BigInteger N)
        {



            BigInteger B1 = 2;
            BigInteger f = Factor1(N, B1);
            while (f == 1 || f == N)
            {



                B1 = B1 + 1;
                f = Factor1(N, B1);
            }


            return f;

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
        
        private void textBox3_TextChanged(object sender, EventArgs e)
        {

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

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                BigInteger r = BigInteger.Parse(textBox1.Text);
                BigInteger p, q, d;
                BigInteger msg;

                string str;
                var watch = System.Diagnostics.Stopwatch.StartNew();

                p = Factor1(r, BigInteger.Parse(textBox10.Text));
                if (p == r || p == 1)
                {
                    throw new Exception("Try to write another bound. The number is "+p);
                }
                var ts = watch.Elapsed;
                str = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);

                q = r / p;
                BigInteger fn = (p - 1) * (q - 1);
                BigInteger x1;
                BigInteger x2;
                BigInteger ee = BigInteger.Parse(textBox2.Text);
                EuclidExtended(fn, ee, out x1, out d, out x2);
                if (d < 0)
                {

                    d = d + fn;
                }
                msg = BigInteger.ModPow(BigInteger.Parse(textBox3.Text), d, r);
               
                textBox5.Text = p.ToString();
                textBox6.Text = q.ToString();
                textBox7.Text = d.ToString();
                textBox8.Text = str;
              


                textBox9.Text = ConvertFromWin(msg);


            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }
    }
}
