using System;
using Flash.Console.UserInterface;

namespace Flash.Console
{
    public class Console
    {
        private static UserInput userInput = new UserInput();
        static void Main()
        {
            System.Console.WriteLine("\n\n Prepare to study! Please choose from the following options: \n");
            userInput.HandleUserInput();
        }
    }
}