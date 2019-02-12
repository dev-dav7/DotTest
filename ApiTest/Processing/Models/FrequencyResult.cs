using System;
using System.Globalization;

namespace DotTest.Processing.Models
{
    class FrequencyResult
    {
        public char symbol;
        public decimal frequency;

        public FrequencyResult(char s, decimal f)
        {
            symbol = s;
            frequency = f;
        }

        public void ConsoleView(int round)
        {
            Console.Write("{0}:{1}   ",symbol, decimal.Round(frequency,round));
        }

        public string JSONView(int round, CultureInfo x)
        {
            return "'" + symbol + "': " + decimal.Round(frequency, round).ToString(x);
        }
    }
}
