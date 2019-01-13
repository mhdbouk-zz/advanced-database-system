using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class ConsoleSpiner
    {
        int counter;
        string[] sequence;

        public ConsoleSpiner()
        {
            counter = 0;
            sequence = new string[] { ".   ", "..  ", "... ", "....", ".....", "......", ".......", "........" };
        }

        public void Turn()
        {
            counter++;

            if (counter >= sequence.Length)
                counter = 0;

            Console.Write(sequence[counter]);
            Console.SetCursorPosition(Console.CursorLeft - sequence[counter].Length, Console.CursorTop);
        }
    }
}
