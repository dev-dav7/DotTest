using DotTest.Processing.Models;
using System;
using System.Collections.Generic;

namespace DotTest.Processing
{
    static class FrequencyCalculator
    {

        /// <summary>
        /// Возвращает частотность для букв английского и русского алфавита
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Tuple<List<FrequencyResult>, List<FrequencyResult>> CalcFrequency(List<string> text)
        {
            decimal[] ruAlphabet = new decimal[32];//'й' не учитывается
            decimal[] enAlphabet = new decimal[26];
            decimal countRuSymbol = 0;
            decimal countEnSymbol = 0;

            foreach (var str in text)
                foreach (char c in str)
                {
                    //Английский алфавит
                    if (c >= 97 && c <= 122)
                    {
                        enAlphabet[c - 97]++;
                        countEnSymbol++;
                    }
                    //Русский алфавит
                    if (c >= 1072 && c <= 1103)
                    {
                        ruAlphabet[c - 1072]++;
                        countRuSymbol++;
                    }
                }
            List<FrequencyResult> en = new List<FrequencyResult>();
            List<FrequencyResult> ru = new List<FrequencyResult>();
            //Рассчет частнотности
            for (int i = 0; i < 26; i++)
            {
                if (countEnSymbol != 0)
                enAlphabet[i] = enAlphabet[i] / countEnSymbol;
                en.Add(new FrequencyResult((char)(97 + i), enAlphabet[i]));
            }
            for (int i = 0; i < 32; i++)
            {
                if (countRuSymbol != 0)
                ruAlphabet[i] = ruAlphabet[i] / countRuSymbol;
                ru.Add(new FrequencyResult((char)(1072 + i), ruAlphabet[i]));
            }
            
            return Tuple.Create(en, ru);
        }
    }
}
