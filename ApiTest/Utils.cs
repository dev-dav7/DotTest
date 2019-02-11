using System;
using System.Linq;

namespace DotTest
{
    class Utils
    {
        /// <summary>
        /// Проверяет есть-ли в строке символы отличные от цифры 0..9
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static public Tuple<bool, int> IsNumericString(string str)
        {
            return CheckThisString(str, true, false, false);
        }

        /// <summary>
        /// Определяет содержит ли входящая строка недпоустимые символы
        /// Допустимые: латинский алфавит, цифры 0..9, '.', '_'
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static public Tuple<bool, int> IsNormalString(string str)
        {
            return CheckThisString(str, true, true, true);
        }

        /// <summary>
        /// Определяет содержит ли входящая строка недпоустимые символы
        /// </summary>
        /// <param name="str"></param>
        /// <param name="numCheck">Добавляет к допустимым символвам цифры от 0 до 9</param>
        /// <param name="enCheck">Добавляет к допустимым символам буквы латинского алфавита</param>
        /// <param name="otherCheck">Добавляет к допустимым символвам точку и нижнее подчеркнивание</param>
        /// <returns></returns>
        static Tuple<bool, int> CheckThisString(string str, bool numCheck, bool enCheck, bool otherCheck)
        {
            str = str.ToLower();
            for (int i = 0; i < str.Count(); i++)
                if (!(
                    (IsNumberSymbol((int)str[i]) && numCheck) ||
                    (IsEnAlphabetSymbol((int)str[i]) && enCheck) ||
                    (IsOtherSymbol((int)str[i]) && otherCheck)))
                    return Tuple.Create(false, i);
            return Tuple.Create(true, -1);
        }

        /// <summary>
        /// Является-ли ASCII код символова кодом числа
        /// </summary>
        /// <param name="asciiNumber"></param>
        /// <returns></returns>
        static bool IsNumberSymbol(int asciiNumber)
        {
            return asciiNumber >= 48 && asciiNumber <= 57;
        }

        /// <summary>
        /// Являетися-ли ASCII код символа кодом буквы латинского алфавита
        /// </summary>
        /// <param name="asciiNumber"></param>
        /// <returns></returns>
        static bool IsEnAlphabetSymbol(int asciiNumber)
        {
            return asciiNumber >= 97 && asciiNumber <= 122;
        }

        /// <summary>
        /// Являетися-ли ASCII код символа кодом буквы кирилличесокго алфавита
        /// </summary>
        /// <param name="asciiNumber"></param>
        /// <returns></returns>
        static bool IsRuAlphabetSymbol(int asciiNumber)
        {
            return asciiNumber >= 1072 && asciiNumber <= 1103;
        }

        /// <summary>
        /// Является-ли ASII код символа кодом доступного символа для сокращенного имени в ВК "." "_"
        /// </summary>
        /// <param name="asciiNumber"></param>
        /// <returns></returns>
        static bool IsOtherSymbol(int asciiNumber)
        {
            return asciiNumber == 95 || asciiNumber == 46;//95 = "."; 46 == "."
        }

    }
}
