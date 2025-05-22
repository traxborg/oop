using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DZ
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            TextContainer text = new TextContainer();

            text.AddLine("Перший рядок");
            text.AddLine("Другий рядок");
            text.AddLine("Третій рядок");

            Console.WriteLine("Кількість рядків: " + text.LineCount);  

            
            Console.WriteLine("Рядок 1: " + text[0]); 

           
            text[1] = "Оновлений другий рядок";
            Console.WriteLine("Рядок 2: " + text[1]);

           
            text.Clear();
            Console.WriteLine("Кількість після очищення: " + text.LineCount);  
        }
    }
    }

