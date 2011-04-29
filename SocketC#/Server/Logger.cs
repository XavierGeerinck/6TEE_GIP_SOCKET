using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    class Logger
    {
        public enum LogType
        {
            Socket,
            Error,
            User
        }

        static public void addlog(string log, LogType logtype)
        {
            switch (logtype)
            {
                case LogType.Socket:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[Socket]: " + log);
                    break;
                case LogType.User:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("[User]: " + log);
                    break;
                case LogType.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[Error]: " + log);
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
            }
            
        }
    }
}
