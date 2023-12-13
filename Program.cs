using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Xml.Serialization;

class Program
{
    static void Main()
    {
        Console.Clear();
        Turtle myTurtle = new Turtle();
        myTurtle.PrintMenu();
        Console.WriteLine(myTurtle);
        while (true)
        {
            string command = Console.ReadLine();

            if (command == "exit")
            {   
                Console.Clear();
                break;
            }
            Console.Clear();
            myTurtle.PrintMenu();
            myTurtle.ProcessCommand(command);
            Console.WriteLine($"\n{myTurtle} | Prev command is [{command}]");
        }
    }
}
