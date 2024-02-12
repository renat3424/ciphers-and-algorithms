using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace WindowsFormsApp1
{
    abstract class Cypher
    {


        string RU = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя0123456789";
        string EN = "abcdefghijklmnopqrstuvwxyz0123456789";
        protected string alphabet;

        public Cypher()
        {

            alphabet = EN;
        }



        public void ChangeLang(string lang)
        {

            if (lang == "RU")
            {


                alphabet = RU;
            }
            else
            {
                alphabet = EN;
            }

        }

        protected string RemovePunctuation(string Text)
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



        abstract public string Decode(string Text, string Key);

        abstract public string Encode(string Text, string Key);

        abstract public void CheckKey(string Key);


        protected bool CheckLang(string Text)
        {


            for (int i = 0; i < Text.Length; i++)
            {


                if (char.IsPunctuation(Text[i]))
                {

                    return false;
                }
            }

            if (alphabet == EN)
            {


                Regex regex = new Regex("^[a-zA-Z0-9. -_?]*$");

                if (!regex.IsMatch(Text))
                {
                    return false;

                }
            }
            else if (alphabet == RU)
            {

                Regex regex = new Regex("^[а-яА-Я0-9. -_?]*$");
                if (!regex.IsMatch(Text))
                {
                    return false;

                }
            }

            return true;

        }

    }















}
