using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RingloggerParser
{
    class RingloggerParser
    {
        static void Main(string[] args)
        {
            // Test if input arguments were supplied.
            if (args.Length < 2)
            {
                Console.WriteLine("Please enter input file and output file");
                Console.WriteLine("Usage: RingloggerParser.exe <input> <output>");
                return;
            }
            var logTxt = File.CreateText(args[1]);
            Ringlogger Ringlogger = new Ringlogger(args[0], "FPN");
            Ringlogger.WriteTo(logTxt);
            logTxt.Close();
        }
    }
}
