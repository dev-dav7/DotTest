using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotTest.Processing.Alphabets.Models
{
    /// <summary>
    /// Модель данных алфавита
    /// </summary>
    class AlphabetModel
    {
        public string name;
        public bool useState = false;
        public List<int> asciiNom = new List<int>();

        public AlphabetModel(string _name, List<int> _alpNom, bool _useState)
        {
            name = _name;
            asciiNom = _alpNom;
            useState = _useState;
        }

        /// <summary>
        /// Считает часточтность букв заданного текста для этого алфавита
        /// </summary>
        /// <param name="texts"></param>
        /// <returns></returns>
        public List<decimal> GetStatistic(List<string> texts)
        {
            return null;
        }
    }
}
