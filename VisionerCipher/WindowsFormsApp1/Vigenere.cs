using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    class Vigenere:Cypher
    {

        string Algo(string Text, string Key, bool f)
        {

            string str = "";



            int k = 0;
            int y;
            for (int i = 0; i < Text.Length; i++)
            {


                if (alphabet.Contains(Text[i].ToString()))
                {
                    if (f)
                    {
                        str = str + alphabet[(alphabet.IndexOf(Text[i]) + alphabet.IndexOf(Key[k % Key.Length])) % alphabet.Length];

                    }
                    else
                    {


                        y = (alphabet.IndexOf(Text[i]) - alphabet.IndexOf(Key[k % Key.Length])) % alphabet.Length;


                        str = str + alphabet[(y < 0) ? alphabet.Length + y : y];

                    }
                    k++;

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
            CheckKey(Key);
            Text = RemovePunctuation(Text);
            Text = Text.ToLower();
            Key = Key.ToLower();

            return Algo(Text, Key, true);
        }

        override public string Decode(string Text, string Key)
        {
            Text = Text.ToLower();
            Key = Key.ToLower();

            return Algo(Text, Key, false);
        }

        override public void CheckKey(string Key)
        {
            if (Key == "")
            {
                throw new Exception("Key field is empty");
            }

            for (int i = alphabet.Length - 1; i >= alphabet.Length - 10; i--)
            {

                if (Key.Contains(alphabet[i].ToString()))
                {
                    throw new Exception("There must not be any numbers");

                }
            }


            if (!CheckLang(Key))
            {

                throw new Exception("The Key contains unproper symbols. Try to change language or remove unproper symbols");

            }

        }
    }
}
