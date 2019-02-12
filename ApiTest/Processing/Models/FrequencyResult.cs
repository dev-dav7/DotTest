using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
