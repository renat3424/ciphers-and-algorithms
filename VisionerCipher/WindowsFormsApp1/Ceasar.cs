using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    class Ceasar : Cypher
    {

        public int RightNumber(int num)
        {


            int r = alphabet.Length;

            if (num < r && num >= 0)
            {

                return num;
            }

            if (num >= r)
            {

                return (num % r);
            }
            else
            {

                return ((num % r == 0) ? 0 : (r + num % r));
            }
        }


        string Algo(string Text, int Key)
        {

            string str = "";

            for (int i = 0; i < Text.Length; i++)
            {


                if (alphabet.Contains(Text[i].ToString()))
                {


                    str = str + alphabet[RightNumber(alphabet.IndexOf(Text[i]) + Key)];
                }
                else
                {
                    str = str + Text[i];
                }

            }
            return str;



        }

        override public string Encode(string Text, string Key)
        {

            Text = RemovePunctuation(Text);
            Text = Text.ToLower();

            return Algo(Text, int.Parse(Key));
        }
        override public string Decode(string Text, string Key)
        {
            Text = Text.ToLower();

            return Algo(Text, (-1 * int.Parse(Key)));
        }

        override public void CheckKey(string Key)
        {

            try
            {
                int a = int.Parse(Key);
            }
            catch
            {

                throw new Exception("No words in key, not more than an integer value!");

            }

        }







    }
}
