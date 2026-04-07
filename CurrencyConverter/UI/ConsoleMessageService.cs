using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverter.UI
{
    public interface IMessageService
    { 
        void PrintMessage(string message, MessageType type = MessageType.Default);
        void PrintTitle(string title);
    }

    public enum MessageType
    {
        Default,
        Info,
        Success,
        Error,
        Accent,
        Title
    }

    public class ConsoleMessageService: IMessageService
        {

        public void PrintMessage(string message, MessageType type = MessageType.Default)
            {
                switch (type)
                {
                    case MessageType.Info:
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        break;

                    case MessageType.Success:
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;

                    case MessageType.Error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;

                    case MessageType.Accent:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;

                    case MessageType.Title:
                        PrintTitle(message);
                        return;

                    default:
                        Console.ResetColor();
                        break;
                }

                Console.WriteLine(message);
                Console.ResetColor();
            }

            public void PrintTitle(string title)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;

                string line = new string('=', title.Length + 8);

                Console.WriteLine(line);
                Console.WriteLine($"=== {title} ===");
                Console.WriteLine(line);

                Console.ResetColor();
            }


        }
    
}
