using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotTest.Processing.Alphabets.Models;

namespace DotTest.Processing.Alphabets
{
    class AlphabetController
    {

        //Список алфавитов
        List<AlphabetModel> alphabets = new List<AlphabetModel>();
        //Список useState алфавитов
        List<bool> _useStateAlphabets = new List<bool>();
        public List<bool> useStateAlphabets
        {
            get
            {
                return _useStateAlphabets;
            }
        }

        /// <summary>
        /// Создает и добавляет новый алфавит 
        /// </summary>
        /// <param name="alphabets"></param>
        public void AddNewAlphabet(string name, string _alphabets, bool isUse)
        {
            _alphabets.ToLower();
            List<int> asciiNom = new List<int>();
            //Символы за пределами ascii таблицы будут искажены
            foreach (char c in _alphabets)
                asciiNom.Add((int)c);
            //Если был задан пустой алфавит, не будем его добавлять
            if (asciiNom.Count() == 0)
            {
                Console.WriteLine("Alphabet is not added. Error: empty alphabet");
                return;
            }
            AlphabetModel newAlphabet = new AlphabetModel(name, asciiNom, isUse);
            //Добавляем новый алфваит
            alphabets.Add(newAlphabet);
            //Добавляем его состояние в таблциу состояний
            _useStateAlphabets.Add(newAlphabet.useState);
        }

        /// <summary>
        /// Устаналивает useState для алфавита
        /// </summary>
        /// <param name="idAlphabet">Номер</param>
        /// <param name="newState">Новое состояние</param>
        public void SetAlphabetsUseState(int idAlphabet, bool newState)
        {
            alphabets[idAlphabet].useState = newState;
            _useStateAlphabets[idAlphabet] = newState;
        }

        /// <summary>
        /// Загрузка алфавитов
        /// </summary>
        public void LoadAlphabets()
        {
            //Для теста создадим алфавиты в программе, вместо загрузки из БД
            AddNewAlphabet("English", "abcdefghijklmnopqrstuvwxyz", true);
            AddNewAlphabet("Russian", "абвгдеёжзийклмнопрстуфхцчшщъьэюя", true);
        }
    }
}
