using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp8
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class S : IS
    {

        BigInteger e;
        BigInteger d;
        BigInteger n;
        string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
        public S()
        {
            BigInteger p = GeneratePrime(128);
            BigInteger q = GeneratePrime(128);
            n = p * q;
            BigInteger fn = (p - 1) * (q - 1);
            e = GeneratePrime(2*128/3);
            BigInteger x1;
            BigInteger x2;
            EuclidExtended(fn, e, out x1, out d, out x2);
            if (d < 0)
            {

                d = d + fn;
            }

            Console.WriteLine("Keys are generated\nn="+n+"\ne="+e+"\nd="+d+"\n");
        }

        private string GetHash(string input)
        {
            var val = MD5.Create();
            var hash = val.ComputeHash(Encoding.UTF8.GetBytes(input));


            return Convert.ToBase64String(hash);

        }
        public bool CheckKey(string realMsg, string Key)
        {

            Console.WriteLine("Message is "+realMsg);
            Console.WriteLine("Key is " + Key);
            string[] key = Key.Split(' ');
            string res = "";
            BigInteger num;
            for (int i = 0; i < key.Length; i++)
            {



                           string s = key[i];
                if (s != " " && s!="")
                {
                    num = BigInteger.Parse(key[i]);
                    num = ModPow(num, d, n);
                    if (num >= alphabet.Length)
                    {
                        num = num % alphabet.Length;
                    }
                    res += alphabet[(int)num];

                }


                        


                    
                    
                }

            
            string res1 = GetHash(realMsg);
            Console.WriteLine("Decrypted key is " + res);
            Console.WriteLine("Hashed msg is " + res1);
            if (res1 == res)
            {
                return true;
            }
            else
            {
                return false;
            }
           

                

            }

        

        public BigInteger GetE()
        {
            return e;
        }

        public BigInteger GetN()
        {
            return n;
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


        void EuclidExtended(BigInteger A, BigInteger B, out BigInteger x, out BigInteger y, out BigInteger C)//расширенный алгоритм Евклида
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


        BigInteger OddNum(int r)//Генерация рандомного нечетного числа заданного количества бит
        {
            BigInteger num = BigInteger.Pow(2, r - 1) + 1;
            Random rnum = new Random(Guid.NewGuid().GetHashCode());
            for (int i = r - 2; i > 0; i--)
            {

                num += BigInteger.Pow((rnum.Next() % 2) * 2, i);

            }

            return BigInteger.Abs(num);

        }

        bool CheckComposite(BigInteger r, BigInteger a, BigInteger t, BigInteger s)//функция проверки является ли число составным(часть проверки на простоту Миллера Рабина)
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

        bool IsPrime(BigInteger r)//Алгоритм проверки на простоту Миллера-Рабина
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


        BigInteger getRandom(int length, BigInteger a, BigInteger b)// Получения рандомного большого числа заданного количества бит больше чем число a, но меньше b
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
        BigInteger GeneratePrime(int length)//Генерация простого числа
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
