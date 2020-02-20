using System;
using BattleToad.Ext;
using BattleToad.Logs;
using System.Linq;
using static BattleToad.ConsoleAddons.ConsoleEx;
using System.IO;
using BattleToad.FastHttpClient;
using System.Text;
using System.Net;
using BattleToad.RosSvyaz;
using System.Timers;
using static BattleToad.Ext.Notifies;
using System.Threading;
using System.Text.RegularExpressions;
using BattleToad.JSONHelper;
using BattleToad.Users;
using BattleToad.ConsoleAddons;
using BattleToad.Progress;
using System.Collections.Generic;
using BattleToad.Strings;

namespace ConsoleApp5
{
    class Program
    {
        private static dynamic x
        {
            set
            {
                Print($"x:\n{value}\n");
            }
        }

        static void Main()
        {
            File.Delete("log.txt");
            WORK();
            x = "Complete";
            NonStop();
        }

        private static void NonStop()
        {
            while (true) { }
        }


        static void WORK()
        {
            Console.WriteLine(BattleToad.Ext.Hash.GetHashBase64StringFromString("12345678", Hash.Type.SHA512));
            
        }
    }
}